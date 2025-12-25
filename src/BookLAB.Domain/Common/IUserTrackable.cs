namespace BookLAB.Domain.Common
{
    public interface IUserTrackable
    {
        Guid? CreatedBy { get; set; }
        Guid? UpdatedBy { get; set; }
    }
}
