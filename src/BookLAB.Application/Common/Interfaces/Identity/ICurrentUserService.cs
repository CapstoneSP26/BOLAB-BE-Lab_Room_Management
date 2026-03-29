namespace BookLAB.Application.Common.Interfaces.Identity
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        int CampusId { get; }
        IReadOnlyList<string> Roles { get; }
        bool IsAuthenticated { get; }
    }
}
