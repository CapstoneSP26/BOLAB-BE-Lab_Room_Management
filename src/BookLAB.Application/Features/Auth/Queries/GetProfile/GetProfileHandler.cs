using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Auth.Queries.GetProfile
{
    public class GetProfileHandler : IRequestHandler<GetProfileQuery, UserProfileDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetProfileHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<UserProfileDto> Handle(GetProfileQuery request, CancellationToken ct)
        {
            try
            {
                var userId = _currentUserService.UserId;
                if (userId == null) throw new UnauthorizedAccessException();

                // Lấy thông tin User kèm theo Roles (Giả định bạn có quan hệ UserRoles)
                var user = await _unitOfWork.Repository<User>().Entities
                    .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId, ct);

                if (user == null) throw new NotFoundException("User not found");

                return new UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                    CampusId = user.CampusId,
                    AvatarUrl = user.UserImageUrl,
                };
            } catch (Exception ex)
            {
                Console.WriteLine("Havent login");
                return new UserProfileDto();
            }
            
        }
    }
}
