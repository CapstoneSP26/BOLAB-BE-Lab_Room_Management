using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Groups.Queries.GetGroupMembers
{
    public class GetGroupMembersQueryHandler : IRequestHandler<GetGroupMembersQuery, List<GroupMemberDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetGroupMembersQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<List<GroupMemberDto>> Handle(GetGroupMembersQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            // Verify group exists and belongs to current user
            var group = await _unitOfWork.Repository<Group>().GetByIdAsync(request.GroupId);
            if (group == null || group.IsDeleted)
                throw new NotFoundException("Nhóm không tồn tại");

            if (group.OwnerId != currentUserId)
                throw new ForbiddenException("Bạn không có quyền xem thành viên của nhóm này");

            IQueryable<GroupMember> members = _unitOfWork.Repository<GroupMember>().Entities;

            if (request.SubjectCode != null && request.SubjectCode != "")
                members = members.Where(x => x.SubjectCode == request.SubjectCode);

            var membersQuery = await members
                .Where(gm => gm.GroupId == request.GroupId)
                .Include(gm => gm.User)
                .Select(gm => new GroupMemberDto
                {
                    UserId = gm.UserId,
                    UserName = gm.User.FullName,
                    UserEmail = gm.User.Email,
                    UserCode = gm.User.UserCode,
                    SubjectCode = gm.SubjectCode
                })
                .ToListAsync(cancellationToken);

            return membersQuery;
        }
    }
}
