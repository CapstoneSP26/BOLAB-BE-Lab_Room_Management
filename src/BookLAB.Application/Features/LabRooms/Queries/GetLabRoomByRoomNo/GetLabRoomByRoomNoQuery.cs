using BookLAB.Application.Features.LabRooms.Queries.GetLabRooms;
using MediatR;

namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRoomByRoomNo
{
    public class GetLabRoomByRoomNoQuery : IRequest<LabRoomDto?>
    {
        public string RoomNo { get; set; } = string.Empty;

    }

}
