using BookLAB.Application.Features.Users.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLAB.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get current user profile information
    /// </summary>
    /// <returns>Current authenticated user profile</returns>
    [HttpGet("me")]
    [AllowAnonymous] // Temporary for testing - remove after verification
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var query = new GetUserProfileQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
