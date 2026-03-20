using BookLAB.Application.Features.Dashboard.Queries.GetDashboardStats;
using BookLAB.Application.Features.Dashboard.Queries.GetPendingRequests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLAB.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get dashboard statistics for Lab Manager
    /// </summary>
    /// <returns>Dashboard statistics including bookings, incidents, rooms, and users</returns>
    [HttpGet("stats")]
    [AllowAnonymous] // Temporary for testing - remove after verification
    [ProducesResponseType(typeof(DashboardStatsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardStats(CancellationToken cancellationToken)
    {
        var query = new GetDashboardStatsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get pending booking requests for Lab Manager to approve/reject
    /// </summary>
    /// <param name="limit">Maximum number of requests to return (default: 10)</param>
    /// <returns>List of pending booking requests</returns>
    [HttpGet("pending-requests")]
    [AllowAnonymous] // Temporary for testing - remove after verification
    [ProducesResponseType(typeof(List<PendingRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingRequests([FromQuery] int? limit, CancellationToken cancellationToken)
    {
        var query = new GetPendingRequestsQuery { Limit = limit };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
