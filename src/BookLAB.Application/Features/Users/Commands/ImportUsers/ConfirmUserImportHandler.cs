using BookLAB.Application.Common.Helpers;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using System.Collections;
using System.Data;

namespace BookLAB.Application.Features.Users.Commands.ImportUsers
{
    public class ConfirmUserImportHandler : IRequestHandler<ConfirmUserImportCommand, ImportResult>
    {
        private readonly IUserImportService _userImportService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        public ConfirmUserImportHandler(IUserImportService userImportService, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _userImportService = userImportService;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ImportResult> Handle(ConfirmUserImportCommand request, CancellationToken cancellationToken)
        {
            var response = await _userImportService.ValidateAsync(request.Users, request.CampusId, cancellationToken, true);
            var result = response.result;
            var maps = response.maps;
            var countUpdated = result.Rows.Count(r => r.Data.IsUpdated);
            var countNew = result.Rows.Count(r => !r.Data.IsUpdated);
            var now = DateTimeOffset.UtcNow;
            if (!result.CanCommit)
            {
                return new ImportResult
                {
                    Success = false,
                };
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newUserList = new List<User>();
                var newUserRoleList = new List<UserRole>();
                foreach (var row in result.Rows)
                {
                    var entity = row.ConvertedEntity;
                    if (row.Data.IsUpdated)
                    {
                        entity.UpdatedAt = now;
                        entity.UpdatedBy = _currentUserService.UserId;
                        var roles = RoleHelper.ParseRoles(row.Data.RoleNames);
                        var newUserRoles = roles.Select(r => new UserRole
                        {
                            RoleId = maps.RoleMap[r].Id,
                            UserId = entity.Id
                        }).ToList();
                        var deletedUserRoles = entity.UserRoles
                            .Where(a => !newUserRoles.Any(b => b.UserId == a.UserId && b.RoleId == a.RoleId))
                            .ToList();
                        var addedUserRoles = newUserRoles
                            .Where(a => !entity.UserRoles.Any(b => b.UserId == a.UserId && b.RoleId == a.RoleId))
                            .ToList();
                        _unitOfWork.Repository<UserRole>().DeleteRange(deletedUserRoles);

                        newUserRoleList.AddRange(addedUserRoles);
                        _unitOfWork.Repository<User>().Update(entity);
                    }
                    else
                    {
                        entity.CreatedAt = now;
                        entity.CreatedBy = _currentUserService.UserId;
                        newUserList.Add(entity);
                    }
                }
                if (newUserList.Any())
                {
                    await _unitOfWork.Repository<User>().AddRangeAsync(newUserList);
                    foreach (var user in newUserList)
                    {
                        foreach (var role in user.UserRoles)
                        {
                            role.UserId = user.Id;
                        }
                    }
                }
                if (newUserRoleList.Any())
                {
                    await _unitOfWork.Repository<UserRole>().AddRangeAsync(newUserRoleList);

                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return new ImportResult
                {
                    Success = true,
                    CreatedCount = countNew,
                    UpdatedCount = countUpdated,
                    TotalProcessed = result.Rows.Count,
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

        }
    }
}
