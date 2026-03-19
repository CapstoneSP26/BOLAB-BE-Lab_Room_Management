using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Managements;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Attendances.Commands.GenerateAttendanceQrCode
{
    public class GenerateAttendanceQrCodeHandler : IRequestHandler<GenerateAttendanceQrCodeCommand, byte[]>
    {
        private readonly QrManagements _qrManagements;
        private readonly IUnitOfWork _unitOfWork;

        public GenerateAttendanceQrCodeHandler(QrManagements qrManagements, IUnitOfWork unitOfWork)
        {
            _qrManagements = qrManagements;
            _unitOfWork = unitOfWork;
        }
        public async Task<byte[]> Handle(GenerateAttendanceQrCodeCommand request, CancellationToken cancellationToken)
        {
            // Check if the schedule exists
            //await _unitOfWork.Repository<Schedule>().Entities.AnyAsync(x => x.Id == Guid.Parse(request.ScheduleId));
            //if (!await _unitOfWork.Repository<Schedule>().Entities.AnyAsync(x => x.Id == Guid.Parse(request.ScheduleId))) return null;

            var attendanceToken = Guid.NewGuid();

            var qrId = _qrManagements.CreateQRCode(new Qr
            {
                scheduleId = Guid.Parse(request.ScheduleId)
            });

            var qrCodeImage = _qrManagements.GetQrCode(qrId);

            return qrCodeImage;
        }
    }
}
