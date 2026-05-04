using AutoMapper;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Groups.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Groups.Queries.GetGroupByName
{
    public class GetGroupByNameHandler : IRequestHandler<GetGroupByNameQuery, List<GroupDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetGroupByNameHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<List<GroupDto>> Handle(GetGroupByNameQuery request, CancellationToken cancellationToken)
        {
            var groups = await _unitOfWork.Repository<Group>().Entities
                .Include(x => x.User)
                .Where(x => x.GroupName.ToLower().Contains(request.GroupName.ToLower())).ToListAsync(cancellationToken);
            var groupDtos = _mapper.Map<List<GroupDto>>(groups);
            return groupDtos;
        }
    }
}
