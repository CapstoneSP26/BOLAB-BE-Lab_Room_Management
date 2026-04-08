using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Profile.Commands.ChangePassword;
using BookLAB.Application.Features.Profile.Commands.MarkNotificationAsRead;
using BookLAB.Application.Features.Profile.Commands.UpdateAvatar;
using BookLAB.Application.Features.Profile.Commands.UpdateMyProfile;
using BookLAB.Application.Features.Profile.DTOs;
using BookLAB.Application.Features.Profile.Queries.GetMyNotifications;
using BookLAB.Application.Features.Profile.Queries.GetMyProfile;
using BookLAB.Application.Features.Profile.Queries.GetProfileStatistics;
using BookLAB.Application.Features.Profile.Queries.GetRecentActivities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLAB.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<MyProfileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyProfileQuery(), cancellationToken);
        return Ok(new ApiResponse<MyProfileDto> { Data = result });
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(ApiResponse<MyProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(new ApiResponse<MyProfileDto> { Data = result, Message = "Profile updated successfully" });
    }

    [HttpPut("change-password")]
    [ProducesResponseType(typeof(ApiResponse<dynamic>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return Ok(new ApiResponse<dynamic> { Message = "Password changed successfully" });
    }

    [HttpPost("avatar")]
    [ProducesResponseType(typeof(ApiResponse<dynamic>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAvatar([FromForm] IFormFile avatar, CancellationToken cancellationToken)
    {
        if (avatar == null || avatar.Length == 0)
            return BadRequest(new { message = "Avatar file is required" });

        var command = new UpdateAvatarCommand 
        { 
            Avatar = avatar 
        };

        var avatarUrl = await _mediator.Send(command, cancellationToken);
        return Ok(new ApiResponse<dynamic> { Data = new { avatarUrl } });
    }

    [HttpGet("notifications")]
    [ProducesResponseType(typeof(ApiResponse<PagedList<NotificationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyNotifications([FromQuery] GetMyNotificationsQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(new ApiResponse<PagedList<NotificationDto>> { Data = result });
    }

    [HttpPut("notifications/{notificationId:int}/read")]
    [ProducesResponseType(typeof(ApiResponse<dynamic>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkNotificationAsRead(int notificationId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new MarkNotificationAsReadCommand { NotificationId = notificationId }, cancellationToken);
        return Ok(new ApiResponse<dynamic> { Message = "Notification marked as read" });
    }

    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<ProfileStatisticsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfileStatistics(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProfileStatisticsQuery(), cancellationToken);
        return Ok(new ApiResponse<ProfileStatisticsDto> { Data = result });
    }

    [HttpGet("recent-activities")]
    [ProducesResponseType(typeof(ApiResponse<List<RecentActivityDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentActivities([FromQuery] int limit = 10, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetRecentActivitiesQuery { Limit = limit }, cancellationToken);
        return Ok(new ApiResponse<List<RecentActivityDto>> { Data = result });
    }
}
