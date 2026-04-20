using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.SlotTypes.Command.CreateSlotType
{
    public class CreateSlotTypeHandler : IRequestHandler<CreateSlotTypeCommand, ResultMessage<SlotTypeDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateSlotTypeHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ResultMessage<SlotTypeDto>> Handle(CreateSlotTypeCommand request, CancellationToken cancellationToken)
        {
            int slotTypeId = _unitOfWork.Repository<SlotType>().Entities.Max(x => x.Id);

            SlotType slotType = new SlotType
            {
                Id = slotTypeId + 1,
                Code = request.Code,
                Name = request.Name,
                CampusId = _currentUserService.CampusId
            };

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Repository<SlotType>().AddAsync(slotType);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                int slotFrameId = _unitOfWork.Repository<SlotFrame>().Entities.Max(x => x.Id);

                await _unitOfWork.BeginTransactionAsync();

                foreach (var x in request.SlotFrames)
                {
                    slotFrameId++;
                    SlotFrame slotFrame = new SlotFrame
                    {
                        Id = slotFrameId,
                        SlotTypeId = slotType.Id,
                        StartTimeSlot = x.StartTime,
                        EndTimeSlot = x.EndTime,
                        OrderIndex = x.OrderIndex
                    };

                    await _unitOfWork.Repository<SlotFrame>().AddAsync(slotFrame);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return new ResultMessage<SlotTypeDto>
                {
                    Success = true,
                    Message = "Create Slot Type Successfully"
                };
            } catch (Exception ex)
            {
                return new ResultMessage<SlotTypeDto>
                {
                    Success = false,
                    Message = "Some thing is wrong while create slot type"
                };
            }
        }
    }
}
