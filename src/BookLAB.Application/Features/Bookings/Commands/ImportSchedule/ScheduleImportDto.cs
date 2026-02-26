
namespace BookLAB.Application.Features.Bookings.Commands.ImportSchedule
{
    public record ScheduleImportDto(
        string RoomCode,
        string LecturerEmail,
        DateTime StartTime,
        DateTime EndTime,
        string SubjectName
    );
}
