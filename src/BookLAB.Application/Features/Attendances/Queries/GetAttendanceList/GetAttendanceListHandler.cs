using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Attendances.Queries.GetAttendanceList;

public class GetAttendanceListHandler : IRequestHandler<GetAttendanceListQuery, List<AttendanceStudentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAttendanceListHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<AttendanceStudentDto>> Handle(GetAttendanceListQuery request, CancellationToken ct)
    {
        // 1. Lấy thông tin Schedule để xác định Group và SubjectCode
        var schedule = await _unitOfWork.Repository<Schedule>().Entities
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.ScheduleId, ct);

        if (schedule == null) return new List<AttendanceStudentDto>();

        // 2. Lấy danh sách điểm danh đã tồn tại trong DB (nếu đã từng điểm danh hoặc lưu nháp)
        var existingAttendance = await _unitOfWork.Repository<Attendance>().Entities
            .AsNoTracking()
            .Where(a => a.ScheduleId == request.ScheduleId)
            .ToDictionaryAsync(a => a.UserId, ct);

        // 3. Truy vấn danh sách sinh viên dựa trên logic "Biến thiên theo môn học"
        // Lấy từ bảng GroupStudent (Nơi gán StudentId + GroupId + SubjectCode)
        var studentList = await _unitOfWork.Repository<GroupMember>().Entities
            .AsNoTracking()
            .Include(gs => gs.User)
            .Where(gs => gs.GroupId == schedule.GroupId && gs.SubjectCode == schedule.SubjectCode)
            .Select(gs => new AttendanceStudentDto
            {
                UserId = gs.UserId,
                StudentCode = gs.User.UserCode, // Giả sử UserName là mã sinh viên
                FullName = gs.User.FullName,
                Email = gs.User.Email,
                Status = AttendanceStatus.Absent // Giá trị khởi tạo mặc định
            })
            .ToListAsync(ct);

        // 4. Map dữ liệu điểm danh thực tế vào danh sách sinh viên dự kiến
        foreach (var student in studentList)
        {
            if (existingAttendance.TryGetValue(student.UserId, out var att))
            {
                student.Status = att.AttendanceStatus;
                student.CheckInTime = att.CheckInTime;
                // Bạn có thể bổ sung Note vào Entity Attendance nếu cần
            }
        }

        return studentList.OrderBy(s => s.StudentCode).ToList();
    }
}