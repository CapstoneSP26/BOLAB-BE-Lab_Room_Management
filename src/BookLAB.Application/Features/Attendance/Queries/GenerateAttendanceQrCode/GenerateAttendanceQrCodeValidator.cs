using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Attendance.Commands.GenerateAttendanceQrCode
{   
    public class GenerateAttendanceQrCodeValidator : AbstractValidator<GenerateAttendanceQrCodeCommand>
    {
        public GenerateAttendanceQrCodeValidator()
        {
            RuleFor(x => x.ScheduleId).NotEmpty().WithMessage("ScheduleId is required")
                .Must(x => Guid.TryParse(x, out _));
        }
    }
}
