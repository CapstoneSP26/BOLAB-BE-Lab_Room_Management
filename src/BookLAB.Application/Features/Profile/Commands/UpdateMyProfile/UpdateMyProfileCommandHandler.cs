using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Profile.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Profile.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, MyProfileDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateMyProfileCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<MyProfileDto> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId
            ?? throw new BusinessException("User is not authenticated.");

        var user = await _unitOfWork.Repository<User>().Entities
            .Include(u => u.Campus)
            .FirstOrDefaultAsync(u => u.Id == currentUserId && !u.IsDeleted && u.IsActive, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(request.FullName))
            user.FullName = request.FullName;

        if (!string.IsNullOrWhiteSpace(request.Email))
            user.Email = request.Email;

        user.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Get updated roles
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

        return result;
    }
}
