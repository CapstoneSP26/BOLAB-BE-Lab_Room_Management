using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.LabRooms.Common;
using BookLAB.Domain.Entities;
using MediatR;

namespace BookLAB.Application.Features.LabRooms.Commands.ValidateImportLabRooms
{
    public class ValidateLabRoomImportHandler : IRequestHandler<ValidateLabRoomImportQuery, ImportValidationResult<LabRoomImportDto, LabRoom>>
    {
        private readonly ILabImportService _labImportService;

        public ValidateLabRoomImportHandler(ILabImportService labImportService)
        {
            _labImportService = labImportService;
        }

        public async Task<ImportValidationResult<LabRoomImportDto, LabRoom>> Handle(ValidateLabRoomImportQuery request, CancellationToken cancellationToken)
        {
            var response = await _labImportService.ValidateAsync(request.LabRooms, request.CampusId, cancellationToken);
            return response.result;
        }
    }
}
