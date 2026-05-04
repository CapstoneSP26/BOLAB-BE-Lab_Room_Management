using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.LabRooms.Commands.UpdateLabRoom
{
    public class UpdateLabRoomHandler : IRequestHandler<UpdateLabRoomCommand, ResultMessage<LabRoomDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public UpdateLabRoomHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ResultMessage<LabRoomDto>> Handle(UpdateLabRoomCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id == 0)
                    return new ResultMessage<LabRoomDto>
                    {
                        Success = false,
                        Message = "Lab Room Id must not be empty"
                    };

                var labRoom = await _unitOfWork.Repository<LabRoom>().GetByIdAsync(request.Id);

                if (labRoom == null)
                    return new ResultMessage<LabRoomDto>
                    {
                        Success = false,
                        Message = "Lab Room is not existed"
                    };

                labRoom.BuildingId = request.BuildingId == 0 ? request.BuildingId : labRoom.BuildingId;
                labRoom.RoomName = request.RoomName == "" ? request.RoomName : labRoom.RoomName;
                labRoom.RoomNo = request.RoomNo == "" ? request.RoomNo : labRoom.RoomNo;
                labRoom.Location = request.Location == "" ? request.Location : labRoom.Location;
                labRoom.OverrideNumber = request.OverrideNumber;
                labRoom.HasEquipment = request.HasEquipment;
                labRoom.Capacity = request.Capacity == 0 ? request.Capacity : labRoom.Capacity;
                labRoom.Description = request.Description == "" ? request.Description : labRoom.Description;
                labRoom.UpdatedBy = _currentUserService.UserId;
                labRoom.UpdatedAt = DateTimeOffset.UtcNow;
                labRoom.IsActive = request.IsActive;

                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Repository<LabRoom>().UpdateAsync(labRoom);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                if (request.LabOwnerId != null)
                {
                    await _unitOfWork.BeginTransactionAsync();
                    var labOwners = await _unitOfWork.Repository<LabOwner>().Entities.Where(x => x.LabRoomId == labRoom.Id).ToListAsync();
                    _unitOfWork.Repository<LabOwner>().DeleteRange(labOwners);
                    await _unitOfWork.Repository<LabOwner>().AddAsync(new LabOwner { LabRoomId = labRoom.Id, UserId = request.LabOwnerId.Value });
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    await _unitOfWork.CommitTransactionAsync();
                }

                return new ResultMessage<LabRoomDto>
                {
                    Success = true,
                    Message = "Update Lab Room Successfully",
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
                        UpdatedAt = labRoom.UpdatedAt,
                        CreatedBy = labRoom.CreatedBy,
                        UpdatedBy = labRoom.UpdatedBy,
                        IsActive = labRoom.IsActive,
                        IsDeleted = labRoom.IsDeleted
                    }
                };
            } catch (Exception ex)
            {
                return new ResultMessage<LabRoomDto>
                {
                    Success = false,
                    Message = $"Some thing is wrong while update lab room with id = {request.Id}"
                };
            }
        }
    }
}
