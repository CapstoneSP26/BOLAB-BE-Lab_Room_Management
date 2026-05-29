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
    public class CancelBookingEmailJob : ICancelBookingEmailJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public CancelBookingEmailJob(IUnitOfWork unitOfWork, IEmailService emailService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task Execute(Guid targetId, bool isCancelledByAdmin, Guid actionByUserId)
        {
            // 1. Quét dữ liệu lịch lịch trình bằng IgnoreQueryFilters vì bản ghi Schedule đã dính IsDeleted = true khi bị hủy
            var schedule = await _unitOfWork.Repository<Schedule>().Entities
                .IgnoreQueryFilters()
                .Include(s => s.LabRoom)
                .FirstOrDefaultAsync(s => s.Id == targetId || s.BookingId == targetId);

            var booking = await _unitOfWork.Repository<Booking>().Entities
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(b => b.Id == targetId || (schedule != null && b.Id == schedule.BookingId));

            // ====================================================================================
            // 🚨 CASE THỦ TIÊU SỚM: Nếu là đơn bận Pending chưa được duyệt (schedule == null)
            // Theo yêu cầu: Luồng này tự hủy tự rút, không cần gửi mail cho bất kỳ ai.
            // ====================================================================================
            if (schedule == null)
            {
                return;
            }

            // 2. Định danh các thực thể liên quan đến lịch trình
            Guid? ownerId = booking?.CreatedBy ?? schedule?.CreatedBy ?? schedule?.LecturerId;
            if (!ownerId.HasValue) return;

            var user = await _unitOfWork.Repository<User>().Entities.FirstOrDefaultAsync(u => u.Id == ownerId.Value);
            var actionUser = await _unitOfWork.Repository<User>().Entities.FirstOrDefaultAsync(u => u.Id == actionByUserId);
            if (user == null) return;

            var roomName = schedule.LabRoom?.RoomName ?? "Phòng máy";
            var startTime = schedule.StartTime;
            var endTime = schedule.EndTime;
            var cancelReason = schedule.CancelReason ?? "Lý do thay đổi kế hoạch giảng dạy.";
            var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:3000";

            // Khởi tạo phôi dữ liệu gán vào Template chung
            var values = new Dictionary<string, string>
            {
                { "LecturerName", user.FullName },
                { "AdminName", actionUser?.FullName ?? "Quản trị viên phòng Lab" },
                { "RoomName", roomName },
                { "Date", startTime.ToString("dd/MM/yyyy") },
                { "StartTime", startTime.ToString("HH:mm") },
                { "EndTime", endTime.ToString("HH:mm") },
                { "Reason", cancelReason },
                { "DetailLink", $"{frontendUrl}/my-bookings" }
            };

            // ====================================================================================
            // PHÂN NHÁNH XỬ LÝ GỬI EMAIL THEO MA TRẬN PHÂN QUYỀN VẬN HÀNH
            // ====================================================================================
            if (isCancelledByAdmin)
            {
                // --------------------------------------------------------------------------------
                // 🔴 KHỐI 1: TRƯỞNG LAB ÉP HỦY LỊCH CỦA USER (Gửi Mail giải trình cho Giảng viên)
                // --------------------------------------------------------------------------------
                var template = await _unitOfWork.Repository<EmailTemplate>().Entities
                    .FirstOrDefaultAsync(t => t.Type == EmailType.BookingCancelledByAdmin);

                if (template != null)
                {
                    var body = TemplateHelper.PopulateTemplate(template.Content, values);
                    string subject = "🚨 [BookLAB] Thông báo quan trọng: Lịch sử dụng phòng máy ĐÃ BỊ HỦY bởi Ban quản lý";
                    await _emailService.SendEmailAsync(user.Email, subject, body);
                }
            }
            else
            {
                // --------------------------------------------------------------------------------
                // 🟢 KHỐI 2: GIẢNG VIÊN (LECTURER) TỰ HỦY LỊCH SCHEDULE CHÍNH THỨC CỦA MÌNH
                // Yêu cầu: Gửi mail báo hủy thành công cho chính mình ĐỒNG THỜI báo cho Trưởng phòng Lab quản lý.
                // --------------------------------------------------------------------------------

                // Bước A: Gửi mail báo thành công cho chính Giảng viên bấm hủy
                var ownerTemplate = await _unitOfWork.Repository<EmailTemplate>().Entities
                    .FirstOrDefaultAsync(t => t.Type == EmailType.BookingCancelledByOwner);

                if (ownerTemplate != null)
                {
                    var body = TemplateHelper.PopulateTemplate(ownerTemplate.Content, values);
                    string subject = "🗑️ [BookLAB] Xác nhận: Bạn đã hủy lịch đặt phòng thành công";
                    await _emailService.SendEmailAsync(user.Email, subject, body);
                }

                // 🔥 Bước B (NÂNG CẤP): Quét tìm toàn bộ danh sách email của các Trưởng phòng Lab quản lý phòng này
                var labRoomId = schedule.LabRoomId;
                var ownerIds = await _unitOfWork.LabOwners.GetOwnerIdsByLabRoomIdAsync(labRoomId);

                if (ownerIds != null && ownerIds.Any())
                {
                    var labAdmins = await _unitOfWork.Repository<User>().Entities
                        .Where(u => ownerIds.Distinct().Contains(u.Id) && u.Id != user.Id) // Tránh gửi ngược lại cho chính giảng viên nếu họ cũng có quyền owner
                        .ToListAsync();

                    // Tìm template thông báo phòng trống dành cho Admin (Nếu chưa có, hệ thống sẽ sử dụng fallback body bên dưới)
                    var adminNotificationTemplate = await _unitOfWork.Repository<EmailTemplate>().Entities
                        .FirstOrDefaultAsync(t => t.Type == EmailType.NotifyAdminBookingCancelledByOwner);

                    foreach (var admin in labAdmins)
                    {
                        string adminBody;
                        if (adminNotificationTemplate != null)
                        {
                            // Cập nhật tên Admin vào phôi template trước khi gửi
                            var adminValues = new Dictionary<string, string>(values);
                            adminValues["AdminName"] = admin.FullName;
                            adminBody = TemplateHelper.PopulateTemplate(adminNotificationTemplate.Content, adminValues);
                        }
                        else
                        {
                            // Fallback mẫu HTML tinh gọn gửi cho Trưởng phòng Lab nếu DB chưa kịp nạp template mẫu
                            adminBody = $@"
                                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                                    <div style='background-color: #f57c00; color: white; padding: 20px; text-align: center;'>
                                        <h2>[BookLAB] Thông Báo Giải Phóng Phòng Máy Trống</h2>
                                    </div>
                                    <div style='padding: 25px;'>
                                        <p>Kính gửi Thầy/Cô Trưởng phòng máy <b>{admin.FullName}</b>,</p>
                                        <p>Hệ thống ghi nhận Giảng viên <b>{user.FullName}</b> đã chủ động thực hiện lệnh <b>HỦY LỊCH TRÌNH CHÍNH THỨC</b> tại phòng máy do Thầy/Cô quản lý. Khung giờ này hiện đã được giải phóng trống lịch trên hệ thống.</p>
                                        <table style='width: 100%; margin: 15px 0; border-collapse: collapse;'>
                                            <tr><td style='padding: 5px; color: #666;'>Phòng máy:</td><td style='font-weight: bold;'>{roomName}</td></tr>
                                            <tr><td style='padding: 5px; color: #666;'>Ngày sử dụng:</td><td style='font-weight: bold;'>{startTime:dd/MM/yyyy}</td></tr>
                                            <tr><td style='padding: 5px; color: #666;'>Khung giờ giải phóng:</td><td style='font-weight: bold; color: #f57c00;'>{startTime:HH:mm} - {endTime:HH:mm}</td></tr>
                                            <tr><td style='padding: 5px; color: #666;'>Lý do hủy lịch:</td><td style='font-style: italic;'>{cancelReason}</td></tr>
                                        </table>
                                        <p>Thông tin được cập nhật tự động phục vụ công tác giám sát điều phối tài nguyên phòng Lab của nhà trường.</p>
                                    </div>
                                </div>";
                        }

                        string adminSubject = $"[BookLAB] Thông báo: Giảng viên hủy lịch trình tại phòng {roomName} ({startTime:dd/MM/yyyy})";
                        await _emailService.SendEmailAsync(admin.Email, adminSubject, adminBody);
                    }
                }
            }
        }
    }
}