using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.LabRooms.Commands.CreateLabRoom
{
    public class CreateLabRoomHandler : IRequestHandler<CreateLabRoomCommand, ResultMessage<LabRoomDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public CreateLabRoomHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<ResultMessage<LabRoomDto>> Handle(CreateLabRoomCommand request, CancellationToken cancellationToken)
        {
            int latestId = request.Id == 0 ? _unitOfWork.Repository<LabRoom>().Entities.Max(lr => lr.Id) : request.Id;

            LabRoom labRoom = new LabRoom
            {
                Id = latestId + 1,
                BuildingId = request.BuildingId,
                RoomName = request.RoomName,
                RoomNo = request.RoomNo,
                Location = request.Location,
                OverrideNumber = request.OverrideNumber,
                HasEquipment = request.HasEquipment,
                Capacity = request.Capacity,
                Description = request.Description,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = _currentUserService.UserId,
                IsActive = true,
                IsDeleted = false
            };

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Repository<LabRoom>().AddAsync(labRoom);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return new ResultMessage<LabRoomDto>
                {
                    Success = true,
                    Message = "Lab room created successfully",
                    Data = new LabRoomDto
                    {
                        Id = labRoom.Id,
                        BuildingId = labRoom.BuildingId,
                        RoomName = labRoom.RoomName,
                        RoomNo = labRoom.RoomNo,
                        Location = labRoom.Location,
                        OverrideNumber = labRoom.OverrideNumber,
                        HasEquipment = labRoom.HasEquipment,
                        Capacity = labRoom.Capacity,
                        Description = labRoom.Description,
                        CreatedAt = labRoom.CreatedAt,
                        CreatedBy = labRoom.CreatedBy,
                        IsActive = labRoom.IsActive,
                        IsDeleted = labRoom.IsDeleted
                    }
                };
            } catch (Exception ex)
            {
                return new ResultMessage<LabRoomDto>
                {
                    Success = false,
                    Message = $"An error occurred while creating the lab room: {ex.Message}"
                };
            }
        }
    }
}
