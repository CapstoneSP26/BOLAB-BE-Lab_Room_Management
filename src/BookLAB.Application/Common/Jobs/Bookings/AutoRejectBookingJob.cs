using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Jobs.Bookings
{
    public class AutoRejectBookingJob
    {
        private readonly IUnitOfWork _unitOfWork;

        public AutoRejectBookingJob(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Execute()
        {
            var now = DateTimeOffset.UtcNow;

            // Tìm các booking vẫn đang chờ duyệt nhưng thời gian bắt đầu đã trôi qua
            var expiredBookings = await _unitOfWork.Repository<Booking>().Entities
                .Where(b => b.BookingStatus == BookingStatus.PendingApproval && b.StartTime < now)
                .ToListAsync();

            var expiredRequestBookings = await _unitOfWork.Repository<BookingRequest>().Entities
                .Where(b => b.BookingRequestStatus == BookingRequestStatus.Pending && b.Booking.StartTime < now)
                .ToListAsync();

            foreach (var booking in expiredBookings)
            {
                booking.BookingStatus = BookingStatus.Rejected;
                // Có thể thêm note vào Metadata hoặc một trường Reason để giảng viên biết
            }

            foreach (var bookingRequest in expiredRequestBookings)
            {
                bookingRequest.BookingRequestStatus = BookingRequestStatus.Rejected;
                bookingRequest.ResponseContext = "Tự động hủy lịch khi quá hạn";
                // Có thể thêm note vào Metadata hoặc một trường Reason để giảng viên biết
            }
            if (expiredRequestBookings.Any() || expiredBookings.Any())
            {
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);
                Console.WriteLine($"[AutoRejectBooking] tự động reject");
            }
        }
    }
}