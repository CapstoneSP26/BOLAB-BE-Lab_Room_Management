using BookLAB.Application.Features.Bookings.Commands.CreateBooking;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.CheckConflict
{
    public class CheckConflictCommand : IRequest<bool>
    {
        public CreateBookingCommand booking { get; set; }
    }
}
