using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Features.Attendances.Commands.ScanAttendanceQRCode;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace BookLAB.Application.Features.Attendances.Commands.ScanFaceAttendance
{
    public class ScanFaceAttendanceHandler : IRequestHandler<ScanFaceAttendanceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public ScanFaceAttendanceHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(ScanFaceAttendanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var student = await _unitOfWork.Repository<User>().Entities.FirstOrDefaultAsync(x => x.UserCode.Equals(request.studentCode));

                if (student == null)
                    return false;

                var schedule = await _unitOfWork.Repository<Schedule>().GetByIdAsync(request.scheduleId);

                if (schedule.GroupId == null)
                    return false;

                var members = await _unitOfWork.Repository<GroupMember>().Entities.Where(x => x.GroupId == schedule.GroupId && schedule.SubjectCode == x.SubjectCode).Select(x => x.UserId).ToListAsync();

                if (!members.Contains(student.Id))
                    return false;

                var attendanceId = Guid.NewGuid();

                var attendance = new Attendance
                {
                    Id = attendanceId,
                    ScheduleId = request.scheduleId,
                    UserId = student.Id,
                    CheckInTime = request.scanTime,
                    CheckInMethod = Domain.Enums.AttendanceCheckInMethod.FaceId,
                    AttendanceStatus = Domain.Enums.AttendanceStatus.Present,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = Guid.Parse("00000000-0000-0000-0000-000000000000")  // system creates this so use this guid
                };
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Repository<Attendance>().AddAsync(attendance);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();
                return true;
            } catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return false;
            }
            
        }
    }
}
