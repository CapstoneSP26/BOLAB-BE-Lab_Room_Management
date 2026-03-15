using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.LabRooms.Queries
{
    public class GetLabRoomsInBuildingCommand : IRequest<List<LabRoomRequest>>
    {
        public string buildingId { get; set; }
    }
}
