using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Interfaces.Persistence
{
    public interface ILabOwnerRepository : IGenericRepository<LabOwner>
    {
        Task<List<Guid>> GetOwnerIdsByLabRoomIdAsync(int labRoomId);
        Task<bool> IsUserOwnerAsync(int labRoomId, Guid userId);
    }
}