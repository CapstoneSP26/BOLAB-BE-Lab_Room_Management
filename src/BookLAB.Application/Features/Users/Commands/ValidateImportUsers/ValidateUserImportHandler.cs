using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Users.Common;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BookLAB.Application.Features.Users.Commands.ValidateImportUsers
{
    public class ValidateUserImportHandler : IRequestHandler<ValidateUserImportQuery, ImportValidationResult<UserImportDto>>
    {
        private static readonly Regex EmailRegex = new(
            @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly IUnitOfWork _unitOfWork;

        public ValidateUserImportHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ImportValidationResult<UserImportDto>> Handle(ValidateUserImportQuery request, CancellationToken cancellationToken)
        {
            var response = new ImportValidationResult<UserImportDto>();

            var normalizedEmails = request.Users
                .Select(x => (x.Email ?? string.Empty).Trim().ToLowerInvariant())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var normalizedUserCodes = request.Users
                .Select(x => (x.UserCode ?? string.Empty).Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var roleNames = request.Users
                .Select(x => (x.RoleName ?? string.Empty).Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var campusIds = request.Users
                .Select(x => x.CampusId)
                .Distinct()
                .ToList();

            var existingEmailSet = (await _unitOfWork.Repository<User>().Entities
                .Where(u => normalizedEmails.Contains(u.Email.ToLower()))
                .Select(u => u.Email)
                .ToListAsync(cancellationToken))
                .Select(x => x.Trim().ToLowerInvariant())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var existingCodeSet = (await _unitOfWork.Repository<User>().Entities
                .Where(u => normalizedUserCodes.Contains(u.UserCode))
                .Select(u => u.UserCode)
                .ToListAsync(cancellationToken))
                .Select(x => x.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var roleMap = (await _unitOfWork.Repository<Role>().Entities
                .Where(r => roleNames.Contains(r.RoleName))
                .ToListAsync(cancellationToken))
                .ToDictionary(r => r.RoleName.Trim(), r => r, StringComparer.OrdinalIgnoreCase);

            var campusSet = (await _unitOfWork.Repository<Campus>().Entities
                .Where(c => campusIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync(cancellationToken))
                .ToHashSet();

            var seenEmailSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var seenCodeSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < request.Users.Count; i++)
            {
                var item = request.Users[i];
                var rowResult = new RowResult<UserImportDto>
                {
                    RowNumber = i + 2,
                    Data = item
                };

                var normalizedEmail = (item.Email ?? string.Empty).Trim().ToLowerInvariant();
                var normalizedUserCode = (item.UserCode ?? string.Empty).Trim();
                var normalizedRoleName = (item.RoleName ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(item.FullName) || item.FullName.Trim().Length > 100)
                {
                    rowResult.Messages.Add("Họ tên bắt buộc và tối đa 100 ký tự.");
                    rowResult.Status = "Invalid";
                    rowResult.IsCritical = true;
                }

                if (string.IsNullOrWhiteSpace(normalizedEmail) || normalizedEmail.Length > 150 || !EmailRegex.IsMatch(normalizedEmail))
                {
                    rowResult.Messages.Add("Email không hợp lệ hoặc vượt quá 150 ký tự.");
                    rowResult.Status = "Invalid";
                    rowResult.IsCritical = true;
                }

                if (string.IsNullOrWhiteSpace(normalizedUserCode) || normalizedUserCode.Length > 100)
                {
                    rowResult.Messages.Add("Mã user bắt buộc và tối đa 100 ký tự.");
                    rowResult.Status = "Invalid";
                    rowResult.IsCritical = true;
                }

                if (item.CampusId <= 0 || !campusSet.Contains(item.CampusId))
                {
                    rowResult.Messages.Add($"CampusId '{item.CampusId}' không tồn tại.");
                    rowResult.Status = "Invalid";
                    rowResult.IsCritical = true;
                }

                if (string.IsNullOrWhiteSpace(normalizedRoleName) || !roleMap.ContainsKey(normalizedRoleName))
                {
                    rowResult.Messages.Add($"Role '{item.RoleName}' không tồn tại.");
                    rowResult.Status = "Invalid";
                    rowResult.IsCritical = true;
                }

                if (!string.IsNullOrWhiteSpace(normalizedEmail))
                {
                    if (!seenEmailSet.Add(normalizedEmail))
                    {
                        rowResult.Messages.Add("Email bị trùng trong file import.");
                        rowResult.Status = "Invalid";
                        rowResult.IsCritical = true;
                    }
                    else if (existingEmailSet.Contains(normalizedEmail))
                    {
                        rowResult.Messages.Add("Email đã tồn tại trên hệ thống.");
                        rowResult.Status = "Invalid";
                        rowResult.IsCritical = true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(normalizedUserCode))
                {
                    if (!seenCodeSet.Add(normalizedUserCode))
                    {
                        rowResult.Messages.Add("Mã user bị trùng trong file import.");
                        rowResult.Status = "Invalid";
                        rowResult.IsCritical = true;
                    }
                    else if (existingCodeSet.Contains(normalizedUserCode))
                    {
                        rowResult.Messages.Add("Mã user đã tồn tại trên hệ thống.");
                        rowResult.Status = "Invalid";
                        rowResult.IsCritical = true;
                    }
                }

                response.Rows.Add(rowResult);
            }

            return response;
        }
    }
}
