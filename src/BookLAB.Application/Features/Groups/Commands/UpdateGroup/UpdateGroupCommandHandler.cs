using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Groups.Commands.UpdateGroup
{
    public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateGroupCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            // Get group
            var group = await _unitOfWork.Repository<Group>().GetByIdAsync(request.GroupId);
            if (group == null || group.IsDeleted)
                throw new NotFoundException("Nhóm không tồn tại");

            // Check authorization - only owner can update
            if (group.OwnerId != currentUserId)
                throw new ForbiddenException("Bạn không có quyền cập nhật nhóm này");

            // Check duplicate name if name is being changed
            if (group.GroupName != request.GroupName)
            {
                var existingGroup = await _unitOfWork.Repository<Group>().Entities
                    .FirstOrDefaultAsync(g => g.OwnerId == currentUserId
                        && g.GroupName == request.GroupName
                        && !g.IsDeleted
                        && g.Id != request.GroupId, cancellationToken);

                if (existingGroup != null)
                    throw new BusinessException("Nhóm với tên này đã tồn tại");
            }

            group.GroupName = request.GroupName;
            group.UpdatedAt = DateTimeOffset.UtcNow;
            group.UpdatedBy = currentUserId;

            _unitOfWork.Repository<Group>().Update(group);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
