using BookLAB.Application.Common.Interfaces.Services;
using MediatR;

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

            var isConflict = await _scheduleService.CheckConflictAsync(request.booking.LabRoomId, request.booking.StartTime.ToUniversalTime(), request.booking.EndTime.ToUniversalTime(), cancellationToken);

            if (isConflict) return true;

            return false;
        }
    }
}
