using AutoMapper;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, ResultMessage<UserProfileDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateUserHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ResultMessage<UserProfileDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var campusId = _currentUserService.CampusId;
            var userid = _currentUserService.UserId;
            var createdUserId = Guid.NewGuid();

            User user = new User
            {
                Id = createdUserId,
                Email = request.Email,
                FullName = request.FullName,
                UserCode = request.UserCode,
                CampusId = campusId,
                CreatedBy = userid,
                CreatedAt = DateTimeOffset.UtcNow,
                IsDeleted = false,
                IsActive = request.IsActive ?? true,
            };

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Repository<User>().AddAsync(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                List<UserRole> userRoles = new List<UserRole>();
                request.Roles.ForEach(x => userRoles.Add(new UserRole
                {
                    RoleId = x,
                    UserId = createdUserId
                }));

                if (userRoles.Count() > 0)
                {
                    await _unitOfWork.BeginTransactionAsync();
                    await _unitOfWork.Repository<UserRole>().AddRangeAsync(userRoles);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    await _unitOfWork.CommitTransactionAsync();
                }

                return new ResultMessage<UserProfileDto>
                {
                    Success = true,
                    Message = "User created successfully",
                    Data = _mapper.Map<UserProfileDto>(user)
                };

            } catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ResultMessage<UserProfileDto>
                {
                    Success = false,
                    Message = "Failed to create user"
                };
            }

            
        }
    }
}
