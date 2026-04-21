using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.SlotTypes.Command.DeleteSlotType
{
    public class DeleteSlotTypeHandler : IRequestHandler<DeleteSlotTypeCommand, ResultMessage<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteSlotTypeHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ResultMessage<bool>> Handle(DeleteSlotTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var slotType = await _unitOfWork.Repository<SlotType>().GetByIdAsync(request.Id);

                if (slotType == null)
                    return new ResultMessage<bool>
                    {
                        Success = false,
                        Message = "Slot Type is not existed"
                    };

                await _unitOfWork.BeginTransactionAsync();
                _unitOfWork.Repository<SlotType>().Delete(slotType);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return new ResultMessage<bool>
                {
                    Success = true,
                    Message = "Delete Slot Type Successfully"
                };
            } catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ResultMessage<bool>
                {
                    Success = false,
                    Message = $"Some thing is wrong while delete slot type with id = {request.Id}"
                };
            }
            

        }
    }
}
