using Microsoft.AspNetCore.Mvc;
using MediatR;
using BookLAB.Application.Features.LabRooms.Queries.GetLabRoomPolicies;

namespace BookLAB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LabRoomController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LabRoomController> _logger;

        public LabRoomController(IMediator mediator, ILogger<LabRoomController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}/policies")]
        public async Task<IActionResult> GetPolicies(int id)
        {
            var query = new GetLabRoomPoliciesQuery { LabRoomId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

    }
}
