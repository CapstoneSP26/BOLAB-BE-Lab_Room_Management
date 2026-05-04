using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Helpers;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Jobs.Emails
{
    public class NotifyAdminNewBookingJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public NotifyAdminNewBookingJob(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task Execute(Guid bookingId)
        {
            // 1. Lấy thông tin Booking và người đặt
            var booking = await _unitOfWork.Repository<Booking>().Entities
                .Include(b => b.LabRoom)
                .Include(b => b.PurposeType)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null || !booking.CreatedBy.HasValue) return;

            var lecturer = await _unitOfWork.Repository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == booking.CreatedBy.Value);

            if (lecturer == null) return;

            // 2. Lấy Template (Bạn có thể thêm EmailType.AdminNotification vào Enum)
            var template = await _unitOfWork.Repository<EmailTemplate>().Entities
                .FirstOrDefaultAsync(t => t.Type == EmailType.BookingSubmitted); // Hoặc template riêng cho Admin

            // 3. Chuẩn bị dữ liệu
            var values = new Dictionary<string, string>
            {
                { "LecturerName", lecturer.FullName },
                { "RoomName", booking.LabRoom.RoomName },
                { "Date", booking.StartTime.ToVietnamString("dd/MM/yyyy") },
                { "StartTime", booking.StartTime.ToVietnamTimeString() },
                { "EndTime", booking.EndTime.ToVietnamTimeString() },
                { "Purpose", booking.PurposeType?.PurposeName ?? "N/A" },
                { "AdminApprovalLink", "https://booklab.edu.vn/admin/approvals" }
            };

            var body = TemplateHelper.PopulateTemplate(template.Content, values);

            // 4. Gửi cho danh sách Admin (Hoặc lấy từ Email chung của bộ phận)
            // Giả sử bạn gửi đến một email cố định của phòng quản lý
            var labManagerEmails = await _unitOfWork.Repository<LabOwner>().Entities
                .Include(lo => lo.User)
                .Where(lo => lo.LabRoomId == booking.LabRoomId)
                .Select(lo => lo.User.Email)
                .ToListAsync();
            if (labManagerEmails.Any())
            {
                var allowedRecipients = new List<string>();

                foreach (var email in labManagerEmails)
                {
                    var owner = await _unitOfWork.Repository<User>().Entities
                        .FirstOrDefaultAsync(u => u.Email == email);

                    if (owner == null)
                        continue;

                    var preferenceEnabled = await _unitOfWork.Repository<UserNotificationPreference>().Entities
                        .AsNoTracking()
                        .AnyAsync(x => x.UserId == owner.Id && x.EmailNotifications);

                    if (preferenceEnabled)
                    {
                        allowedRecipients.Add(email);
                    }
                }

                if (allowedRecipients.Any())
                {
                    var recipients = string.Join(", ", allowedRecipients);

                    await _emailService.SendEmailAsync(
                        recipients,
                        "⚠️ [BookLAB] Có yêu cầu đặt phòng mới cần phê duyệt",
                        body
                    );
                }
            }
        }
    }
}