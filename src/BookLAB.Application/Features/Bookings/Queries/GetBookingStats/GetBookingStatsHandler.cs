using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.DTOs;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

        public GetBookingStatsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetBookingStatsResponseDTO> Handle(GetBookingStatsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Guid userId = Guid.Parse(request.userId);

                var bookingRequests = await _unitOfWork.Repository<BookingRequest>().Entities.Include(x => x.Booking).Where(x => x.RequestedByUserId.Equals(userId) && x.CreatedAt >= request.startDate && x.CreatedAt <= request.endDate).ToListAsync();

                GetBookingStatsResponseDTO response = new GetBookingStatsResponseDTO
                {
                    totalAccepted = bookingRequests.Count(x => x.BookingRequestStatus == BookingRequestStatus.Approved),
                    totalRejected = bookingRequests.Count(x => x.BookingRequestStatus == BookingRequestStatus.Rejected),
                    totalPending = bookingRequests.Count(x => x.BookingRequestStatus == BookingRequestStatus.Pending),
                    upcomingBookings = bookingRequests.Count(x => x.BookingRequestStatus == BookingRequestStatus.Approved && x.Booking.StartTime > DateTimeOffset.UtcNow)
                };

                return response;
            } catch (Exception ex)
            {
                return null;
            }
            
        }
    }
}
