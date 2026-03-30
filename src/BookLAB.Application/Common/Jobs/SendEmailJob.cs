
using BookLAB.Application.Common.Helpers;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Jobs
{
    public class SendEmailJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public SendEmailJob(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task Execute(Guid bookingId)
        {
            // 1. Lấy thông tin Booking chi tiết (Include để lấy Email User và Tên phòng)
            var booking = await _unitOfWork.Repository<Booking>().Entities
                .Include(b => b.LabRoom)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null || booking.CreatedBy == null) return;

            var user = await _unitOfWork.Repository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == booking.CreatedBy.Value);
            if (user == null) return;

            // 2. Lấy Template tương ứng với loại "BookingApproved"
            var template = await _unitOfWork.Repository<EmailTemplate>().Entities
                .FirstOrDefaultAsync(t => t.Type == EmailType.BookingApproved);

            if (template == null) return;

            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            

            // 3. Chuẩn bị dữ liệu để thay thế vào Template
            var values = new Dictionary<string, string>
        {
            { "LecturerName",user.FullName },
            { "RoomName", booking.LabRoom.RoomName },
            { "StartTime", TimeZoneInfo.ConvertTime(booking.StartTime, vietnamTimeZone).ToString("dd/MM/yyyy HH:mm") },
            { "EndTime", TimeZoneInfo.ConvertTime(booking.EndTime, vietnamTimeZone).ToString("HH:mm") },
            { "Reason", booking.Reason }
        };

            var finalContent = TemplateHelper.PopulateTemplate(template.Content, values);

            // 4. Gửi Email thật sự
            await _emailService.SendEmailAsync(
                to: user.Email,
                subject: "Thông báo: Yêu cầu đặt phòng Lab đã được duyệt",
                body: finalContent
            );
        }
    }
}
