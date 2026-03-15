using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Queries.GetAvailableSlots
{
    public class GetAvailableSlotsCommand : IRequest<List<AvailableScheduleResponse>>
    {
        public AvailableScheduleRequest query { get; set; }
    }
}
