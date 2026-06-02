namespace BookLAB.Application.Common.Interfaces.Jobs
{
    public interface IStudentScheduleNotifyJob
    {
        Task Execute(Guid scheduleId);
    }
}
