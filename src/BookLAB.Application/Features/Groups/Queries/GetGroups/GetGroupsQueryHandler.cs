using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Groups.Queries.GetGroups
{
    public class GetGroupsQueryHandler : IRequestHandler<GetGroupsQuery, List<GroupDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetGroupsQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<List<GroupDto>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            var groups = await _unitOfWork.Repository<Group>().Entities
                .Where(g => g.OwnerId == currentUserId && !g.IsDeleted)
                .Include(g => g.User)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    GroupName = g.GroupName,
                    OwnerId = g.OwnerId,
                    OwnerName = g.User.FullName,
                    MembersCount = g.Id == Guid.Empty ? 0 : 0, // Will be populated below
                    CreatedAt = g.CreatedAt,
                    UpdatedAt = g.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            // Get members count for each group
            var groupIds = groups.Select(g => g.Id).ToList();
            var memberCounts = await _unitOfWork.Repository<GroupMember>().Entities
                .Where(gm => groupIds.Contains(gm.GroupId))
                .GroupBy(gm => gm.GroupId)
                .Select(g => new { GroupId = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            // Update member counts
            foreach (var group in groups)
            {
                var count = memberCounts.FirstOrDefault(mc => mc.GroupId == group.Id);
                group.MembersCount = count?.Count ?? 0;
            }

            return groups;
        }
    }
}
