using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using AutoMapper;
using MediatR;

namespace BookLAB.Application.Features.Users.Queries.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserProfileDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetCurrentUserQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserProfileDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            // Fetch user with roles and campus information
            var user = await _userRepository.GetByIdWithRolesAsync(request.UserId);

            if (user == null)
            {
                throw new NotFoundException("User", request.UserId.ToString());
            }

            if (user.IsDeleted)
            {
                throw new NotFoundException("User", request.UserId.ToString());
            }

            // Map User entity to UserProfileDto
            var profileDto = _mapper.Map<UserProfileDto>(user);

            return profileDto;
        }
    }
}
