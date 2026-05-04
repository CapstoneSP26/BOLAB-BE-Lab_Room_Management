using BookLAB.Application.Common.Helpers;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Users.Common;
using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;


namespace BookLAB.Infrastructure.Services
{
    public class UserImportService : IUserImportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserImportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserImportValidateResponse> ValidateAsync(
            List<UserImportDto> users,
            int campusId,
            CancellationToken cancellationToken,
            bool isAllowCreateImportData = false)
        {
            var result = new ImportValidationResult<UserImportDto, User>
            {
                TotalRows = users.Count
            };

            // ===== PRELOAD =====
            var maps = await BuildContext(users, campusId, cancellationToken);

            // ===== VALIDATE =====
            for (int i = 0; i < users.Count; i++)
            {
                var row = new RowResult<UserImportDto, User>
                {
                    RowNumber = i,
                    Data = users[i]
                };

                ValidateSingleRow(users[i], row, maps);
                result.Rows.Add(row);
            }

            // ===== Convert Entities =====
            if(isAllowCreateImportData && result.CanCommit)
            {
                foreach (var row in result.Rows)
                {
                    row.ConvertedEntity = MapToEntity(row.Data, maps);
                }
            };
            var response = new UserImportValidateResponse
            {
                result = result,
                maps = maps
            };
            return response;
        }

        private async Task<UserImportMaps> BuildContext(
            List<UserImportDto> users,
            int campusId,
            CancellationToken cancellationToken)
        {
            foreach (var user in users)
            {
                user.Email = user.Email?.Trim().ToLower();
                user.UserCode = user.UserCode?.Trim().ToUpper();
                user.CampusCode = user.CampusCode?.Trim().ToLower();
            }
            var emails = users.Select(x => x.Email)
                              .Where(x => !string.IsNullOrWhiteSpace(x))
                              .Distinct()
                              .ToList();

            var userCodes = users.Select(x => x.UserCode)
                             .Where(x => !string.IsNullOrWhiteSpace(x))
                             .Distinct()
                             .ToList();

            var campusCodes = users.Select(x => x.CampusCode)
                               .Where(x => !string.IsNullOrWhiteSpace(x))
                               .Distinct()
                               .ToList();

            var roleNames = users
                .SelectMany(x => RoleHelper.ParseRoles(x.RoleNames))
                .Distinct()
                .ToList();

            var existingCodes = (await _unitOfWork.Repository<User>().Entities
                .Where(x => userCodes.Contains(x.UserCode) && x.CampusId == campusId)
                .Select(x => x.UserCode)
                .ToListAsync(cancellationToken))
                .Select(x => x.Trim())
                .ToHashSet();

            var userMap = (await _unitOfWork.Repository<User>().Entities
                .Where(x => (emails.Contains(x.Email) || userCodes.Contains(x.UserCode)) && x.CampusId == campusId)
                .Include(x => x.UserRoles)
                .ToListAsync(cancellationToken))
                .ToDictionary(x => x.Email.ToLower(), x => x);

            var campusMap = (await _unitOfWork.Repository<Campus>().Entities
                .Where(c => campusCodes.Contains(c.CampusCode) && c.Id == campusId)
                .ToListAsync(cancellationToken))
                .ToDictionary(c => c.CampusCode, c => c);

            var roleMap = (await _unitOfWork.Repository<Role>().Entities
                .Where(r => roleNames.Contains(r.RoleName))
                .ToListAsync(cancellationToken))
                .ToDictionary(r => r.RoleName, r => r);

            return new UserImportMaps
            {
                ExistingEmails = userMap.Keys.ToHashSet(),
                ExistingCodes = existingCodes,
                RoleMap = roleMap,
                CampusMap = campusMap,
                UserMap = userMap,
                SeenEmails = new HashSet<string>(),
                SeenCodes = new HashSet<string>()
            };
        }

        private static readonly Regex EmailRegex = new(
            @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
            RegexOptions.Compiled);

        public void ValidateSingleRow(UserImportDto dto, RowResult<UserImportDto, User> row, UserImportMaps ctx)
        {
            var email = dto.Email;
            var code = dto.UserCode;
            var campusCode = dto.CampusCode;
            var roles = RoleHelper.ParseRoles(dto.RoleNames);

            // ===== BASIC =====
            if (string.IsNullOrWhiteSpace(dto.FullName) || dto.FullName.Length > 100)      
                AddError(row, "FullName", "Họ tên không hợp lệ.", ErrorSeverity.Error);


            if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
                AddError(row, "Email", "Email không hợp lệ.", ErrorSeverity.Error);

            if (string.IsNullOrWhiteSpace(code))
                AddError(row, "UserCode", "UserCode bắt buộc.", ErrorSeverity.Error);

            // 🔥 validate campus theo code
            if (string.IsNullOrWhiteSpace(campusCode) || !ctx.CampusMap.ContainsKey(campusCode))
                AddError(row, "CampusCode", $"Campus '{dto.CampusCode}' không tồn tại.", ErrorSeverity.Error);

            // ===== ROLE =====
            if (!roles.Any())
            {
                AddError(row, "RoleNames", "Phải có ít nhất 1 role.", ErrorSeverity.Error);
            }
            else
            {
                var invalid = roles.Where(r => !ctx.RoleMap.ContainsKey(r)).ToList();
                if (invalid.Any())
                    AddError(row, "RoleNames", $"Role không tồn tại: {string.Join(", ", invalid)}", ErrorSeverity.Error);
            }

            // ===== DUPLICATE =====
            if (!string.IsNullOrWhiteSpace(email)) 
            {
                if (!ctx.SeenEmails.Add(email))
                    AddError(row, "Email", "Trùng trong file.", ErrorSeverity.Error);
                else if (ctx.UserMap.TryGetValue(email, out var existingUser))
                {
                    if(existingUser.UserCode == code)
                    {
                        AddError(row, "Email", $"User đã tồn tại", ErrorSeverity.Warning);
                        dto.IsUpdated = true;
                    }
                    else
                    {
                        AddError(row, "Email", "Email Đã tồn tại.", ErrorSeverity.Error);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(code))
            {
                if (!ctx.SeenCodes.Add(code))
                    AddError(row, "UserCode", "Trùng trong file.", ErrorSeverity.Error);
                else if (ctx.ExistingCodes.Contains(code) && !string.IsNullOrWhiteSpace(email) && !ctx.UserMap.ContainsKey(email))
                {
                    AddError(row, "UserCode", "UserCode Đã tồn tại.", ErrorSeverity.Error);
                }
            }
        }

        private void AddError(RowResult<UserImportDto, User> row, string field, string message, ErrorSeverity errorSeverity)
        {
            row.Errors.Add(new RowError
            {
                FieldName = field,
                Message = message,
                Severity = errorSeverity
            });
        }

        private User MapToEntity(UserImportDto dto, UserImportMaps maps)
        {
            var roles = RoleHelper.ParseRoles(dto.RoleNames);
            if (dto.IsUpdated)
            {

                var existingUser = maps.UserMap[dto.Email.Trim().ToLower()];
                existingUser.FullName = dto.FullName.Trim();

                return existingUser;
            }
            else
            {
                return new User
                {
                    FullName = dto.FullName.Trim(),
                    Email = dto.Email.Trim().ToLower(),
                    UserCode = dto.UserCode.Trim().ToUpper(),
                    CampusId = maps.CampusMap[dto.CampusCode.Trim().ToLower()].Id,
                    UserRoles = roles.Select(r => new UserRole
                    {
                        RoleId = maps.RoleMap[r].Id
                    }).ToList()
                };
            }
        }
    }
}
