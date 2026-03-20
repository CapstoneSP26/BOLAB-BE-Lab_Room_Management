using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.IncidentReports.Queries.GetUnresolvedIncidents
{
    public class GetUnresolvedIncidentsQueryHandler : IRequestHandler<GetUnresolvedIncidentsQuery, List<IncidentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUnresolvedIncidentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<IncidentDto>> Handle(GetUnresolvedIncidentsQuery request, CancellationToken cancellationToken)
        {
            var limit = request.Limit ?? 10;
            var now = DateTime.UtcNow;
            var nowOffset = new DateTimeOffset(now, TimeSpan.Zero);

            var incidents = await _unitOfWork.Repository<Report>().Entities
                .Include(r => r.Schedule)
                .ThenInclude(s => s.LabRoom)
                .Where(r => !r.IsResolved)
                .OrderByDescending(r => r.CreatedAt)
                .Take(limit)
                .ToListAsync(cancellationToken);

            var createdByIds = incidents.Select(i => i.CreatedBy).Where(id => id.HasValue).Select(id => id.Value).Distinct();
            var users = await _unitOfWork.Repository<User>().Entities
                .Where(u => createdByIds.Contains(u.Id))
                .ToListAsync(cancellationToken);

            var result = incidents.Select(r =>
            {
                var user = users.FirstOrDefault(u => u.Id == r.CreatedBy);
                return new IncidentDto
                {
                    IncidentId = r.Id,
                    LabRoomName = r.Schedule?.LabRoom?.RoomName ?? "Unknown",
                    Description = r.Description,
                    IsResolved = r.IsResolved,
                    CreatedAt = r.CreatedAt.DateTime,
                    CreatedByName = user?.FullName ?? "Unknown",
                    DaysOpenCount = (int)(nowOffset - r.CreatedAt).TotalDays
                };
            }).ToList();

            return result;
        }
    }
}
