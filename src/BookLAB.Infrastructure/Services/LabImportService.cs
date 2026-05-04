using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.LabRooms.Common;
using BookLAB.Application.Features.Users.Common;
using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Infrastructure.Services
{
    public class LabImportService : ILabImportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LabImportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LabImportValidateResponse> ValidateAsync(
            List<LabRoomImportDto> labs,
            int campusId,
            CancellationToken cancellationToken,
            bool isAllowCreateImportData = false)
        {
            var result = new ImportValidationResult<LabRoomImportDto, LabRoom>
            {
                TotalRows = labs.Count
            };

            // ===== 1. PRELOAD (Tối ưu hiệu năng bằng cách load map trước) =====
            var maps = await BuildContext(labs, campusId, cancellationToken);

            // ===== 2. VALIDATE TỪNG DÒNG =====
            for (int i = 0; i < labs.Count; i++)
            {
                var row = new RowResult<LabRoomImportDto, LabRoom>
                {
                    RowNumber = i + 1,
                    Data = labs[i]
                };

                ValidateSingleRow(labs[i], row, maps);
                result.Rows.Add(row);
            }

            // ===== 3. CHUYỂN ĐỔI SANG ENTITIES =====
            if (isAllowCreateImportData && result.CanCommit)
            {
                foreach (var row in result.Rows)
                {
                    row.ConvertedEntity = MapToEntity(row.Data, maps);
                }
            }

            var response = new LabImportValidateResponse
            {
                result = result,
                maps = maps
            };
            return response;
        }

        private async Task<LabImportMaps> BuildContext(List<LabRoomImportDto> labs, int campusId, CancellationToken cancellationToken)
        {
            var roomNos = labs.Select(x => x.RoomNo.Trim().ToUpper()).Distinct().ToList();
            var buildingCodes = labs.Select(x => x.BuildingCode.Trim().ToUpper()).Distinct().ToList();

            // Ánh xạ Building theo Code trong cùng Campus
            var buildingMap = await _unitOfWork.Repository<Building>().Entities
                .Where(b => buildingCodes.Contains(b.BuildingCode.ToUpper()) && b.CampusId == campusId)
                .ToDictionaryAsync(b => b.BuildingCode.ToUpper(), b => b, cancellationToken);

            // Ánh xạ LabRoom hiện có để xử lý Update
            var labRoomMap = await _unitOfWork.Repository<LabRoom>().Entities
                .Where(r => roomNos.Contains(r.RoomNo.ToUpper()) && r.Building.CampusId == campusId)
                .ToDictionaryAsync(r => r.RoomNo.ToUpper(), r => r, cancellationToken);

            return new LabImportMaps
            {
                BuildingMap = buildingMap,
                LabRoomMap = labRoomMap,
                SeenRoomNos = new HashSet<string>()
            };
        }

        private void ValidateSingleRow(LabRoomImportDto dto, RowResult<LabRoomImportDto, LabRoom> row, LabImportMaps maps)
        {
            dto.RoomNo = dto.RoomNo?.Trim().ToUpper();
            dto.BuildingCode = dto.BuildingCode?.Trim().ToUpper();

            // Kiểm tra các ràng buộc cơ bản
            if (string.IsNullOrWhiteSpace(dto.RoomNo))
                AddError(row, "RoomNo", "Mã phòng không được để trống.", ErrorSeverity.Error);

            if (dto.Capacity <= 0)
                AddError(row, "Capacity", "Sức chứa phải lớn hơn 0.", ErrorSeverity.Error);

            // Xác thực mã tòa nhà
            if (string.IsNullOrWhiteSpace(dto.BuildingCode) || !maps.BuildingMap.ContainsKey(dto.BuildingCode))
                AddError(row, "BuildingCode", $"Tòa nhà '{dto.BuildingCode}' không tồn tại.", ErrorSeverity.Error);

            // Kiểm tra trùng lặp và đánh dấu cập nhật
            if (!string.IsNullOrWhiteSpace(dto.RoomNo))
            {
                if (!maps.SeenRoomNos.Add(dto.RoomNo))
                    AddError(row, "RoomNo", "Bị trùng trong tệp tin.", ErrorSeverity.Error);
                else if (maps.LabRoomMap.TryGetValue(dto.RoomNo, out _))
                {
                    AddError(row, "RoomNo", "Phòng đã tồn tại - Thông tin sẽ được cập nhật.", ErrorSeverity.Warning);
                    dto.IsUpdated = true;
                }
            }
        }

        private LabRoom MapToEntity(LabRoomImportDto dto, LabImportMaps maps)
        {
            if (dto.IsUpdated)
            {
                var existing = maps.LabRoomMap[dto.RoomNo];
                existing.RoomName = dto.RoomName;
                existing.Capacity = dto.Capacity;
                existing.HasEquipment = dto.HasEquipment; // Gán trực tiếp bool
                existing.BuildingId = maps.BuildingMap[dto.BuildingCode].Id;
                return existing;
            }

            return new LabRoom
            {
                RoomNo = dto.RoomNo,
                RoomName = dto.RoomName,
                Capacity = dto.Capacity,
                HasEquipment = dto.HasEquipment, // Gán trực tiếp bool
                BuildingId = maps.BuildingMap[dto.BuildingCode].Id,
                IsActive = true
            };
        }

        private void AddError(RowResult<LabRoomImportDto, LabRoom> row, string field, string message, ErrorSeverity severity)
        {
            row.Errors.Add(new RowError { FieldName = field, Message = message, Severity = severity });
        }
    }
}