using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Interfaces.Repositories
{
    public interface IIncidentRepository : IGenericRepository<Incident>
    {
        Task<List<Incident>> GetByReportedByAsync(Guid reportedBy, CancellationToken cancellationToken = default);
    }
}