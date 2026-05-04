using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.ViewUncheckedBookingRequest
{
    public class ViewUncheckedBookingRequestCommand : IRequest<ViewUncheckedBookingRequestReturn>
    {
        public Guid? userId { get; set; }
        public int page { get; set; } = 1;
        public int limit { get; set; } = 10;
        public string status { get; set; } = "all";
        public DateTimeOffset startDate { get; set; } = DateTimeOffset.Parse("2006-01-01T00:00:00Z");
        public DateTimeOffset endDate { get; set; } = DateTimeOffset.Parse("9999-01-01T00:00:00Z");
        public int? buildingId { get; set; }
        public int? labRoomId { get; set; }
        public int? slotTypeId { get; set; }
        public string? keyword { get; set; }
    }
}
