using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;
using MediatR;
using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Attendances.Commands.GenerateAttendanceQrCode
{
    public class GenerateAttendanceQrCodeHandler : IRequestHandler<GenerateAttendanceQrCodeCommand, byte[]>
    {
        private readonly IQrManagements _qrManagements;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GenerateAttendanceQrCodeHandler> _logger;

        public GenerateAttendanceQrCodeHandler(IQrManagements qrManagements, 
            IUnitOfWork unitOfWork,
            ILogger<GenerateAttendanceQrCodeHandler> logger)
        {
            _qrManagements = qrManagements;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GenerateAttendanceQrCodeCommand to create a QR code image for attendance.
        /// The method verifies that the schedule exists, generates a QR code using QrManagements,
        /// and returns the QR code image as a byte array.
        /// </summary>
        /// <param name="request">The command containing ScheduleId and IsCheckIn flag.</param>
        /// <param name="cancellationToken">Token to cancel the operation if requested.</param>
        /// <returns>
        /// A byte array representing the QR code image if successful, or null if the schedule does not exist or an error occurs.
        /// </returns>
        public async Task<byte[]> Handle(GenerateAttendanceQrCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate that the schedule exists in the database
                if (!await _unitOfWork.Repository<Schedule>().Entities
                    .AnyAsync(x => x.Id == request.ScheduleId, cancellationToken))
                    return null;

                // Create a new QR object with scheduleId and check-in flag
                var qr = _qrManagements.CreateQRCode(new Qr
                {
                    scheduleId = request.ScheduleId,
                    isCheckIn = request.IsCheckIn
                });

                // Retrieve the generated QR code image as byte array
                var qrCodeImage = _qrManagements.GetQrCode(qr);

                // Return the QR code image
                return qrCodeImage;
            }
            catch (Exception ex)
            {
                // Log the error with context information
                _logger.LogError(ex, "Error generating QR code for ScheduleId {ScheduleId}", request.ScheduleId);

                // Return null if an exception occurs
                return null;
            }
        }

    }
}
