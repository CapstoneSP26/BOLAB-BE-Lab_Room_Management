using AutoMapper;
using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Groups.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Groups.Queries.GetGroupById
{
    public class GetGroupByIdQueryHandler : IRequestHandler<GetGroupByIdQuery, GroupDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetGroupByIdQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<GroupDto> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            var group = await _unitOfWork.Repository<Group>().Entities
                .Where(g => g.Id == request.GroupId && !g.IsDeleted)
                .Include(g => g.User)
                .FirstOrDefaultAsync(cancellationToken);

            if (group == null)
                throw new NotFoundException("Nhóm không tồn tại");

            // Check authorization - owner can view their group
            if (group.OwnerId != currentUserId)
                throw new ForbiddenException("Bạn không có quyền xem nhóm này");

            // Get members count
            var membersCount = await _unitOfWork.Repository<GroupMember>().Entities
                .Where(gm => gm.GroupId == request.GroupId)
                .CountAsync(cancellationToken);

            // Map entity to DTO
            var dto = _mapper.Map<GroupDto>(group);
            dto.MembersCount = membersCount;

            return dto;
        }
    }
}
