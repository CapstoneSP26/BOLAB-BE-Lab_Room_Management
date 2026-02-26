using BookLAB.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.GetBookingStats
{
    public class GetBookingStatsCommand : IRequest<GetBookingStatsResponseDTO>
    {
        public DateTimeOffset startDate { get; set; }
        public DateTimeOffset endDate { get; set; }
    }
}
