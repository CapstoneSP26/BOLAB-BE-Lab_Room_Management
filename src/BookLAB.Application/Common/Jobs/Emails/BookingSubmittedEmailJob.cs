using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Helpers;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Jobs.Emails
{
    public class BookingSubmittedEmailJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public BookingSubmittedEmailJob(IUnitOfWork unitOfWork, IEmailService emailService)
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

            var user = await _unitOfWork.Repository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == booking.CreatedBy.Value);
            if (user == null) return;

            if (!await ShouldSendEmailAsync(user.Id)) return;

            var template = await _unitOfWork.Repository<EmailTemplate>().Entities
                .FirstOrDefaultAsync(t => t.Type == EmailType.BookingSubmitted);

            if (template == null) return;

            var values = new Dictionary<string, string>
        {
            { "LecturerName", user.FullName },
            { "BookingId", booking.Id.ToString().Substring(0, 8).ToUpper() }, // Lấy 8 ký tự đầu làm mã code
            { "RoomName", booking.LabRoom.RoomName },
            { "Date", booking.StartTime.ToVietnamString("dd/MM/yyyy") },
            { "StartTime", booking.StartTime.ToVietnamTimeString() },
            { "EndTime", booking.EndTime.ToVietnamTimeString() }
        };

            var body = TemplateHelper.PopulateTemplate(template.Content, values);
            await _emailService.SendEmailAsync(user.Email, "📩 [BookLAB] Xác nhận: Yêu cầu đặt phòng đã được tiếp nhận", body);
        }

        private Task<bool> ShouldSendEmailAsync(Guid userId)
        {
            return _unitOfWork.Repository<UserNotificationPreference>().Entities
                .AsNoTracking()
                .AnyAsync(x => x.UserId == userId && x.EmailNotifications);
        }
    }
}
