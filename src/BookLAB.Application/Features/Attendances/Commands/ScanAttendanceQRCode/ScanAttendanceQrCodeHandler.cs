using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Attendances.Commands.ScanAttendanceQRCode;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Attendances.Commands.ScanAttendanceQRCode
{
    public class ScanAttendanceQrCodeHandler : IRequestHandler<ScanAttendanceQrCodeCommand, ResultMessage<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQrManagements _qrManagements;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<ScanAttendanceQrCodeHandler> _logger;

        public ScanAttendanceQrCodeHandler(IUnitOfWork unitOfWork,
            IQrManagements qrManagements,
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            ILogger<ScanAttendanceQrCodeHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _qrManagements = qrManagements;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        /// <summary>
        /// Handles the ScanAttendanceQrCodeCommand to record attendance based on a scanned QR code.
        /// Performs validation checks, creates an Attendance record (check-in or check-out),
        /// and saves it to the database within a transaction.
        /// </summary>
        /// <param name="request">The command containing scheduleId, studentId, attendanceId, and check-in flag.</param>
        /// <param name="cancellationToken">Token to cancel the operation if requested.</param>
        /// <returns>True if attendance was recorded successfully, false otherwise.</returns>
        public async Task<ResultMessage<bool>> Handle(ScanAttendanceQrCodeCommand request, CancellationToken cancellationToken)
        {
            var campus = await _unitOfWork.Repository<Campus>().GetByIdAsync(_currentUserService.CampusId);
            if (campus == null)
                return new ResultMessage<bool>
                {
                    Success = false,
                    Message = "Campus not found."
                };

            var distance = CalculateDistance(campus.Latitude, campus.Longitude, request.Latitude, request.Longtitude);

            if (distance > 50)
            {
                return new ResultMessage<bool>
                {
                    Success = false,
                    Message = "You are too far from the campus to check in."
                };
            }

            // Ensure the QR code does not already exist in the system
            if (_qrManagements.CheckQrCodeExist(new Qr { scheduleId = request.scheduleId, isCheckIn = request.IsCheckIn }))
                return new ResultMessage<bool> { Success = false, Message = "QR code already exists." };

            // Verify that the schedule exists in the database
            if (!await _unitOfWork.Repository<Schedule>().Entities.AnyAsync(s => s.Id == request.scheduleId, cancellationToken))
                return new ResultMessage<bool> { Success = false, Message = "Schedule does not exist." };

            // Verify that the student exists in the user repository
            if (!await _userRepository.IfExisted(request.studentId))
                return new ResultMessage<bool> { Success = false, Message = "Student does not exist." };

            // Prevent invalid check-out if AttendanceId is provided incorrectly
            if ((request.AttendanceId != null && request.AttendanceId != Guid.Empty) && !request.IsCheckIn)
                return new ResultMessage<bool> { Success = false, Message = "Invalid check-out attempt." };

            if (_unitOfWork.Repository<Attendance>().Entities.Any(x => x.ScheduleId == request.scheduleId && x.UserId == request.studentId && x.CheckInTime.HasValue == request.IsCheckIn && x.AttendanceStatus.Equals(AttendanceStatus.Present)))
                return new ResultMessage<bool> { Success = false, Message = "Attendance record already exists." };

            // Build the Attendance record depending on check-in or check-out
            var attendance = request.IsCheckIn
                ? new Attendance
                {
                    Id = Guid.NewGuid(),                       // Generate a new unique Id
                    ScheduleId = request.scheduleId,           // Link to the schedule
                    UserId = request.studentId,                // Link to the student
                    CheckInTime = DateTimeOffset.UtcNow,       // Record current check-in time
                    CheckInMethod = AttendanceCheckInMethod.QR,// Mark check-in method as QR
                    AttendanceStatus = AttendanceStatus.Present,// Mark student as present
                    CreatedAt = DateTimeOffset.UtcNow,         // Timestamp creation
                    CreatedBy = request.LecturerId,            // Track who created it
                }
                : new Attendance
                {
                    Id = request.AttendanceId.Value,           // Use existing AttendanceId for check-out
                    ScheduleId = request.scheduleId,           // Link to the schedule
                    UserId = request.studentId,                // Link to the student
                    CheckOutTime = DateTimeOffset.UtcNow,      // Record current check-out time
                    CheckInMethod = AttendanceCheckInMethod.QR,// Mark check-in method as QR
                    AttendanceStatus = AttendanceStatus.Present,// Mark student as present
                    UpdatedAt = DateTimeOffset.UtcNow,         // Timestamp update
                    UpdatedBy = request.LecturerId,            // Track who updated it
                };

            var existedAttendance = await _unitOfWork.Repository<Attendance>().Entities.FirstOrDefaultAsync(x => x.UserId == request.studentId && x.ScheduleId == request.scheduleId, cancellationToken);

            try
            {
                if (existedAttendance != null)
                {
                    attendance.Id = existedAttendance.Id;
                    await _unitOfWork.BeginTransactionAsync();

                    // Add the new attendance record to the repository
                    await _unitOfWork.Repository<Attendance>().UpdateAsync(attendance);

                    // Save changes to the database
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Commit transaction if everything succeeds
                    await _unitOfWork.CommitTransactionAsync();

                    return new ResultMessage<bool> { Success = true, Message = "Attendance recorded successfully.", Data = true };
                }

            
                // Begin transaction to ensure data consistency
                await _unitOfWork.BeginTransactionAsync();

                // Add the new attendance record to the repository
                await _unitOfWork.Repository<Attendance>().AddAsync(attendance);

                // Save changes to the database
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Commit transaction if everything succeeds
                await _unitOfWork.CommitTransactionAsync();

                return new ResultMessage<bool> { Success = true, Message = "Attendance recorded successfully.", Data = true };
            }
            catch (Exception ex)
            {
                // Roll back transaction if an error occurs
                await _unitOfWork.RollbackTransactionAsync();

                // Log the exception with details for debugging
                _logger.LogError(ex, "Error scanning QR code for ScheduleId {ScheduleId}", request.scheduleId);

                return new ResultMessage<bool> { Success = false }; // Return false if an exception was thrown
            }
        }

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Bán kính Trái Đất (mét)

            // Chuyển đổi độ sang radian
            double latRad1 = lat1 * Math.PI / 180.0;
            double latRad2 = lat2 * Math.PI / 180.0;
            double deltaLat = (lat2 - lat1) * Math.PI / 180.0;
            double deltaLon = (lon2 - lon1) * Math.PI / 180.0;

            // Công thức Haversine
            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(latRad1) * Math.Cos(latRad2) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = R * c; // Khoảng cách tính bằng mét
            return distance;
        }
    }
}
