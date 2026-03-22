using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Attendances.Commands.GenerateAttendanceQrCode
{   
    public class GenerateAttendanceQrCodeValidator : AbstractValidator<GenerateAttendanceQrCodeCommand>
    {
        public GenerateAttendanceQrCodeValidator()
        {
            RuleFor(x => x.ScheduleId).NotEmpty()
                .Must(x => x != Guid.Empty).WithMessage("ScheduleId is required");
        }
    }
}
