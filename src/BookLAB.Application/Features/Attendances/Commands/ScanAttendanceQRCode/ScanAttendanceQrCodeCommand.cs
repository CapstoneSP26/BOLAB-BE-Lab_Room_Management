using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Attendances.Commands.ScanAttendanceQRCode
{
    public class ScanAttendanceQrCodeCommand : IRequest<bool>
    {
        public Guid? AttendanceId { get; set; }
        public Guid LecturerId { get; set; }
        public Guid qrId { get; set; }
        public Guid scheduleId { get; set; }
        public Guid studentId { get; set; }
        public bool IsCheckIn { get; set; }
    }
}
