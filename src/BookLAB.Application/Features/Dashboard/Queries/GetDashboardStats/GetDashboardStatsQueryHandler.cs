using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Dashboard.Queries.GetDashboardStats
{
    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardStatsResponse> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var thirtyDaysAgo = now.AddDays(-30);
            var twelveMonthsAgo = now.AddMonths(-12);

            // Booking Statistics
            var totalBookings = await _unitOfWork.Repository<Booking>().Entities.CountAsync(cancellationToken);
            var pendingBookings = await _unitOfWork.Repository<Booking>().Entities
                .CountAsync(b => b.BookingStatus == BookingStatus.PendingApproval, cancellationToken);
            var approvedBookings = await _unitOfWork.Repository<Booking>().Entities
                .CountAsync(b => b.BookingStatus == BookingStatus.Approved, cancellationToken);
            var rejectedBookings = await _unitOfWork.Repository<Booking>().Entities
                .CountAsync(b => b.BookingStatus == BookingStatus.Rejected, cancellationToken);

            // Incident Statistics
            var totalIncidents = await _unitOfWork.Repository<Report>().Entities.CountAsync(cancellationToken);
            var unresolvedIncidents = await _unitOfWork.Repository<Report>().Entities
                .CountAsync(r => !r.IsResolved, cancellationToken);

            // Room Statistics
            var totalRooms = await _unitOfWork.Repository<LabRoom>().Entities.CountAsync(cancellationToken);
            
            var nowOffset = new DateTimeOffset(now, TimeSpan.Zero);
            
            var availableRooms = await _unitOfWork.Repository<Schedule>().Entities
                .Where(s => s.StartTime <= nowOffset && s.EndTime >= nowOffset && s.IsActive && !s.IsDeleted)
                .Select(s => s.LabRoomId)
                .Distinct()
                .CountAsync(cancellationToken);
            availableRooms = totalRooms - availableRooms;

            // User Statistics
            var totalStudents = await _unitOfWork.Repository<User>().Entities
                .CountAsync(u => !u.UserRoles.Any(ur => ur.Role.RoleName == "Admin" || ur.Role.RoleName == "Manager" || ur.Role.RoleName == "Lecturer"), cancellationToken);
            var totalLecturers = await _unitOfWork.Repository<User>().Entities
                .CountAsync(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Lecturer"), cancellationToken);

            // Booking Duration Average
            var bookingsForAvg = await _unitOfWork.Repository<Booking>().Entities
                .Where(b => b.CreatedAt >= new DateTimeOffset(thirtyDaysAgo, TimeSpan.Zero))
                .Select(b => new { b.StartTime, b.EndTime })
                .ToListAsync(cancellationToken);
            
            var avgDuration = bookingsForAvg.Any() 
                ? bookingsForAvg.Average(b => (b.EndTime - b.StartTime).TotalHours)
                : 0d;

            // Most Booked Room
            var mostBookedRoom = await _unitOfWork.Repository<Booking>().Entities
                .Where(b => b.CreatedAt >= new DateTimeOffset(thirtyDaysAgo, TimeSpan.Zero))
                .Include(b => b.LabRoom)
                .ThenInclude(lr => lr.Building)
                .GroupBy(b => new { b.LabRoom.Id, b.LabRoom.RoomName, Building = b.LabRoom.Building.BuildingName })
                .OrderByDescending(g => g.Count())
                .Select(g => $"{g.Key.Building} - {g.Key.RoomName}")
                .FirstOrDefaultAsync(cancellationToken) ?? "N/A";

            // Busiest Hour
            var busiestHour = bookingsForAvg
                .GroupBy(b => b.StartTime.DateTime.Hour)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            // Monthly Bookings (last 12 months) - calculate from the start of 11 months ago
            // Get the first day of current month
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            // Get the first day of 11 months ago (includes current month as 12th month)
            var twelveMonthsAgoStart = currentMonthStart.AddMonths(-11);
            
            var monthlyBookingsData = await GetMonthlyBookings(twelveMonthsAgoStart, now, cancellationToken);
            var monthlyIncidentsData = await GetMonthlyIncidents(twelveMonthsAgoStart, now, cancellationToken);

            return new DashboardStatsResponse
            {
                TotalBookings = totalBookings,
                PendingBookings = pendingBookings,
                ApprovedBookings = approvedBookings,
                RejectedBookings = rejectedBookings,
                TotalIncidents = totalIncidents,
                UnresolvedIncidents = unresolvedIncidents,
                TotalRooms = totalRooms,
                AvailableRooms = availableRooms,
                TotalStudents = totalStudents,
                TotalLecturers = totalLecturers,
                AverageBookingDuration = (decimal)avgDuration,
                MostBookedRoom = mostBookedRoom,
                BusiestHourOfDay = busiestHour,
                MonthlyBookings = monthlyBookingsData,
                MonthlyIncidents = monthlyIncidentsData
            };
        }

        private async Task<List<int>> GetMonthlyBookings(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            // Fetch all bookings data
            var bookings = await _unitOfWork.Repository<Booking>().Entities
                .AsNoTracking()
                .Select(b => new { Year = b.CreatedAt.Year, Month = b.CreatedAt.Month })
                .ToListAsync(cancellationToken);

            var monthlyData = new List<int>();
            // Iterate exactly 12 times to get the last 12 months including current month
            var currentDate = startDate;

            for (int i = 0; i < 12; i++)
            {
                var year = currentDate.Year;
                var month = currentDate.Month;

                var count = bookings.Count(b => b.Year == year && b.Month == month);
                monthlyData.Add(count);
                
                currentDate = currentDate.AddMonths(1);
            }

            return monthlyData;
        }

        private async Task<List<int>> GetMonthlyIncidents(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            // Fetch all incidents data without complex date filtering to avoid PostgreSQL type issues
            var incidents = await _unitOfWork.Repository<Report>().Entities
                .AsNoTracking()
                .Select(r => new { Year = r.CreatedAt.Year, Month = r.CreatedAt.Month })
                .ToListAsync(cancellationToken);

            var monthlyData = new List<int>();
            var currentDate = startDate;

            for (int i = 0; i < 12; i++)
            {
                var year = currentDate.Year;
                var month = currentDate.Month;

                var count = incidents.Count(r => r.Year == year && r.Month == month);
                monthlyData.Add(count);
                
                currentDate = currentDate.AddMonths(1);
            }

            return monthlyData;
        }
    }
}
