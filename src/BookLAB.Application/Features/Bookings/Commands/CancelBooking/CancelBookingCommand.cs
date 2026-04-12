using BookLAB.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Commands.CancelBooking
{
    public class CancelBookingCommand : IRequest<ResultMessage<bool>>
    {
        public Guid BookingId { get; set; }
    }
}
