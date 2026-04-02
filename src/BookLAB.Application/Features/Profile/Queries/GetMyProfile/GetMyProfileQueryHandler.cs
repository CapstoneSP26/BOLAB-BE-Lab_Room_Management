using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Profile.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLAB.Application.Features.Profile.Queries.GetMyProfile;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, MyProfileDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetMyProfileQueryHandler> _logger;

    public GetMyProfileQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<GetMyProfileQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<MyProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId
            ?? throw new BusinessException("User is not authenticated.");

        var user = await _unitOfWork.Repository<User>().Entities
            .Include(u => u.Campus)
            .FirstOrDefaultAsync(u => u.Id == currentUserId && !u.IsDeleted && u.IsActive, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        var userRoles = await _unitOfWork.Repository<UserRole>().Entities
            .Where(ur => ur.UserId == currentUserId)
            .Include(ur => ur.Role)
            .ToListAsync(cancellationToken);

        var result = new MyProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            UserCode = user.UserCode,
            UserImageUrl = user.UserImageUrl,
            Role = userRoles.FirstOrDefault()?.Role?.RoleName,
            AvatarUrl = user.UserImageUrl,
            CampusId = user.CampusId,
            CreatedAt = user.CreatedAt.DateTime,
            UpdatedAt = user.UpdatedAt?.DateTime
        };

        _logger.LogInformation($"MyProfile retrieved - UserId: {user.Id}, UserImageUrl: '{result.UserImageUrl}'");

        return result;
    }
}
