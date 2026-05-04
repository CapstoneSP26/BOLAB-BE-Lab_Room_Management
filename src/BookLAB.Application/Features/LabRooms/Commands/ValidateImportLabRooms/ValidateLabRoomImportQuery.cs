using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.LabRooms.Common;
using BookLAB.Domain.Entities;
using MediatR;

namespace BookLAB.Application.Features.LabRooms.Commands.ValidateImportLabRooms
{
    public class ValidateLabRoomImportQuery : IRequest<ImportValidationResult<LabRoomImportDto, LabRoom>>
    {
        public List<LabRoomImportDto> LabRooms { get; set; } = new();
        public int CampusId { get; set; }
    }
}
