using BookLAB.Api.Controllers;
using BookLAB.Application.Features.Bookings.Queries.ViewBookingHistory;
using BookLAB.Application.Features.IncidentReports.Queries.GetReportedReport;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookLAB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IMediator mediator, ILogger<ReportsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        [HttpGet("get-incident-reports")]
        public async Task<IActionResult> GetReportedReportAsync()
        {
            try
            {
                Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out var userId);

                GetReportedReportCommand command = new GetReportedReportCommand
                {
                    userId = userId
                };

                var result = await _mediator.Send(command);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Problem("Something is wrong");
            }

        }
    }
}
