using BookLAB.Application.Features.LabRooms.Queries.GetLabRooms;
using MediatR;

namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRoomById;

public class GetLabRoomByIdQuery : IRequest<LabRoomDto?>
{
    public int Id { get; set; }
    public bool IncludeImages { get; set; } = true;
    public bool IncludeBuilding { get; set; } = true;
    public bool IncludeLabOwner { get; set; } = true;
}
