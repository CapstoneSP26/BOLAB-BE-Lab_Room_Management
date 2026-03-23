using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Groups.Queries.GetGroupById
{
    public class GetGroupByIdQueryHandler : IRequestHandler<GetGroupByIdQuery, GroupDetailDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetGroupByIdQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<GroupDetailDto> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            var group = await _unitOfWork.Repository<Group>().Entities
                .Where(g => g.Id == request.GroupId && !g.IsDeleted)
                .Include(g => g.User)
                .FirstOrDefaultAsync(cancellationToken);

            if (group == null)
                throw new NotFoundException("Nhóm không tồn tại");

            // Check authorization - owner can view their group
            if (group.OwnerId != currentUserId)
                throw new ForbiddenException("Bạn không có quyền xem nhóm này");

            // Get members count
            var membersCount = await _unitOfWork.Repository<GroupMember>().Entities
                .Where(gm => gm.GroupId == request.GroupId)
                .CountAsync(cancellationToken);

            return new GroupDetailDto
            {
                Id = group.Id,
                GroupName = group.GroupName,
                OwnerId = group.OwnerId,
                OwnerName = group.User.FullName,
                MembersCount = membersCount,
                CreatedAt = group.CreatedAt,
                UpdatedAt = group.UpdatedAt
            };
        }
    }
}
