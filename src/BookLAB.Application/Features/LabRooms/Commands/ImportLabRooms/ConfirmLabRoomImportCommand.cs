using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.LabRooms.Common;
using MediatR;

namespace BookLAB.Application.Features.LabRooms.Commands.ImportLabRooms
{
    public class ConfirmLabRoomImportCommand : IRequest<ImportResult>
    {
        public List<LabRoomImportDto> LabRooms { get; set; } = new();
        public int CampusId { get; set; }
    }
}
