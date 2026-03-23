using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;

namespace BookLAB.Application.Features.Groups.Commands.DeleteGroup
{
    public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteGroupCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            // Get group
            var group = await _unitOfWork.Repository<Group>().GetByIdAsync(request.GroupId);
            if (group == null || group.IsDeleted)
                throw new NotFoundException("Nhóm không tồn tại");

            // Check authorization - only owner can delete
            if (group.OwnerId != currentUserId)
                throw new ForbiddenException("Bạn không có quyền xóa nhóm này");

            // Soft delete the group
            group.IsDeleted = true;
            group.UpdatedAt = DateTimeOffset.UtcNow;
            group.UpdatedBy = currentUserId;

            _unitOfWork.Repository<Group>().Update(group);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
