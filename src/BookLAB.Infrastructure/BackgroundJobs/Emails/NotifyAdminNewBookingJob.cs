using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Helpers;
using BookLAB.Application.Common.Interfaces.Jobs;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BookLAB.Infrastructure.BackgroundJobs.Emails
{
    public class NotifyAdminNewBookingJob : INotifyAdminNewBookingJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public NotifyAdminNewBookingJob(IUnitOfWork unitOfWork, IEmailService emailService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task Execute(Guid bookingId)
        {
            Console.WriteLine("=======================================================================>");
            // 1. Lấy thông tin Booking và người đặt
            var bookingRequest = await _unitOfWork.Repository<BookingRequest>().Entities
                .Include(br => br.Booking)
                    .ThenInclude(b => b.LabRoom)
                .Include(br => br.Booking)
                    .ThenInclude(b => b.PurposeType)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (bookingRequest == null || !bookingRequest.Booking.CreatedBy.HasValue) return;

            var lecturer = await _unitOfWork.Repository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == bookingRequest.CreatedBy.Value);

            if (lecturer == null) return;

            // 2. Lấy Template (Bạn có thể thêm EmailType.AdminNotification vào Enum)
            var template = await _unitOfWork.Repository<EmailTemplate>().Entities
                .FirstOrDefaultAsync(t => t.Type == EmailType.BookingReminder); // Hoặc template riêng cho Admin
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Template: " + (template != null ? "Found" : "Not Found"));
            var pendingBookingUrl = $"{_configuration["FrontendUrl"]}/labmanager/booking-requests/pending";

            // 3. Chuẩn bị dữ liệu
            var values = new Dictionary<string, string>
            {
                { "LecturerName", lecturer.FullName },
                { "RoomName", bookingRequest.Booking.LabRoom.RoomName },
                { "Date", bookingRequest.Booking.StartTime.ToVietnamString("dd/MM/yyyy") },
                { "StartTime", bookingRequest.Booking.StartTime.ToVietnamTimeString() },
                { "EndTime", bookingRequest.Booking.EndTime.ToVietnamTimeString() },
                { "Purpose", bookingRequest.Booking.PurposeType?.PurposeName ?? "N/A" },
                { "AdminApprovalLink", pendingBookingUrl}
            };

            var body = TemplateHelper.PopulateTemplate(template.Content, values);

            // 4. Gửi cho danh sách Admin (Hoặc lấy từ Email chung của bộ phận)
            // Giả sử bạn gửi đến một email cố định của phòng quản lý
            var labManagerEmails = await _unitOfWork.Repository<LabOwner>().Entities
                .Include(lo => lo.User)
                .Where(lo => lo.LabRoomId == bookingRequest.Booking.LabRoomId) //&& lo.User.NotificationPreference.EmailNotifications
                .Select(lo => lo.User.Email)
                .ToListAsync();
            if (labManagerEmails.Any())
            {
                var recipients = string.Join(", ", labManagerEmails);

                await _emailService.SendEmailAsync(
                    recipients,
                    "[BookLAB] Có yêu cầu đặt phòng mới cần phê duyệt",
                    body
                );
                
            }
        }
    }
}