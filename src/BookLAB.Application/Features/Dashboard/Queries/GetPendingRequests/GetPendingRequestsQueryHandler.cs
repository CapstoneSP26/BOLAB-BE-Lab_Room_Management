using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Dashboard.Queries.GetPendingRequests
{
    public class GetPendingRequestsQueryHandler : IRequestHandler<GetPendingRequestsQuery, List<PendingRequestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPendingRequestsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PendingRequestDto>> Handle(GetPendingRequestsQuery request, CancellationToken cancellationToken)
        {
            var limit = request.Limit ?? 10;

            var pendingRequests = await _unitOfWork.Repository<Booking>().Entities
                .Include(b => b.LabRoom)
                .ThenInclude(lr => lr.Building)
                .Where(b => b.BookingStatus == BookingStatus.PendingApproval)
                .OrderByDescending(b => b.CreatedAt)
                .Take(limit)
                .ToListAsync(cancellationToken);

            var createdByIds = pendingRequests.Select(p => p.CreatedBy).Where(id => id.HasValue).Select(id => id.Value).Distinct();
            var users = await _unitOfWork.Repository<User>().Entities
                .Where(u => createdByIds.Contains(u.Id))
                .ToListAsync(cancellationToken);

            var result = pendingRequests.Select(b =>
            {
                var user = users.FirstOrDefault(u => u.Id == b.CreatedBy);
                return new PendingRequestDto
                {
                    BookingId = b.Id,
                    LabRoomName = $"{b.LabRoom.Building.BuildingName} - {b.LabRoom.RoomName}",
                    BuildingName = b.LabRoom.Building.BuildingName,
                    RequesterName = user?.FullName ?? "Unknown",
                    RequesterEmail = user?.Email ?? "Unknown",
                    StartTime = b.StartTime.DateTime,
                    EndTime = b.EndTime.DateTime,
                    ExpectedStudents = b.StudentCount,
                    Purpose = b.Reason,
                    RequestedAt = b.CreatedAt.DateTime
                };
            }).ToList();

            return result;
        }
    }
}
