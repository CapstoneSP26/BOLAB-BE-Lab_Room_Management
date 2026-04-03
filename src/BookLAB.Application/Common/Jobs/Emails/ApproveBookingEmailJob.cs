using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Helpers;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Jobs.Emails
{
    public class ApproveBookingEmailJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public ApproveBookingEmailJob(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task Execute(Guid bookingId)
        {
            var booking = await _unitOfWork.Repository<Booking>().Entities
                .Include(b => b.LabRoom)
                .Include(b => b.PurposeType)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null || !booking.CreatedBy.HasValue) return;

            var user = await _unitOfWork.Repository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == booking.CreatedBy.Value);
            if (user == null) return;

            var template = await _unitOfWork.Repository<EmailTemplate>().Entities
                .FirstOrDefaultAsync(t => t.Type == EmailType.BookingApproved);
            if (template == null) return;

            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
             
            var values = new Dictionary<string, string>
        {
            { "LecturerName", user.FullName },
            { "RoomName", booking.LabRoom.RoomName },
            { "Date",booking.StartTime.ToVietnamString("dd/mm/yyyy")},
            { "StartTime", booking.StartTime.ToVietnamString("HH:mm") },
            { "EndTime", booking.EndTime.ToVietnamString("HH:mm") },
            { "Reason", booking.Reason ?? "Yêu cầu của bạn đã được chấp nhận." },
            { "PurposeType", booking.PurposeType.PurposeName ?? "No Purpose"}

        };

            var body = TemplateHelper.PopulateTemplate(template.Content, values);
            await _emailService.SendEmailAsync(user.Email, "✅ [BookLAB] Thông báo: Lịch đặt phòng đã được DUYỆT", body);
        }
    }
}
