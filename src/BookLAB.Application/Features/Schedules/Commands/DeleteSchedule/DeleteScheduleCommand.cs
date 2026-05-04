using BookLAB.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Commands.DeleteSchedule
{
    public class DeleteScheduleCommand : IRequest<ResultMessage<bool>>
    {
        public Guid Id { get; set; }
    }
}
