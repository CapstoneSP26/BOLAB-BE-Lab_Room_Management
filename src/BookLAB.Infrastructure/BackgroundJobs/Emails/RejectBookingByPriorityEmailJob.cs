using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Helpers;
using BookLAB.Application.Common.Interfaces.Jobs;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Infrastructure.BackgroundJobs.Emails
{
    public class RejectBookingByPriorityEmailJob : IRejectBookingByPriorityEmailJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public RejectBookingByPriorityEmailJob(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task Execute(List<Guid> bookingIds, List<Guid> scheduleIds)
        {
            if ((bookingIds == null || !bookingIds.Any()) && (scheduleIds == null || !scheduleIds.Any()))
                return;

            var template = await _unitOfWork.Repository<EmailTemplate>().Entities
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Type == EmailType.RejectedByPriority);
            Console.WriteLine($"=======>[RejectBookingByPriorityEmailJob] Fetched email template: {(template != null ? "Found" : "Not Found")}");
            if (template == null) return;

            // =========================================================================
            // CHẶNG 1: XỬ LÝ GỬI EMAIL CHO GIẢNG VIÊN ĐẶT PHÒNG LẺ (BOOKING REQUESTS)
            // =========================================================================
            if (bookingIds != null && bookingIds.Any())
            {
                Console.WriteLine("=============> bookingIds : " + bookingIds.Count);
                var rejectedBookings = await _unitOfWork.Repository<BookingRequest>().Entities
                    .AsNoTracking()
                    .Include(br => br.Booking).ThenInclude(b => b.LabRoom)
                    .Where(br => bookingIds.Contains(br.BookingId) && br.CreatedBy.HasValue)
                    .ToListAsync();
                Console.WriteLine("=============> RejectedBookings : " + rejectedBookings.Count);

                foreach (var br in rejectedBookings)
                {
                    var userWithPref = await _unitOfWork.Repository<User>().Entities
                        .AsNoTracking()
                        .Select(u => new
                        {
                            u.Id,
                            u.Email,
                            u.FullName
                        })
                        .FirstOrDefaultAsync(u => u.Id == br.CreatedBy.Value);
                    if (userWithPref == null || string.IsNullOrEmpty(userWithPref.Email))
                        continue;

                    var values = new Dictionary<string, string>
                    {
                        { "ReceiverName", userWithPref.FullName },
                        { "TargetType", "Yêu cầu đăng ký phòng Lab lẻ" },
                        { "TargetName", $"Đăng ký sử dụng {br.Booking.LabRoom.RoomName}" },
                        { "RoomName", br.Booking.LabRoom.RoomName },
                        { "Date", br.Booking.StartTime.ToVietnamString("dd/MM/yyyy") },
                        { "TimeSlot", $"{br.Booking.StartTime.ToVietnamTimeString()} - {br.Booking.EndTime.ToVietnamTimeString()}" },
                        { "Reason", "Hệ thống tự động hủy do khung giờ này đã được cấp quyền sử dụng cho một hoạt động khác sở hữu mức độ ưu tiên (Priority) cao hơn." },
                        { "ActionLink", "https://booklab.edu.vn/dashboard" }
                    };

                    var body = TemplateHelper.PopulateTemplate(template.Content, values);
                    _ = _emailService.SendEmailAsync(userWithPref.Email, "🚨 [BookLAB] Hủy lịch đăng ký phòng Lab do trùng lịch ưu tiên", body);
                }
            }

            // =========================================================================
            // CHẶNG 2: XỬ LÝ LỊCH HỌC CHÍNH KHÓA (SCHEDULES - PHỨC TẠP HƠN)
            // =========================================================================
            if (scheduleIds != null && scheduleIds.Any())
            {
                var rejectedSchedules = await _unitOfWork.Repository<Schedule>().Entities
                    .AsNoTracking()
                    .Include(cs => cs.LabRoom)
                    .Where(cs => scheduleIds.Contains(cs.Id))
                    .ToListAsync();
                Console.WriteLine("=============> RejectedSchedules : " + rejectedSchedules.Count);


                foreach (var sched in rejectedSchedules)
                {
                    // Tập hợp ID những người liên quan (Giảng viên phụ trách và Người tạo lịch nếu có)
                    var recipientUserIds = new HashSet<Guid> { sched.LecturerId };
                    if (sched.CreatedBy.HasValue) recipientUserIds.Add(sched.CreatedBy.Value);

                    // Lấy danh sách User hợp lệ có bật cấu hình nhận thông báo qua Email
                    var usersToNotify = await _unitOfWork.Repository<User>().Entities
                        .AsNoTracking()
                        .Where(u => recipientUserIds.Contains(u.Id))
                        .Select(u => new
                        {
                            u.Email,
                            u.FullName
                        })
                        .Where(u =>!string.IsNullOrEmpty(u.Email))
                        .ToListAsync();

                    // Gửi thông báo chung cho tất cả các bên liên quan
                    foreach (var targetUser in usersToNotify)
                    {
                        var values = new Dictionary<string, string>
            {
                { "ReceiverName", targetUser.FullName },
                { "TargetType", "Lịch trình hệ thống (Schedule)" },
                { "TargetName", $"Mã môn học/Sự kiện: {sched.SubjectCode ?? "N/A"}" },
                { "RoomName", sched.LabRoom?.RoomName ?? "N/A" },
                { "Date", sched.StartTime.ToVietnamString("dd/MM/yyyy") },
                { "TimeSlot", $"{sched.StartTime.ToVietnamTimeString()} - {sched.EndTime.ToVietnamTimeString()}" },
                { "Reason", "Lịch trình này đã bị hệ thống tự động hủy và thu hồi phòng do xuất hiện xung đột với một tác vụ khác được cấp quyền có mức độ ưu tiên (Priority) cao hơn." },
                { "ActionLink", "https://booklab.edu.vn/schedule" }
            };

                        var body = TemplateHelper.PopulateTemplate(template.Content, values);

                        _ = _emailService.SendEmailAsync(
                            targetUser.Email,
                            "🚨 [BookLAB] Thông báo hủy lịch trình do trùng lịch ưu tiên",
                            body
                        );
                    }
                }
            }
        }
    }
}