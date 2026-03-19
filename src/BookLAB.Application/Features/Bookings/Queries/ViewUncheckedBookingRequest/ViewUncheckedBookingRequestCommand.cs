using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.ViewUncheckedBookingRequest
{
    public class ViewUncheckedBookingRequestCommand : IRequest<List<BookingRequest>>
    {
        public Guid userId { get; set; }
    }
}
