using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Infrastructure.Repositories
{
    public class LabOwnerRepository : GenericRepository<LabOwner>, ILabOwnerRepository
    {
        private readonly BookLABDbContext _context;

        public LabOwnerRepository(BookLABDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Guid>> GetOwnerIdsByLabRoomIdAsync(int labRoomId)
        {
            return await _context.LabOwners
                .Where(lo => lo.LabRoomId == labRoomId)
                .Select(lo => lo.UserId)
                .ToListAsync();
        }

        public async Task<bool> IsUserOwnerAsync(int labRoomId, Guid userId)
        {
            return await _context.LabOwners
                .AnyAsync(lo => lo.LabRoomId == labRoomId && lo.UserId == userId);
        }
    }
}