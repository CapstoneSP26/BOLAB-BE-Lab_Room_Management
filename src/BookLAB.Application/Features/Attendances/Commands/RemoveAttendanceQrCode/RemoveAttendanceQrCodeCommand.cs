using BookLAB.Application.Common.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Attendances.Commands.RemoveAttendanceQrCode
{
    public class RemoveAttendanceQrCodeCommand : IRequest<bool>
    {
        public Guid ScheduleId { get; set; }
        public bool IsCheckIn { get; set; }
    }
}
