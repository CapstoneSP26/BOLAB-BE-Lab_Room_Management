using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.SlotTypes.Command.UpdateSlotType
{
    public class UpdateSlotTypeHandler : IRequestHandler<UpdateSlotTypeCommand, ResultMessage<SlotTypeDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateSlotTypeHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ResultMessage<SlotTypeDto>> Handle(UpdateSlotTypeCommand request, CancellationToken cancellationToken)
        {
            var slotType = await _unitOfWork.Repository<SlotType>().GetByIdAsync(request.Id);

            if (slotType == null)
                return new ResultMessage<SlotTypeDto>
                {
                    Success = false,
                    Message = "Slot Type is not exist"
                };

            slotType.Code = request.Code;
            slotType.Name = request.Name;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Repository<SlotType>().UpdateAsync(slotType);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                var slotFrames = await _unitOfWork.Repository<SlotFrame>().Entities.Where(x => x.SlotTypeId == request.Id).ToListAsync();

                await _unitOfWork.BeginTransactionAsync();

                SlotFrame slotFrame = new SlotFrame();
                List<SlotFrameTemp> addedSlotFrames = new List<SlotFrameTemp>();

                // Update SlotFrame loop
                foreach (var x in request.SlotFrames)
                {
                    slotFrame = slotFrames.FirstOrDefault(y => y.Id == x.Id);

                    if (slotFrame != null)
                    {
                        slotFrames.Remove(slotFrame);

                        slotFrame.StartTimeSlot = x.StartTime;
                        slotFrame.EndTimeSlot = x.EndTime;
                        slotFrame.OrderIndex = x.OrderIndex;

                        await _unitOfWork.Repository<SlotFrame>().UpdateAsync(slotFrame);
                        
                    } else addedSlotFrames.Add(x);

                }

                // Delete SlotFrame loop
                foreach (var x in slotFrames)
                    _unitOfWork.Repository<SlotFrame>().Delete(x);

                var slotFrameId = await _unitOfWork.Repository<SlotFrame>().Entities.MaxAsync(x => x.Id);

                // Create SlotFrame loop
                foreach (var x in addedSlotFrames)
                {
                    slotFrameId++;
                    SlotFrame addedSlotFrame = new SlotFrame
                    {
                        Id = slotFrameId,
                        SlotTypeId = request.Id.Value,
                        StartTimeSlot = x.StartTime,
                        EndTimeSlot = x.EndTime,
                        OrderIndex = x.OrderIndex
                    };

                    await _unitOfWork.Repository<SlotFrame>().AddAsync(addedSlotFrame);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return new ResultMessage<SlotTypeDto>
                {
                    Success = true,
                    Message = "Create Slot Type Successfully"
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ResultMessage<SlotTypeDto>
                {
                    Success = false,
                    Message = "Some thing is wrong while create slot type"
                };
            }
        }
    }
}
