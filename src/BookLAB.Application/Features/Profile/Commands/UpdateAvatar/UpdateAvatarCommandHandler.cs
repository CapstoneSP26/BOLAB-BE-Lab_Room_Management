using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Profile.Commands.UpdateAvatar;

public class UpdateAvatarCommandHandler : IRequestHandler<UpdateAvatarCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAvatarCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<string> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId
            ?? throw new BusinessException("User is not authenticated.");

        var user = await _unitOfWork.Repository<User>().Entities
            .FirstOrDefaultAsync(u => u.Id == currentUserId && !u.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        // TODO: Implement actual file upload logic
        // For now, return a placeholder URL
        var avatarUrl = $"/api/files/avatars/{currentUserId}.jpg";

        user.UserImageUrl = avatarUrl;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return avatarUrl;
    }
}
