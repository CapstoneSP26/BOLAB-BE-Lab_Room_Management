using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;

namespace BookLAB.Application.Features.LabRooms.Commands.ImportLabRooms
{
    public class ConfirmLabRoomImportHandler : IRequestHandler<ConfirmLabRoomImportCommand, ImportResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILabImportService _labImportService;

        public ConfirmLabRoomImportHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILabImportService labImportService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _labImportService = labImportService;
        }

        public async Task<ImportResult> Handle(ConfirmLabRoomImportCommand request, CancellationToken cancellationToken)
        {
            var response = await _labImportService.ValidateAsync(request.LabRooms, request.CampusId, cancellationToken, true);
            var result = response.result;

            if (!result.CanCommit)
            {
                return new ImportResult { Success = false };
            }

            var countUpdated = result.Rows.Count(r => r.Data.IsUpdated);
            var countNew = result.Rows.Count(r => !r.Data.IsUpdated);
            var now = DateTimeOffset.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newLabEntries = new List<(LabRoom Entity, List<RoomPolicy> Policies)>();

                foreach (var row in result.Rows)
                {
                    var entity = row.ConvertedEntity;

                    if (row.Data.IsUpdated)
                    {
                        entity.UpdatedAt = now;
                        entity.UpdatedBy = _currentUserService.UserId;
                        _unitOfWork.Repository<LabRoom>().Update(entity);
                    }
                    else
                    {
                        entity.CreatedAt = now;
                        entity.CreatedBy = _currentUserService.UserId;

                        // Khởi tạo danh sách Policy mặc định cho phòng mới
                        var policies = new List<RoomPolicy>
                        {
                            CreateDefaultPolicy(PolicyType.MaxBookingAdvance, "14", now),
                            CreateDefaultPolicy(PolicyType.MinBookingLeadTime, "5", now),
                            CreateDefaultPolicy(PolicyType.CurfewTime, "23:59", now),
                            CreateDefaultPolicy(PolicyType.MaxConcurrentBookings, "1", now)
                        };

                        newLabEntries.Add((entity, policies));
                    }
                }

                // GIAI ĐOẠN 1: Lưu LabRoom mới để Database sinh Id tự tăng
                if (newLabEntries.Any())
                {
                    var labsToAdd = newLabEntries.Select(x => x.Entity).ToList();
                    await _unitOfWork.Repository<LabRoom>().AddRangeAsync(labsToAdd);

                    // SaveChanges lần 1 để lấy Id gán vào Policy
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // GIAI ĐOẠN 2: Gán LabRoomId cho các Policy và lưu
                    var allPoliciesToAdd = new List<RoomPolicy>();
                    foreach (var entry in newLabEntries)
                    {
                        foreach (var policy in entry.Policies)
                        {
                            policy.LabRoomId = entry.Entity.Id; // Lúc này Id đã tồn tại
                            allPoliciesToAdd.Add(policy);
                        }
                    }

                    if (allPoliciesToAdd.Any())
                    {
                        await _unitOfWork.Repository<RoomPolicy>().AddRangeAsync(allPoliciesToAdd);
                    }
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return new ImportResult
                {
                    Success = true,
                    CreatedCount = countNew,
                    UpdatedCount = countUpdated,
                    TotalProcessed = result.Rows.Count,
                };
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        private RoomPolicy CreateDefaultPolicy(PolicyType type, string value, DateTimeOffset now)
        {
            return new RoomPolicy
            {
                PolicyKey = type,
                PolicyValue = value,
                CreatedAt = now,
                CreatedBy = _currentUserService.UserId,
                IsActive = true
            };
        }
    }
}