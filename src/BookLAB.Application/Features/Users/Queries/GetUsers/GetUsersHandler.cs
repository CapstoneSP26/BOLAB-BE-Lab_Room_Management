using AutoMapper;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Users.Queries.GetUsers
{
    public class GetUsersHandler : IRequestHandler<GetUsersQuery, List<UserProfileDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetUsersHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<List<UserProfileDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var campusId = _currentUserService.CampusId;
                int? role = null;

                if (request.role != null)
                    role = request.role;

                var userRoles = await _unitOfWork.Repository<User>().Entities
                    .ToListAsync();

                var usersQuery = _unitOfWork.Repository<User>().Entities
                    .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .Where(x => x.CampusId == campusId &&
                        (x.FullName.ToLower().Contains(request.keyword.ToLower()) ||
                        x.Email.ToLower().Contains(request.keyword.ToLower()) ||
                        x.UserCode.ToLower().Contains(request.keyword.ToLower())));

                if (role != null)
                    usersQuery = usersQuery.Where(x => x.UserRoles.Any(x => x.RoleId == role));

                var users = await usersQuery.ToListAsync();

                var mappedUsers = _mapper.Map<List<User>, List<UserProfileDto>>(users);
                return mappedUsers;
            } catch (Exception ex)
            {
                return new List<UserProfileDto>();
            }
        }
    }
}
