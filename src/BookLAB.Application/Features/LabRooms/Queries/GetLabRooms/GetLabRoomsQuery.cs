using BookLAB.Application.Common.Models;
using MediatR;

namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRooms
{
    public class GetLabRoomsQuery : IRequest<PagedList<LabRoomDto>>
    {
        public int? BuildingId { get; set; }
        public string? RoomNo { get; set; }
        public string? SearchTerm { get; set; }
        public bool IncludeImages { get; set; } = false;
        public bool IncludeBuilding { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 0;
    }
}