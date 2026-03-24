using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Groups.Commands.AddGroupMember
{
    public class AddGroupMemberCommandHandler : IRequestHandler<AddGroupMemberCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddGroupMemberCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(AddGroupMemberCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            // Verify group exists and belongs to current user
            var group = await _unitOfWork.Repository<Group>().GetByIdAsync(request.GroupId);
            if (group == null || group.IsDeleted)
                throw new NotFoundException("Nhóm không tồn tại");

            if (group.OwnerId != currentUserId)
                throw new ForbiddenException("Bạn không có quyền thêm thành viên vào nhóm này");

            // Verify user exists
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(request.UserId);
            if (user == null || user.IsDeleted)
                throw new NotFoundException("Sinh viên không tồn tại");

            // Check if member already exists
            var existingMember = await _unitOfWork.Repository<GroupMember>().Entities
                .FirstOrDefaultAsync(gm => gm.GroupId == request.GroupId && gm.UserId == request.UserId, cancellationToken);

            if (existingMember != null)
                throw new BusinessException("Sinh viên này đã có trong nhóm");

            var groupMember = new GroupMember
            {
                Id = Guid.NewGuid(),
                GroupId = request.GroupId,
                UserId = request.UserId,
                SubjectCode = request.SubjectCode
            };

            await _unitOfWork.Repository<GroupMember>().AddAsync(groupMember);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
