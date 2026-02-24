using MediatR;

namespace BookLAB.Application.Features.Bookings.Commands.ImportSchedule
{
    public class ImportScheduleCommand : IRequest<bool> 
    {
        public List<ScheduleImportDto> Schedules { get; set; } = new();
    }
}
