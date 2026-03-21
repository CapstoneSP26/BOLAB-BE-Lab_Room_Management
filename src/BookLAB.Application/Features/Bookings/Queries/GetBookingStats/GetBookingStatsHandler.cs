using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.DTOs;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.GetBookingStats
{
    public class GetBookingStatsHandler : IRequestHandler<GetBookingStatsCommand, GetBookingStatsResponseDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetBookingStatsHandler> _logger;

        public GetBookingStatsHandler(IUnitOfWork unitOfWork, ILogger<GetBookingStatsHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetBookingStatsCommand by retrieving booking requests for a given user
        /// within the specified date range, then calculates statistics such as total accepted,
        /// rejected, pending, and upcoming bookings.
        /// </summary>
        /// <param name="request">
        /// The command containing userId, startDate, and endDate filters.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to observe while waiting for the task to complete, useful for cancellation.
        /// </param>
        /// <returns>
        /// Returns a GetBookingStatsResponseDTO with aggregated booking statistics.
        /// If an exception occurs, returns null.
        /// </returns>
        public async Task<GetBookingStatsResponseDTO> Handle(GetBookingStatsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Safely parse the userId string into a Guid.
                Guid.TryParse(request.userId, out Guid userId);

                // Query BookingRequest entities, including related Booking,
                // filtered by userId and date range.
                var bookingRequests = await _unitOfWork.Repository<BookingRequest>().Entities
                    .Include(x => x.Booking)
                    .Where(x => x.RequestedByUserId.Equals(userId)
                                && x.CreatedAt >= request.startDate
                                && x.CreatedAt <= request.endDate)
                    .ToListAsync(cancellationToken);

                // Initialize counters for different booking statuses.
                int totalAccepted = 0;
                int totalRejected = 0;
                int totalPending = 0;
                int upcomingBookings = 0;

                // Loop through the booking requests once and update counters accordingly.
                foreach (var req in bookingRequests)
                {
                    switch (req.BookingRequestStatus)
                    {
                        case BookingRequestStatus.Approved:
                            totalAccepted++;
                            // Count upcoming bookings only if approved and start time is in the future.
                            if (req.Booking.StartTime > DateTimeOffset.UtcNow)
                                upcomingBookings++;
                            break;
                        case BookingRequestStatus.Rejected:
                            totalRejected++;
                            break;
                        case BookingRequestStatus.Pending:
                            totalPending++;
                            break;
                    }
                }

                // Build the response DTO with aggregated statistics.
                GetBookingStatsResponseDTO response = new GetBookingStatsResponseDTO
                {
                    totalAccepted = totalAccepted,
                    totalRejected = totalRejected,
                    totalPending = totalPending,
                    upcomingBookings = upcomingBookings
                };

                return response;
            }
            catch (Exception ex)
            {
                // Log the exception with context information
                _logger.LogError(ex, "Error occurred while handling GetBookingStatsHandler");
                return null;
            }
        }

    }
}
