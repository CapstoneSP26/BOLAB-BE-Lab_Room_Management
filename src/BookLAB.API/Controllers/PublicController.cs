using BookLAB.Application.Features.Schedules.Queries.GetSchedules;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookLAB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PublicController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("calendar/labroom")]
        public async Task<IActionResult> GetLabRoomSchedules([FromQuery] GetSchedulesQuery query)
        {
            query.ExcludedStatus = ScheduleStatus.Cancelled;
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
