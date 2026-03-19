using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Queries.AddSchedule
{
    public class AddScheduleCommand : IRequest<bool>
    {
        public Schedule Schedule { get; set; }
    }
}
