using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.CheckConflict
{
    public class CheckConflictCommand : IRequest<bool>
    {
        public Booking booking { get; set; }
    }
}
