using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Attendances.Commands.ScanAttendanceQRCode
{
    public class ScanAttendanceQrCodeValidator : AbstractValidator<ScanAttendanceQrCodeCommand>
    {
        public ScanAttendanceQrCodeValidator()
        {
            RuleFor(x => x.AttendanceId)
                .Must(x => x == null || x != Guid.Empty).WithMessage("AttendanceId must be valid");
            RuleFor(x => x.LecturerId)
                .NotEmpty()
                .Must(x => x != Guid.Empty).WithMessage("LecturerId must be required and valid");
            RuleFor(x => x.studentId)
                .NotEmpty()
                .Must(x => x != Guid.Empty).WithMessage("studentId must be required and valid");
            RuleFor(x => x.qrId)
                .NotEmpty()
                .Must(x => x != Guid.Empty).WithMessage("QrId must be required and valid");
        }
    }
}
