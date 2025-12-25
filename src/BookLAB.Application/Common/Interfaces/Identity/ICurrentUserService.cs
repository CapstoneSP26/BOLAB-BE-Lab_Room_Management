namespace BookLAB.Application.Common.Interfaces.Identity
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        IReadOnlyList<string> Roles { get; }
        bool IsAuthenticated { get; }
    }
}
