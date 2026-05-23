using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Groups.DTOs;
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

            var groups = await _unitOfWork.Repository<GroupMember>().Entities
                .Include(x => x.Group)
                .Where(g => g.Group.OwnerId == currentUserId && !g.Group.IsDeleted)
                .Include(g => g.Group.User)
                .Select(g => new GroupDto
                {
                    Id = g.Group.Id,
                    GroupName = g.Group.GroupName,
                    OwnerId = g.Group.OwnerId,
                    OwnerName = g.Group.User.FullName,
                    MembersCount = g.Group.Id == Guid.Empty ? 0 : 0, // Will be populated below
                    CreatedAt = g.Group.CreatedAt,
                    UpdatedAt = g.Group.UpdatedAt,
                    SubjectCode = g.SubjectCode
                })
                .Distinct()
                .OrderBy(x => x.GroupName)
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
