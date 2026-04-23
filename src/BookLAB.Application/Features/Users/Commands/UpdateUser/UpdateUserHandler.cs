using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, ResultMessage<UserProfileDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateUserHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ResultMessage<UserProfileDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(request.Id);

            if (user == null)
                return new ResultMessage<UserProfileDto>
                {
                    Success = false,
                    Message = "User not found."
                };

            user.Email = request.Email ?? user.Email;
            user.FullName = request.FullName ?? user.FullName;
            user.IsActive = request.IsActive ?? user.IsActive;
            user.UserCode = request.UserCode ?? user.UserCode;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Repository<User>().UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                if (request.Roles != null && request.Roles.Count > 0)
                {
                    var existingUserRoles = await _unitOfWork.Repository<UserRole>().Entities.Where(x => x.UserId == request.Id).ToListAsync();
                    _unitOfWork.Repository<UserRole>().DeleteRange(existingUserRoles);
                    var newUserRoles = request.Roles.Select(roleId => new UserRole
                    {
                        UserId = request.Id.Value,
                        RoleId = roleId
                    }).ToList();
                    await _unitOfWork.BeginTransactionAsync();
                    await _unitOfWork.Repository<UserRole>().AddRangeAsync(newUserRoles);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    await _unitOfWork.CommitTransactionAsync();
                }

                return new ResultMessage<UserProfileDto>
                {
                    Success = true,
                    Message = "User updated successfully.",
                    Data = new UserProfileDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        UserCode = user.UserCode,
                        CampusId = user.CampusId,
                        AvatarUrl = user.UserImageUrl
                    }
                };
            } catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ResultMessage<UserProfileDto>
                {
                    Success = false,
                    Message = $"An error occurred while updating the user: {ex.Message}"
                };
            }
        }
    }
}
