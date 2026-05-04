using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.LabRooms.Common
{
    public class LabImportValidateResponse
    {
        public LabImportMaps maps { get; set; }
        public ImportValidationResult<LabRoomImportDto, LabRoom> result { get; set; }
    }
}
