using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Helpers;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Jobs.Emails
{
    public class StudentScheduleNotifyJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public StudentScheduleNotifyJob(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task Execute(Guid scheduleId)
        {
            // 1. Lấy thông tin chi tiết của Schedule
            var schedule = await _unitOfWork.Repository<Schedule>().Entities
                .Include(s => s.LabRoom)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);

            if (schedule == null || schedule.ScheduleStatus == ScheduleStatus.Cancelled) return;

            // 2. Lấy danh sách sinh viên thuộc Group và Subject của Schedule này
            // Dựa trên bảng GroupMember chúng ta đã xây dựng
            var students = await _unitOfWork.Repository<GroupMember>().Entities
                .Where(gm => gm.GroupId == schedule.GroupId && gm.SubjectCode == schedule.SubjectCode)
                .Include(gm => gm.User)
                .Select(gm => new { gm.User.Email, gm.User.FullName })
                .ToListAsync();

            if (!students.Any()) return;

            // 3. Lấy Template Email từ Database
            var template = await _unitOfWork.Repository<EmailTemplate>().Entities
                .FirstOrDefaultAsync(t => t.Type == EmailType.StudentNotification);

            if (template == null) return;

            // 4. Gửi email cho từng sinh viên
            foreach (var student in students)
            {
                var values = new Dictionary<string, string>
                {
                    { "StudentName", student.FullName },
                    { "SubjectName", schedule.SubjectCode },
                    { "SubjectCode", schedule.SubjectCode },
                    { "RoomName", schedule.LabRoom.RoomName },
                    { "Date", schedule.StartTime.ToVietnamString("dd/MM/yyyy") },
                    { "StartTime", schedule.StartTime.ToVietnamString("HH:mm") },
                    { "EndTime", schedule.EndTime.ToVietnamString("HH:mm") },
                    { "LecturerName", schedule.User?.FullName ?? "Giảng viên bộ môn" },
                    { "AppUrl", "https://booklab.fpt.edu.vn" }
                };

                var body = TemplateHelper.PopulateTemplate(template.Content, values);

                await _emailService.SendEmailAsync(
                    student.Email,
                    $"[BookLAB] Lịch học mới môn {schedule.SubjectCode}",
                    body);
            }
        }
    }
}