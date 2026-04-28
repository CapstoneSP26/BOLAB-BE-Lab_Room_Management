using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.DTOs;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Dashboard.Queries.GetDashboardOverview
{
    public class GetDashboardOverviewQueryHandler : IRequestHandler<GetDashboardOverviewQuery, DashboardOverviewDto>
    {
        private const string AdminRoleValue = "1";
        private const string LabManagerRoleValue = "2";
        private const int LecturerRoleId = 3;
        private const int StudentRoleId = 4;

        private readonly IUnitOfWork _unitOfWork;

        public GetDashboardOverviewQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardOverviewDto> Handle(GetDashboardOverviewQuery request, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;
            var todayStart = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero);
            var tomorrowStart = todayStart.AddDays(1);
            var monthStart = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
            var nextMonthStart = monthStart.AddMonths(1);
            var year = now.Year;

            var isAdmin = request.Role == AdminRoleValue;
            var isLabManager = request.Role == LabManagerRoleValue;
            var scope = isAdmin ? "ADMIN" : "LAB_MANAGER";

            var roomsQuery = _unitOfWork.Repository<LabRoom>().Entities
                .Where(x => !x.IsDeleted && x.IsActive);

            if (isLabManager)
            {
                if (!request.UserId.HasValue)
                {
                    return CreateEmptyResponse(scope, now);
                }

                var ownedRoomIds = await _unitOfWork.Repository<LabOwner>().Entities
                    .Where(x => x.UserId == request.UserId.Value)
                    .Select(x => x.LabRoomId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                if (ownedRoomIds.Count == 0)
                {
                    return CreateEmptyResponse(scope, now);
                }

                roomsQuery = roomsQuery.Where(x => ownedRoomIds.Contains(x.Id));
            }

            var scopedRooms = await roomsQuery
                .Select(x => new RoomScope
                {
                    Id = x.Id,
                    RoomNo = x.RoomNo,
                    RoomName = x.RoomName,
                    BuildingName = x.Building.BuildingName
                })
                .ToListAsync(cancellationToken);

            var scopedRoomIds = scopedRooms.Select(x => x.Id).ToList();
            var totalRooms = scopedRoomIds.Count;

            if (totalRooms == 0)
            {
                return CreateEmptyResponse(scope, now);
            }

            var bookingRequestsQuery = _unitOfWork.Repository<BookingRequest>().Entities
                .Where(x => scopedRoomIds.Contains(x.Booking.LabRoomId));

            var totalBookings = await bookingRequestsQuery.CountAsync(cancellationToken);
            var pendingBookings = await bookingRequestsQuery.CountAsync(x => x.BookingRequestStatus == BookingRequestStatus.Pending, cancellationToken);
            var approvedBookings = await bookingRequestsQuery.CountAsync(x => x.BookingRequestStatus == BookingRequestStatus.Approved, cancellationToken);
            var rejectedBookings = await bookingRequestsQuery.CountAsync(x => x.BookingRequestStatus == BookingRequestStatus.Rejected, cancellationToken);

            var approvedBookingsToday = await bookingRequestsQuery
                .Where(x => x.BookingRequestStatus == BookingRequestStatus.Approved)
                .CountAsync(x => (x.UpdatedAt ?? x.CreatedAt) >= todayStart && (x.UpdatedAt ?? x.CreatedAt) < tomorrowStart, cancellationToken);

            var approvedTodayBookingSchedules = await bookingRequestsQuery
                .Where(x => x.BookingRequestStatus == BookingRequestStatus.Approved)
                .Where(x => (x.UpdatedAt ?? x.CreatedAt) >= todayStart && (x.UpdatedAt ?? x.CreatedAt) < tomorrowStart)
                .Select(x => new
                {
                    x.BookingId,
                    x.Booking.ScheduleId
                })
                .ToListAsync(cancellationToken);

            var approvedTodayScheduleIds = approvedTodayBookingSchedules
                .Where(x => x.ScheduleId.HasValue)
                .Select(x => x.ScheduleId!.Value)
                .Distinct()
                .ToList();

            var checkedInScheduleIds = approvedTodayScheduleIds.Count == 0
                ? new HashSet<Guid>()
                : (await _unitOfWork.Repository<Attendance>().Entities
                    .Where(x => approvedTodayScheduleIds.Contains(x.ScheduleId)
                                && x.CheckInTime.HasValue
                                && x.CheckInTime.Value >= todayStart
                                && x.CheckInTime.Value < tomorrowStart)
                    .Select(x => x.ScheduleId)
                    .Distinct()
                    .ToListAsync(cancellationToken))
                .ToHashSet();

            var checkedInBookingsToday = approvedTodayBookingSchedules
                .Where(x => x.ScheduleId.HasValue && checkedInScheduleIds.Contains(x.ScheduleId.Value))
                .Select(x => x.BookingId)
                .Distinct()
                .Count();

            var noCheckInBookingsToday = Math.Max(approvedBookingsToday - checkedInBookingsToday, 0);
            var checkInCompliancePercentage = approvedBookingsToday == 0
                ? 0
                : Math.Round((double)checkedInBookingsToday * 100.0 / approvedBookingsToday, 2);

            var approvedDurations = await bookingRequestsQuery
                .Where(x => x.BookingRequestStatus == BookingRequestStatus.Approved)
                .Select(x => new
                {
                    x.Booking.StartTime,
                    x.Booking.EndTime
                })
                .ToListAsync(cancellationToken);

            var averageBookingDuration = approvedDurations.Count == 0
                ? 0
                : Math.Round(approvedDurations.Average(x => (x.EndTime - x.StartTime).TotalHours), 2);

            var mostBookedRoomRaw = await bookingRequestsQuery
                .GroupBy(x => x.Booking.LabRoomId)
                .Select(g => new
                {
                    g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync(cancellationToken);

            var mostBookedRoom = string.Empty;
            if (mostBookedRoomRaw is not null)
            {
                var room = scopedRooms.FirstOrDefault(x => x.Id == mostBookedRoomRaw.Key);
                mostBookedRoom = room is null ? string.Empty : (string.IsNullOrWhiteSpace(room.RoomNo) ? room.RoomName : room.RoomNo);
            }

            var busiestHour = await bookingRequestsQuery
                .Where(x => x.BookingRequestStatus == BookingRequestStatus.Approved)
                .GroupBy(x => x.Booking.StartTime.Hour)
                .Select(g => new { Hour = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Hour)
                .FirstOrDefaultAsync(cancellationToken);

            var busiestHourOfDay = busiestHour?.Hour ?? 0;

            var monthlyBookingsRaw = await bookingRequestsQuery
                .Where(x => x.CreatedAt.Year == year)
                .GroupBy(x => x.CreatedAt.Month)
                .Select(g => new MonthAggregate { Month = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var reportsQuery = _unitOfWork.Repository<Report>().Entities
                .Where(x => scopedRoomIds.Contains(x.Schedule.LabRoomId));

            var totalIncidents = await reportsQuery.CountAsync(cancellationToken);
            var unresolvedIncidents = await reportsQuery.CountAsync(x => !x.IsResolved, cancellationToken);

            var monthlyIncidentsRaw = await reportsQuery
                .Where(x => x.CreatedAt.Year == year)
                .GroupBy(x => x.CreatedAt.Month)
                .Select(g => new MonthAggregate { Month = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var occupiedRoomCount = await _unitOfWork.Repository<Schedule>().Entities
                .Where(x => !x.IsDeleted
                            && x.IsActive
                            && scopedRoomIds.Contains(x.LabRoomId)
                            && x.StartTime <= now
                            && x.EndTime >= now)
                .Select(x => x.LabRoomId)
                .Distinct()
                .CountAsync(cancellationToken);

            var availableRooms = Math.Max(totalRooms - occupiedRoomCount, 0);

            var totalStudents = await _unitOfWork.Repository<UserRole>().Entities
                .Where(x => x.RoleId == StudentRoleId && x.User.IsActive && !x.User.IsDeleted)
                .Select(x => x.UserId)
                .Distinct()
                .CountAsync(cancellationToken);

            var totalLecturers = await _unitOfWork.Repository<UserRole>().Entities
                .Where(x => x.RoleId == LecturerRoleId && x.User.IsActive && !x.User.IsDeleted)
                .Select(x => x.UserId)
                .Distinct()
                .CountAsync(cancellationToken);

            var lecturerBookingRequests = await bookingRequestsQuery
                .Where(x => x.CreatedAt >= todayStart && x.CreatedAt < tomorrowStart)
                .GroupBy(x => x.RequestedByUserId)
                .Select(g => new DashboardLecturerBookingRequestDto
                {
                    lecturerId = g.Key.ToString(),
                    lecturerName = g.Select(x => x.Requester.FullName).FirstOrDefault() ?? string.Empty,
                    lecturerEmail = g.Select(x => x.Requester.Email).FirstOrDefault() ?? string.Empty,
                    bookingRequestCount = g.Count(),
                    approvedBookingCount = g.Count(x => x.BookingRequestStatus == BookingRequestStatus.Approved),
                    roomCount = g.Select(x => x.Booking.LabRoomId).Distinct().Count(),
                    latestRequestAt = g.Max(x => x.CreatedAt)
                })
                .OrderByDescending(x => x.bookingRequestCount)
                .ThenByDescending(x => x.latestRequestAt)
                .ToListAsync(cancellationToken);

            var totalIncidentReportsInSchedules = await reportsQuery.CountAsync(cancellationToken);
            var totalSchedulesWithIncidentReports = await reportsQuery
                .Select(x => x.ScheduleId)
                .Distinct()
                .CountAsync(cancellationToken);

            var scheduleUsageByRoom = await _unitOfWork.Repository<Schedule>().Entities
                .Where(x => !x.IsDeleted
                            && x.IsActive
                            && scopedRoomIds.Contains(x.LabRoomId)
                            && x.StartTime >= monthStart
                            && x.StartTime < nextMonthStart)
                .GroupBy(x => x.LabRoomId)
                .Select(g => new RoomScheduleUsage
                {
                    LabRoomId = g.Key,
                    TotalSchedules = g.Count(),
                    UsedSchedules = g.Count(x => x.ScheduleStatus != ScheduleStatus.Cancelled)
                })
                .ToListAsync(cancellationToken);

            var bookingByRoomThisMonth = await bookingRequestsQuery
                .Where(x => x.CreatedAt >= monthStart && x.CreatedAt < nextMonthStart)
                .GroupBy(x => x.Booking.LabRoomId)
                .Select(g => new RoomBookingAggregate
                {
                    LabRoomId = g.Key,
                    BookingCount = g.Count(),
                    ApprovedBookingCount = g.Count(x => x.BookingRequestStatus == BookingRequestStatus.Approved)
                })
                .ToListAsync(cancellationToken);

            var incidentsByRoomThisMonth = await reportsQuery
                .Where(x => x.CreatedAt >= monthStart && x.CreatedAt < nextMonthStart)
                .GroupBy(x => x.Schedule.LabRoomId)
                .Select(g => new RoomIncidentAggregate
                {
                    LabRoomId = g.Key,
                    IncidentCount = g.Count()
                })
                .ToListAsync(cancellationToken);

            var roomBookingMap = bookingByRoomThisMonth.ToDictionary(x => x.LabRoomId, x => x);
            var roomIncidentMap = incidentsByRoomThisMonth.ToDictionary(x => x.LabRoomId, x => x.IncidentCount);
            var roomScheduleMap = scheduleUsageByRoom.ToDictionary(x => x.LabRoomId, x => x);

            var roomBookingStats = scopedRooms
                .Select(room =>
                {
                    roomBookingMap.TryGetValue(room.Id, out var bookingStat);
                    roomIncidentMap.TryGetValue(room.Id, out var incidentCount);
                    roomScheduleMap.TryGetValue(room.Id, out var scheduleUsage);

                    var roomUtilizationRate = scheduleUsage is null || scheduleUsage.TotalSchedules == 0
                        ? 0
                        : Math.Round((double)scheduleUsage.UsedSchedules * 100.0 / scheduleUsage.TotalSchedules, 2);

                    return new DashboardRoomBookingStatDto
                    {
                        roomId = room.Id.ToString(),
                        roomName = string.IsNullOrWhiteSpace(room.RoomNo) ? room.RoomName : room.RoomNo,
                        buildingName = room.BuildingName,
                        bookingCount = bookingStat?.BookingCount ?? 0,
                        approvedBookingCount = bookingStat?.ApprovedBookingCount ?? 0,
                        incidentReportCount = incidentCount,
                        utilizationRate = roomUtilizationRate
                    };
                })
                .OrderByDescending(x => x.bookingCount)
                .ThenBy(x => x.roomName)
                .ToList();

            var usedRooms = scheduleUsageByRoom.Count(x => x.UsedSchedules > 0);
            var totalSchedules = scheduleUsageByRoom.Sum(x => x.TotalSchedules);
            var usedSchedules = scheduleUsageByRoom.Sum(x => x.UsedSchedules);
            var utilizationRate = totalRooms == 0
                ? 0
                : Math.Round((double)usedRooms * 100.0 / totalRooms, 2);

            return new DashboardOverviewDto
            {
                totalBookings = totalBookings,
                pendingBookings = pendingBookings,
                approvedBookings = approvedBookings,
                rejectedBookings = rejectedBookings,
                totalIncidents = totalIncidents,
                unresolvedIncidents = unresolvedIncidents,
                totalRooms = totalRooms,
                availableRooms = availableRooms,
                totalStudents = totalStudents,
                totalLecturers = totalLecturers,
                averageBookingDuration = averageBookingDuration,
                mostBookedRoom = mostBookedRoom,
                busiestHourOfDay = busiestHourOfDay,
                monthlyBookings = BuildMonthlyArray(monthlyBookingsRaw),
                monthlyIncidents = BuildMonthlyArray(monthlyIncidentsRaw),
                approvedBookingsToday = approvedBookingsToday,
                checkedInBookingsToday = checkedInBookingsToday,
                noCheckInBookingsToday = noCheckInBookingsToday,
                checkInCompliancePercentage = checkInCompliancePercentage,
                approvedBookingRequestsToday = approvedBookingsToday,
                lecturerBookingRequests = lecturerBookingRequests,
                incidentReportsInSchedules = new DashboardIncidentReportsInSchedulesDto
                {
                    totalIncidentReports = totalIncidentReportsInSchedules,
                    totalSchedulesWithIncidentReports = totalSchedulesWithIncidentReports
                },
                labUtilization = new DashboardLabUtilizationDto
                {
                    utilizationRate = utilizationRate,
                    usedRooms = usedRooms,
                    totalRooms = totalRooms,
                    usedSchedules = usedSchedules,
                    totalSchedules = totalSchedules
                },
                roomBookingStats = roomBookingStats,
                scope = scope,
                generatedAt = now
            };
        }

        private static DashboardOverviewDto CreateEmptyResponse(string scope, DateTimeOffset generatedAt)
        {
            return new DashboardOverviewDto
            {
                monthlyBookings = Enumerable.Repeat(0, 12).ToList(),
                monthlyIncidents = Enumerable.Repeat(0, 12).ToList(),
                incidentReportsInSchedules = new DashboardIncidentReportsInSchedulesDto(),
                labUtilization = new DashboardLabUtilizationDto(),
                roomBookingStats = new List<DashboardRoomBookingStatDto>(),
                lecturerBookingRequests = new List<DashboardLecturerBookingRequestDto>(),
                scope = scope,
                generatedAt = generatedAt
            };
        }

        private static List<int> BuildMonthlyArray(IEnumerable<MonthAggregate> raw)
        {
            var map = raw.ToDictionary(x => x.Month, x => x.Count);
            var result = new List<int>(12);
            for (var month = 1; month <= 12; month++)
            {
                result.Add(map.TryGetValue(month, out var count) ? count : 0);
            }

            return result;
        }

        private sealed class RoomScope
        {
            public int Id { get; set; }
            public string RoomNo { get; set; } = string.Empty;
            public string RoomName { get; set; } = string.Empty;
            public string BuildingName { get; set; } = string.Empty;
        }

        private sealed class MonthAggregate
        {
            public int Month { get; set; }
            public int Count { get; set; }
        }

        private sealed class RoomScheduleUsage
        {
            public int LabRoomId { get; set; }
            public int TotalSchedules { get; set; }
            public int UsedSchedules { get; set; }
        }

        private sealed class RoomBookingAggregate
        {
            public int LabRoomId { get; set; }
            public int BookingCount { get; set; }
            public int ApprovedBookingCount { get; set; }
        }

        private sealed class RoomIncidentAggregate
        {
            public int LabRoomId { get; set; }
            public int IncidentCount { get; set; }
        }
    }
}
