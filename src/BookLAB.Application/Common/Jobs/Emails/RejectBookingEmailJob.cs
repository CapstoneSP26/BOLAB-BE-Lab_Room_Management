using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Helpers;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Jobs.Emails
{
    public class RejectBookingEmailJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public RejectBookingEmailJob(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task Execute(Guid bookingId)
        {
            var booking = await _unitOfWork.Repository<Booking>().Entities
                .Include(b => b.LabRoom)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null || !booking.CreatedBy.HasValue) return;

            var bookingReuqest = await _unitOfWork.Repository<BookingRequest>().Entities
                .FirstOrDefaultAsync(br => br.BookingId == bookingId);
            if (bookingReuqest == null) return;

            var user = await _unitOfWork.Repository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == booking.CreatedBy.Value);
            if (user == null) return;

            var shouldSendEmail = await _unitOfWork.Repository<UserNotificationPreference>().Entities
                .AsNoTracking()
                .AnyAsync(x => x.UserId == user.Id && x.EmailNotifications && x.BookingRejected);
            if (!shouldSendEmail) return;

            var template = await _unitOfWork.Repository<EmailTemplate>().Entities
                .FirstOrDefaultAsync(t => t.Type == EmailType.BookingRejected);

            if (template == null) return;

            var values = new Dictionary<string, string>
        {
            { "LecturerName", user.FullName },
            { "RoomName", booking.LabRoom.RoomName },
            { "Date", booking.StartTime.ToVietnamString("dd/MM/yyyy") },
            { "StartTime", booking.StartTime.ToVietnamString("HH:mm") },
            { "EndTime", booking.EndTime.ToVietnamString("HH:mm") },
            { "Reason", bookingReuqest.ResponseContext ?? "Vui lòng liên hệ quản trị viên để biết thêm chi tiết." },
        };

            var body = TemplateHelper.PopulateTemplate(template.Content, values);
            await _emailService.SendEmailAsync(user.Email, "❌ [BookLAB] Thông báo: Yêu cầu đặt phòng bị TỪ CHỐI", body);
        }
    }
}
