using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Features.LabRooms.Common;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.LabRooms.Commands.ImportLabRooms
{
    public class ConfirmLabRoomImportHandler : IRequestHandler<ConfirmLabRoomImportCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ConfirmLabRoomImportHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(ConfirmLabRoomImportCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)
            {
                throw new UnauthorizedAccessException("Không xác định được người dùng thực hiện import.");
            }

            var buildingIds = request.ValidLabRooms.Select(x => x.BuildingId).Distinct().ToList();

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
            var labRoomsToInsert = new List<LabRoom>();

            foreach (var item in request.ValidLabRooms)
            {
                var normalizedRoomName = (item.RoomName ?? string.Empty).Trim();
                var normalizedRoomNo = (item.RoomNo ?? string.Empty).Trim().TrimEnd('.');
                var buildingRoomNameKey = $"{item.BuildingId}:{normalizedRoomName.ToLowerInvariant()}";

                var isInvalid =
                    item.BuildingId <= 0 || !buildingSet.Contains(item.BuildingId) ||
                    string.IsNullOrWhiteSpace(normalizedRoomName) || normalizedRoomName.Length > 100 ||
                    string.IsNullOrWhiteSpace(normalizedRoomNo) || normalizedRoomNo.Length > 50 ||
                    item.Capacity <= 0 ||
                    item.OverrideNumber < 0 ||
                    !seenBuildingRoomNameSet.Add(buildingRoomNameKey) || existingBuildingRoomNameSet.Contains(buildingRoomNameKey) ||
                    !seenRoomNoSet.Add(normalizedRoomNo) || existingRoomNoSet.Contains(normalizedRoomNo);

                if (isInvalid)
                {
                    throw new Exception($"Dữ liệu import phòng không hợp lệ hoặc đã thay đổi ở phòng '{item.RoomNo}'.");
                }

                labRoomsToInsert.Add(new LabRoom
                {
                    BuildingId = item.BuildingId,
                    RoomName = normalizedRoomName,
                    RoomNo = normalizedRoomNo,
                    Location = string.IsNullOrWhiteSpace(item.Location) ? null : item.Location.Trim(),
                    Capacity = item.Capacity,
                    HasEquipment = item.HasEquipment,
                    OverrideNumber = item.OverrideNumber,
                    Description = string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.Trim(),
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = _currentUserService.UserId.Value
                });

                existingBuildingRoomNameSet.Add(buildingRoomNameKey);
                existingRoomNoSet.Add(normalizedRoomNo);
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Repository<LabRoom>().AddRangeAsync(labRoomsToInsert);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            return true;
        }
    }
}
