using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.LabRooms.Common;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.LabRooms.Commands.ValidateImportLabRooms
{
    public class ValidateLabRoomImportHandler : IRequestHandler<ValidateLabRoomImportQuery, ImportValidationResult<LabRoomImportDto, LabRoom>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ValidateLabRoomImportHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ImportValidationResult<LabRoomImportDto, LabRoom>> Handle(ValidateLabRoomImportQuery request, CancellationToken cancellationToken)
        {
            var response = new ImportValidationResult<LabRoomImportDto, LabRoom>();

            var buildingIds = request.LabRooms.Select(x => x.BuildingId).Distinct().ToList();

            var buildingSet = (await _unitOfWork.Repository<Building>().Entities
                .Where(b => buildingIds.Contains(b.Id))
                .Select(b => b.Id)
                .ToListAsync(cancellationToken))
                .ToHashSet();

            var existingRooms = await _unitOfWork.Repository<LabRoom>().Entities
                .Where(r => buildingIds.Contains(r.BuildingId))
                .Select(r => new { r.BuildingId, r.RoomName, r.RoomNo })
                .ToListAsync(cancellationToken);

            var existingBuildingRoomNameSet = existingRooms
                .Select(x => $"{x.BuildingId}:{x.RoomName.Trim().ToLowerInvariant()}")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var existingRoomNoSet = existingRooms
                .Select(x => x.RoomNo.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var seenBuildingRoomNameSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var seenRoomNoSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            //for (var i = 0; i < request.LabRooms.Count; i++)
            //{
            //    var item = request.LabRooms[i];
            //    var rowResult = new RowResult<LabRoomImportDto, LabRoom>
            //    {
            //        RowNumber = i + 2,
            //        Data = item
            //    };

            //    var normalizedRoomName = (item.RoomName ?? string.Empty).Trim();
            //    var normalizedRoomNo = (item.RoomNo ?? string.Empty).Trim().TrimEnd('.');
            //    var buildingRoomNameKey = $"{item.BuildingId}:{normalizedRoomName.ToLowerInvariant()}";

            //    if (item.BuildingId <= 0 || !buildingSet.Contains(item.BuildingId))
            //    {
            //        rowResult.Messages.Add($"BuildingId '{item.BuildingId}' không tồn tại.");
            //        rowResult.Status = "Invalid";
            //        rowResult.IsCritical = true;
            //    }

            //    if (string.IsNullOrWhiteSpace(normalizedRoomName) || normalizedRoomName.Length > 100)
            //    {
            //        rowResult.Messages.Add("Tên phòng bắt buộc và tối đa 100 ký tự.");
            //        rowResult.Status = "Invalid";
            //        rowResult.IsCritical = true;
            //    }

            //    if (string.IsNullOrWhiteSpace(normalizedRoomNo) || normalizedRoomNo.Length > 50)
            //    {
            //        rowResult.Messages.Add("Mã phòng bắt buộc và tối đa 50 ký tự.");
            //        rowResult.Status = "Invalid";
            //        rowResult.IsCritical = true;
            //    }

            //    if (item.Capacity <= 0)
            //    {
            //        rowResult.Messages.Add("Sức chứa phải lớn hơn 0.");
            //        rowResult.Status = "Invalid";
            //        rowResult.IsCritical = true;
            //    }

            //    if (item.OverrideNumber < 0)
            //    {
            //        rowResult.Messages.Add("OverrideNumber không được âm.");
            //        rowResult.Status = "Invalid";
            //        rowResult.IsCritical = true;
            //    }

            //    if (normalizedRoomName.Length > 0)
            //    {
            //        if (!seenBuildingRoomNameSet.Add(buildingRoomNameKey))
            //        {
            //            rowResult.Messages.Add("Tên phòng bị trùng trong cùng tòa nhà ở file import.");
            //            rowResult.Status = "Invalid";
            //            rowResult.IsCritical = true;
            //        }
            //        else if (existingBuildingRoomNameSet.Contains(buildingRoomNameKey))
            //        {
            //            rowResult.Messages.Add("Tên phòng đã tồn tại trong tòa nhà trên hệ thống.");
            //            rowResult.Status = "Invalid";
            //            rowResult.IsCritical = true;
            //        }
            //    }

            //    if (normalizedRoomNo.Length > 0)
            //    {
            //        if (!seenRoomNoSet.Add(normalizedRoomNo))
            //        {
            //            rowResult.Messages.Add("Mã phòng bị trùng trong file import.");
            //            rowResult.Status = "Invalid";
            //            rowResult.IsCritical = true;
            //        }
            //        else if (existingRoomNoSet.Contains(normalizedRoomNo))
            //        {
            //            rowResult.Messages.Add("Mã phòng đã tồn tại trên hệ thống.");
            //            rowResult.Status = "Invalid";
            //            rowResult.IsCritical = true;
            //        }
            //    }

            //    response.Rows.Add(rowResult);
            //}

            return response;
        }
    }
}
