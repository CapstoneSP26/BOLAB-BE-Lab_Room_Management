using BookLAB.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.LabRooms.Commands.UpdateLabRoom
{
    public class UpdateLabRoomCommand : IRequest<ResultMessage<LabRoomDto>>
    {
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public string RoomName { get; set; } = null!;
        public string RoomNo { get; set; } = null!;
        public string? Location { get; set; }
        public int OverrideNumber { get; set; } = 0;
        public bool HasEquipment { get; set; }
        public int Capacity { get; set; }

        public string? Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
    }
}
