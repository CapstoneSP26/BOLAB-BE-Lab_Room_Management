using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Attendance.Commands.ScanAttendanceQRCode
{
    public class ScanAttendanceQrCodeCommand : IRequest<bool>
    {
        public string qrId { get; set; }
        public string scheduleId { get; set; }
        public string studentId { get; set; }
    }
}
