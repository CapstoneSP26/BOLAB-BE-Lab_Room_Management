using AutoMapper;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Users.Commands.DeleteUser
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, ResultMessage<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public DeleteUserHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ResultMessage<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {

            var user = await _unitOfWork.Repository<User>().GetByIdAsync(request.Id);
            var userRoles = await _unitOfWork.Repository<UserRole>().Entities.Where(x => x.UserId == request.Id).ToListAsync();

            if (user == null)
                return new ResultMessage<bool>
                {
                    Success = false,
                    Message = "User is not existed"
                };
            try
            {

                await _unitOfWork.BeginTransactionAsync();
                _unitOfWork.Repository<UserRole>().DeleteRange(userRoles);
                _unitOfWork.Repository<User>().Delete(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return new ResultMessage<bool>
                {
                    Success = true,
                    Message = "Delete user successfully",
                };

            } catch (DbUpdateException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ResultMessage<bool>
                {
                    Success = false,
                    Message = "Delete this user violates foreign key on other table"
                };
            } catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ResultMessage<bool>
                {
                    Success = false,
                    Message = "Delete user failed"
                };
            }
            
        }
    }
}
