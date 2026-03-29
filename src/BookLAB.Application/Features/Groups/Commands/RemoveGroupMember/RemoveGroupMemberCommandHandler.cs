using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Groups.Commands.RemoveGroupMember
{
    public class RemoveGroupMemberCommandHandler : IRequestHandler<RemoveGroupMemberCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public RemoveGroupMemberCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(RemoveGroupMemberCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            // Verify group exists and belongs to current user
            var group = await _unitOfWork.Repository<Group>().GetByIdAsync(request.GroupId);
            if (group == null || group.IsDeleted)
                throw new NotFoundException("Nhóm không tồn tại");

            if (group.OwnerId != currentUserId)
                throw new ForbiddenException("Bạn không có quyền xóa thành viên khỏi nhóm này");

            // Get member
            var member = await _unitOfWork.Repository<GroupMember>().Entities
                .FirstOrDefaultAsync(gm => gm.GroupId == request.GroupId && gm.UserId == request.UserId, cancellationToken);

            if (member == null)
                throw new NotFoundException("Sinh viên không có trong nhóm");

            _unitOfWork.Repository<GroupMember>().Delete(member);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
