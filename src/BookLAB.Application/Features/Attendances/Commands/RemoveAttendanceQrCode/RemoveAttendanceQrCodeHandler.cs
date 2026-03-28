using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Features.Attendances.Commands.GenerateAttendanceQrCode;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Attendances.Commands.RemoveAttendanceQrCode
{
    public class RemoveAttendanceQrCodeHandler : IRequestHandler<RemoveAttendanceQrCodeCommand, bool>
    {
        private readonly IQrManagements _qrManagements;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RemoveAttendanceQrCodeHandler> _logger;

        public RemoveAttendanceQrCodeHandler(IQrManagements qrManagements,
            IUnitOfWork unitOfWork,
            ILogger<RemoveAttendanceQrCodeHandler> logger)
        {
            _qrManagements = qrManagements;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(RemoveAttendanceQrCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var qr = new Qr
                {
                    scheduleId = request.ScheduleId,
                    isCheckIn = request.IsCheckIn,
                };

                if (_qrManagements.CheckQrCodeExist(qr))
                {
                    _qrManagements.RemoveQRCode(qr);
                }
                return true;
            } catch (Exception ex)
            {
                return false;
            }
            
        }
    }
}
