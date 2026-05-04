using BookLAB.Application.Features.LabRooms.Common;

namespace BookLAB.Application.Common.Interfaces.Services
{
    public interface ILabImportService
    {
        Task<LabImportValidateResponse> ValidateAsync(
            List<LabRoomImportDto> labs,
            int campusId,
            CancellationToken cancellationToken,
            bool isAllowCreateImportData = false);  
    }
}
