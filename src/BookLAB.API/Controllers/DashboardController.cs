using BookLAB.Application.Features.Dashboard.Queries.GetDashboardStats;
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
    }
}
