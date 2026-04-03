using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Users.Common;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BookLAB.Application.Features.Users.Commands.ImportUsers
{
    public class ConfirmUserImportHandler : IRequestHandler<ConfirmUserImportCommand, bool>
    {
        private static readonly Regex EmailRegex = new(
            @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly IUnitOfWork _unitOfWork;

        public ConfirmUserImportHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ConfirmUserImportCommand request, CancellationToken cancellationToken)
        {
            var normalizedEmails = request.ValidUsers
                .Select(x => (x.Email ?? string.Empty).Trim().ToLowerInvariant())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var normalizedUserCodes = request.ValidUsers
                .Select(x => (x.UserCode ?? string.Empty).Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var roleNames = request.ValidUsers
                .Select(x => (x.RoleName ?? string.Empty).Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var campusIds = request.ValidUsers
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
            var usersToInsert = new List<User>();
            var userRolesToInsert = new List<UserRole>();

            foreach (var item in request.ValidUsers)
            {
                var normalizedEmail = (item.Email ?? string.Empty).Trim().ToLowerInvariant();
                var normalizedUserCode = (item.UserCode ?? string.Empty).Trim();
                var normalizedRoleName = (item.RoleName ?? string.Empty).Trim();

                var isInvalid =
                    string.IsNullOrWhiteSpace(item.FullName) || item.FullName.Trim().Length > 100 ||
                    string.IsNullOrWhiteSpace(normalizedEmail) || normalizedEmail.Length > 150 || !EmailRegex.IsMatch(normalizedEmail) ||
                    string.IsNullOrWhiteSpace(normalizedUserCode) || normalizedUserCode.Length > 100 ||
                    item.CampusId <= 0 || !campusSet.Contains(item.CampusId) ||
                    string.IsNullOrWhiteSpace(normalizedRoleName) || !roleMap.ContainsKey(normalizedRoleName) ||
                    !seenEmailSet.Add(normalizedEmail) || existingEmailSet.Contains(normalizedEmail) ||
                    !seenCodeSet.Add(normalizedUserCode) || existingCodeSet.Contains(normalizedUserCode);

                if (isInvalid)
                {
                    throw new Exception($"Dữ liệu import user không hợp lệ hoặc đã thay đổi ở user code '{item.UserCode}'.");
                }

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = item.FullName.Trim(),
                    Email = normalizedEmail,
                    UserCode = normalizedUserCode,
                    CampusId = item.CampusId,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UserImageUrl = string.Empty
                };

                usersToInsert.Add(newUser);
                userRolesToInsert.Add(new UserRole
                {
                    UserId = newUser.Id,
                    RoleId = roleMap[normalizedRoleName].Id
                });

                existingEmailSet.Add(normalizedEmail);
                existingCodeSet.Add(normalizedUserCode);
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Repository<User>().AddRangeAsync(usersToInsert);
                await _unitOfWork.Repository<UserRole>().AddRangeAsync(userRolesToInsert);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            return true;
        }
    }
}
