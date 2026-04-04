using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.DTOs;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Dashboard.Queries.GetDashboardStats
{
    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsResponseDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<DashboardStatsResponseDTO> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;
            var targetYear = now.Year;

            var emptyResponse = CreateEmptyResponse(targetYear);

            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return emptyResponse;
            }

            var ownedRoomIds = await _unitOfWork.Repository<LabOwner>().Entities
                .Where(x => x.UserId == userId.Value)
                .Select(x => x.LabRoomId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var roomsQuery = _unitOfWork.Repository<LabRoom>().Entities
                .Where(x => !x.IsDeleted && x.IsActive);

            if (ownedRoomIds.Count > 0)
            {
                roomsQuery = roomsQuery.Where(x => ownedRoomIds.Contains(x.Id));
            }
            else if (_currentUserService.CampusId > 0)
            {
                roomsQuery = roomsQuery.Where(x => x.Building.CampusId == _currentUserService.CampusId);
            }

            var scopedRoomIds = await roomsQuery
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var totalRooms = scopedRoomIds.Count;
            if (totalRooms == 0)
            {
                return emptyResponse;
            }

            var occupiedRooms = await _unitOfWork.Repository<Schedule>().Entities
                .Where(x => !x.IsDeleted
                            && x.IsActive
                            && scopedRoomIds.Contains(x.LabRoomId)
                            && x.StartTime <= now
                            && x.EndTime >= now)
                .Select(x => x.LabRoomId)
                .Distinct()
                .CountAsync(cancellationToken);

            var pendingBookings = await _unitOfWork.Repository<BookingRequest>().Entities
                .Where(x => x.BookingRequestStatus == BookingRequestStatus.Pending
                            && scopedRoomIds.Contains(x.Booking.LabRoomId))
                .CountAsync(cancellationToken);

            var approvedBookings = await _unitOfWork.Repository<BookingRequest>().Entities
                .Where(x => x.BookingRequestStatus == BookingRequestStatus.Approved
                            && scopedRoomIds.Contains(x.Booking.LabRoomId))
                .CountAsync(cancellationToken);

            var unresolvedIncidents = await _unitOfWork.Repository<Report>().Entities
                .Where(x => !x.IsResolved && scopedRoomIds.Contains(x.Schedule.LabRoomId))
                .CountAsync(cancellationToken);

            var monthlyBookingsRaw = await _unitOfWork.Repository<BookingRequest>().Entities
                .Where(x => scopedRoomIds.Contains(x.Booking.LabRoomId)
                            && x.CreatedAt.Year == targetYear)
                .GroupBy(x => x.CreatedAt.Month)
                .Select(g => new MonthlyAggregate { Month = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var monthlyIncidentsRaw = await _unitOfWork.Repository<Report>().Entities
                .Where(x => scopedRoomIds.Contains(x.Schedule.LabRoomId)
                            && x.CreatedAt.Year == targetYear)
                .GroupBy(x => x.CreatedAt.Month)
                .Select(g => new MonthlyAggregate { Month = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var monthlyApprovedBookingsRaw = await _unitOfWork.Repository<BookingRequest>().Entities
                .Where(x => scopedRoomIds.Contains(x.Booking.LabRoomId)
                            && x.BookingRequestStatus == BookingRequestStatus.Approved
                            && x.CreatedAt.Year == targetYear)
                .GroupBy(x => x.CreatedAt.Month)
                .Select(g => new MonthlyAggregate { Month = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var monthlyPendingBookingsRaw = await _unitOfWork.Repository<BookingRequest>().Entities
                .Where(x => scopedRoomIds.Contains(x.Booking.LabRoomId)
                            && x.BookingRequestStatus == BookingRequestStatus.Pending
                            && x.CreatedAt.Year == targetYear)
                .GroupBy(x => x.CreatedAt.Month)
                .Select(g => new MonthlyAggregate { Month = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            return new DashboardStatsResponseDTO
            {
                pendingBookings = pendingBookings,
                approvedBookings = approvedBookings,
                unresolvedIncidents = unresolvedIncidents,
                totalRooms = totalRooms,
                availableRooms = Math.Max(totalRooms - occupiedRooms, 0),
                year = targetYear,
                monthlyBookings = BuildMonthlySeries(targetYear, monthlyBookingsRaw),
                statistics = new DashboardStatisticsSeriesDTO
                {
                    monthlyIncidents = BuildMonthlySeries(targetYear, monthlyIncidentsRaw),
                    monthlyApprovedBookings = BuildMonthlySeries(targetYear, monthlyApprovedBookingsRaw),
                    monthlyPendingBookings = BuildMonthlySeries(targetYear, monthlyPendingBookingsRaw)
                }
            };
        }

        private static DashboardStatsResponseDTO CreateEmptyResponse(int year)
        {
            var emptyRaw = new List<MonthlyAggregate>();
            return new DashboardStatsResponseDTO
            {
                year = year,
                monthlyBookings = BuildMonthlySeries(year, emptyRaw),
                statistics = new DashboardStatisticsSeriesDTO
                {
                    monthlyIncidents = BuildMonthlySeries(year, emptyRaw),
                    monthlyApprovedBookings = BuildMonthlySeries(year, emptyRaw),
                    monthlyPendingBookings = BuildMonthlySeries(year, emptyRaw)
                }
            };
        }

        private static List<MonthlyDataPointDTO> BuildMonthlySeries(int year, IEnumerable<MonthlyAggregate> raw)
        {
            var monthlyMap = raw.ToDictionary(x => x.Month, x => x.Count);

            var result = new List<MonthlyDataPointDTO>(12);
            for (var month = 1; month <= 12; month++)
            {
                result.Add(new MonthlyDataPointDTO
                {
                    month = month,
                    label = new DateTime(year, month, 1).ToString("MMMM"),
                    value = monthlyMap.TryGetValue(month, out var count) ? count : 0
                });
            }

            return result;
        }

        private sealed class MonthlyAggregate
        {
            public int Month { get; set; }
            public int Count { get; set; }
        }
    }
}
