using BookLAB.Application.Features.Schedules.Queries.GetSchedules;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Queries.GetCurrentScheduleInRoom
{
    public class GetCurrentScheduleInRoomQuery : IRequest<List<ScheduleDto>>
    {
        public string roomNo { get; set; }
    }
}
