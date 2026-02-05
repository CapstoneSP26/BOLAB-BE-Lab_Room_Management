using BookLAB.Application.Common.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.CheckConflict
{
    public class CheckConflictHandler : IRequestHandler<CheckConflictCommand, bool>
    {
        private readonly IScheduleService _scheduleService;

        public CheckConflictHandler(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        public async Task<bool> Handle(CheckConflictCommand request, CancellationToken cancellationToken)
        {
            if (request.booking == null) return true;

            var isConflict = await _scheduleService.CheckConflictAsync(request.booking.LabRoomId, request.booking.StartTime, request.booking.EndTime);

            if (isConflict) return true;

            return false;
        }
    }
}
