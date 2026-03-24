<<<<<<
﻿using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.ViewUncheckedBookingRequest
{
    public class ViewUncheckedBookingRequestCommand : IRequest<List<BookingRequestDto>>
    {
        public Guid userId { get; set; }
    }
}
