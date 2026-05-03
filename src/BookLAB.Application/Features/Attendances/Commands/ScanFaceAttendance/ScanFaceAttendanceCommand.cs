using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Attendances.Commands.ScanFaceAttendance
{
    public class ScanFaceAttendanceCommand : IRequest<bool>
    {
        public string studentCode { get; set; }
        public DateTimeOffset scanTime { get; set; }
        public Guid scheduleId { get; set; }
    }
}
