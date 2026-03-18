using BookLAB.Domain.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.ViewBookingHistory
{
    public class ViewBookingHistoryCommand : IRequest<List<Booking>>
    {
        public Guid UserId { get; set; }
        public string userId { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public string status { get; set; }
        public DateTimeOffset startDate { get; set; }
        public DateTimeOffset endDate { get; set; }
    }
}
