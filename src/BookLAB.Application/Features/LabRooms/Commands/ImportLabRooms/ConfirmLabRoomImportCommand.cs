using BookLAB.Application.Features.LabRooms.Common;
using MediatR;

namespace BookLAB.Application.Features.LabRooms.Commands.ImportLabRooms
{
    public class ConfirmLabRoomImportCommand : IRequest<bool>
    {
        public List<LabRoomImportDto> ValidLabRooms { get; set; } = new();
    }
}
