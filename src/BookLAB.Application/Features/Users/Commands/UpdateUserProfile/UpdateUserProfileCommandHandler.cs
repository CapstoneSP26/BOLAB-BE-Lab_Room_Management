using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using AutoMapper;
using MediatR;

namespace BookLAB.Application.Features.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserProfileDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateUserProfileCommandHandler(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<UserProfileDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            // Authorization: User can only update their own profile
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;
            if (currentUserId != request.UserId)
            {
                throw new ForbiddenException("You can only update your own profile");
            }

            // Fetch user
            var user = await _userRepository.GetByIdWithRolesAsync(request.UserId);
            if (user == null)
            {
                throw new NotFoundException("User", request.UserId.ToString());
            }

            if (user.IsDeleted)
            {
                throw new NotFoundException("User", request.UserId.ToString());
            }

            // Update fields
            user.FullName = request.FullName;
            user.UserImageUrl = request.UserImageUrl;
            user.UpdatedAt = DateTimeOffset.UtcNow;
            user.UpdatedBy = currentUserId;

            // Save changes
            await _userRepository.UpdateAsync(user);

            // Map and return updated profile
            var profileDto = _mapper.Map<UserProfileDto>(user);
            return profileDto;
        }
    }
}
