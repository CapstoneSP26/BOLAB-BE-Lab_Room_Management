using BookLAB.Application.Features.Dashboard.Queries.GetDashboardStats;
using BookLAB.Application.Features.Dashboard.Queries.GetDashboardOverview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLAB.API.Controllers
{
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

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetDashboardStatsQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("monthly-bookings")]
        public async Task<IActionResult> GetMonthlyBookings(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetDashboardStatsQuery(), cancellationToken);
            return Ok(new
            {
                year = result.year,
                monthlyBookings = result.monthlyBookings
            });
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetDashboardStatsQuery(), cancellationToken);
            return Ok(new
            {
                year = result.year,
                statistics = result.statistics
            });
        }

        [HttpGet("overview")]
        [Authorize(Policy = "AcademicOffice_LabManager")]
        public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("Id")?.Value;
            var role = User.FindFirst("Role")?.Value ?? string.Empty;
            var userId = Guid.TryParse(userIdClaim, out var parsedUserId) ? parsedUserId : (Guid?)null;

            var result = await _mediator.Send(new GetDashboardOverviewQuery
            {
                UserId = userId,
                Role = role
            }, cancellationToken);

            return Ok(result);
        }
    }
}
