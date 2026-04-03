using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.LabRooms.Common;
using MediatR;

namespace BookLAB.Application.Features.LabRooms.Commands.ValidateImportLabRooms
{
    public class ValidateLabRoomImportQuery : IRequest<ImportValidationResult<LabRoomImportDto>>
    {
        public List<LabRoomImportDto> LabRooms { get; set; } = new();
    }
}
