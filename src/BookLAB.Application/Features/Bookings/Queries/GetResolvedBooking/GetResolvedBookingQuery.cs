using BookLAB.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.GetResolvedBooking
{
    public class GetResolvedBookingQuery : IRequest<GetResolvedBookingReturn>
    {
        public Guid userId { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public string status { get; set; }
        public DateTimeOffset startDate { get; set; }
        public DateTimeOffset endDate { get; set; }
        public int? labRoomId { get; set; }
    }
}
