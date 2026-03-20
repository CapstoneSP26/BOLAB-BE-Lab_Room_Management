using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Users.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetUserProfileQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId ?? Guid.Empty;

            var user = await _unitOfWork.Repository<User>().Entities
                .Include(u => u.Campus)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);

            if (user == null)
                throw new InvalidOperationException("User not found or deleted");

            var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();

            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                UserCode = user.UserCode,
                CampusName = user.Campus?.CampusName ?? "Unknown",
                Roles = roles,
                CreatedAt = user.CreatedAt.DateTime,
                UpdatedAt = user.UpdatedAt?.DateTime,
                IsActive = user.IsActive
            };
        }
    }
}
