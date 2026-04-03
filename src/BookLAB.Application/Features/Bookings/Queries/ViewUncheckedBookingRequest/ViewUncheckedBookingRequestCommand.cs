using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.ViewUncheckedBookingRequest
{
    public class ViewUncheckedBookingRequestCommand : IRequest<List<BookingRequestFe>>
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
