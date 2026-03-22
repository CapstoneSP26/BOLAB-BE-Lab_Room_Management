using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Attendances.Commands.GenerateAttendanceQrCode
{
    public class GenerateAttendanceQrCodeCommand : IRequest<byte[]>
    {
        public Guid ScheduleId { get; set; }
        public bool IsCheckIn { get; set; }
    }
}
