using BookLAB.Application.Common.Helpers;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Interfaces.Jobs;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLAB.Infrastructure.BackgroundJobs.Emails
{
    public class RecoverOanBookingEmailJob : IRecoverOanBookingEmailJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public RecoverOanBookingEmailJob(IUnitOfWork unitOfWork, IEmailService emailService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task Execute(int labRoomId, List<Guid> autoCancelledScheduleIds, List<Guid> autoRejectedBookingIds)
        {
            // 1. Kiểm tra phòng Lab sự cố giải phóng tài nguyên
            var room = await _unitOfWork.Repository<LabRoom>().GetByIdAsync(labRoomId);
            if (room == null) return;

            // 2. Lấy Template mẫu khôi phục phòng trống chung từ hệ thống DB
            var template = await _unitOfWork.Repository<EmailTemplate>().Entities
                .FirstOrDefaultAsync(t => t.Type == EmailType.BookingRecovered);
            if (template == null) return;

            var bookingUrl = $"{_configuration["FrontendUrl"] ?? "http://localhost:3000"}/booking";

            // ====================================================================================
            // 💚 KỊCH BẢN 1: GỬI MAIL CHO CÁC GIẢNG VIÊN CÓ LỊCH CHÍNH THỨC TỪNG BỊ HỦY (Auto-Cancelled)
            // ====================================================================================
            if (autoCancelledScheduleIds != null && autoCancelledScheduleIds.Any())
            {
                var schedules = await _unitOfWork.Repository<Schedule>().Entities
                    .IgnoreQueryFilters() // Bẻ Query Filter xóa mềm vì lịch bị đè đã dính IsDeleted = true
                    .Include(s => s.User)
                    .Where(s => autoCancelledScheduleIds.Contains(s.Id) && s.LecturerId != Guid.Empty)
                    .ToListAsync();

                foreach (var sch in schedules.Where(s => s.User != null))
                {
                    var values = new Dictionary<string, string>
                    {
                        { "LecturerName", sch.User.FullName },
                        { "RoomName", room.RoomName },
                        { "Date", sch.StartTime.ToString("dd/MM/yyyy") },
                        { "StartTime", sch.StartTime.ToString("HH:mm") },
                        { "EndTime", sch.EndTime.ToString("HH:mm") },
                        { "DetailLink", bookingUrl }
                    };

                    var body = TemplateHelper.PopulateTemplate(template.Content, values);
                    await _emailService.SendEmailAsync(sch.User.Email, "💚 [BookLAB] Cơ hội đặt lịch: Phòng máy cũ của bạn hiện đã TRỐNG LỊCH", body);
                }
            }

            // ====================================================================================
            // 🧡 KỊCH BẢN 2 (NÂNG CẤP): GỬI MAIL CHO CÁC ĐƠN ĐĂNG KÝ CHỜ DUYỆT TỪNG BỊ TỪ CHỐI (Auto-Rejected)
            // ====================================================================================
            if (autoRejectedBookingIds != null && autoRejectedBookingIds.Any())
            {
                // Truy vấn danh sách Đơn đặt phòng dính đè kèm thông tin Giảng viên thông qua CreatedBy
                // Sử dụng IgnoreQueryFilters để bao phủ trường hợp hệ thống có dính dáng cơ chế xóa mềm Booking
                var affectedBookings = await _unitOfWork.Repository<Booking>().Entities
                    .IgnoreQueryFilters()
                    .Where(b => autoRejectedBookingIds.Contains(b.Id) && b.CreatedBy.HasValue)
                    .ToListAsync();

                // Gom danh sách UserId người tạo để thực hiện nạp thông tin User một lượt (Tối ưu hóa Performance truy vấn)
                var userIds = affectedBookings.Select(b => b.CreatedBy!.Value).Distinct().ToList();
                var usersMap = await _unitOfWork.Repository<User>().Entities
                    .Where(u => userIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id);

                foreach (var booking in affectedBookings)
                {
                    // Kiểm tra xem User tạo đơn có tồn tại trong Map dữ liệu hay không
                    if (usersMap.TryGetValue(booking.CreatedBy!.Value, out var user))
                    {
                        var values = new Dictionary<string, string>
                        {
                            { "LecturerName", user.FullName },
                            { "RoomName", room.RoomName },
                            { "Date", booking.StartTime.ToString("dd/MM/yyyy") },
                            { "StartTime", booking.StartTime.ToString("HH:mm") },
                            { "EndTime", booking.EndTime.ToString("HH:mm") },
                            { "DetailLink", bookingUrl }
                        };

                        var body = TemplateHelper.PopulateTemplate(template.Content, values);

                        // Tiêu đề may mắn khích lệ người dùng quay lại nộp đơn đăng ký hàng đợi đặt phòng máy
                        string subject = $"🧡 [BookLAB] Phòng máy {room.RoomName} đã trống lịch - Hệ thống mời bạn nộp lại yêu cầu";
                        await _emailService.SendEmailAsync(user.Email, subject, body);
                    }
                }
            }
        }
    }
}