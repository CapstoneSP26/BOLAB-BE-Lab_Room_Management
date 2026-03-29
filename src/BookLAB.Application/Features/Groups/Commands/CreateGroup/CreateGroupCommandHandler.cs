using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Groups.Commands.CreateGroup
{
    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateGroupCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            // Check if user exists
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(currentUserId);
            if (user == null)
                throw new NotFoundException("Người dùng không tồn tại");

            // Check duplicate group name for the same owner
            var existingGroup = await _unitOfWork.Repository<Group>().Entities
                .FirstOrDefaultAsync(g => g.OwnerId == currentUserId 
                    && g.GroupName == request.GroupName 
                    && !g.IsDeleted, cancellationToken);

            if (existingGroup != null)
                throw new BusinessException("Nhóm với tên này đã tồn tại trong danh sách của bạn");

            var group = new Group
            {
                Id = Guid.NewGuid(),
                GroupName = request.GroupName,
                OwnerId = currentUserId,
                IsDeleted = false,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = currentUserId
            };

            await _unitOfWork.Repository<Group>().AddAsync(group);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return group.Id;
        }
    }
}
