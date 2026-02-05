using BookLAB.Application.Common.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BookLAB.Application.Features.Bookings.AddSchedule
{
    public class AddScheduleHandler : IRequestHandler<AddScheduleCommand, bool>
    {
        private readonly IScheduleService _scheduleService;

        public AddScheduleHandler(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        public async Task<bool> Handle(AddScheduleCommand request, CancellationToken cancellationToken)
        {
            if (request.Schedule == null)
            {
                return false;
            }

            return await _scheduleService.AddScheduleAsync(request.Schedule);
        }
    }
}
