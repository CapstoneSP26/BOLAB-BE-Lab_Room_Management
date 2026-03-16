using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Infrastructure.Repositories
{
    public class IncidentRepository : GenericRepository<Incident>, IIncidentRepository
    {
        private readonly BookLABDbContext _context;

        public IncidentRepository(BookLABDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Incident>> GetByReportedByAsync(Guid reportedBy, CancellationToken cancellationToken = default)
        {
            return await _context.Incidents
                .Where(incident => incident.ReportedBy == reportedBy)
                .OrderByDescending(incident => incident.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}