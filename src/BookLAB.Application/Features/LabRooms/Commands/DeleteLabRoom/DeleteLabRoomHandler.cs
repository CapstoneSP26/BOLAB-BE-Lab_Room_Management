using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.LabRooms.Commands.DeleteLabRoom
{
    public class DeleteLabRoomHandler : IRequestHandler<DeleteLabRoomCommand, ResultMessage<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteLabRoomHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ResultMessage<bool>> Handle(DeleteLabRoomCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var labRoom = await _unitOfWork.Repository<LabRoom>().GetByIdAsync(request.LabRoomId);

                if (labRoom == null)
                    return new ResultMessage<bool>
                    {
                        Success = false,
                        Message = "Lab Room is not existed"
                    };

                await _unitOfWork.BeginTransactionAsync();
                _unitOfWork.Repository<LabRoom>().Delete(labRoom);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return new ResultMessage<bool>
                {
                    Success = true,
                    Message = $"Delete lab room successfully"
                };
            } catch (Exception ex)
            {
                return new ResultMessage<bool>
                {
                    Success = false,
                    Message = $"Some thing is wrong while delete lab room with id = {request.LabRoomId}"
                };
            }
        }
    }
}
