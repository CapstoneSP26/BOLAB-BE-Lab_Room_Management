using BookLAB.Application.Features.SlotTypes.GetSlotTypes;
using BookLAB.Application.Features.SlotTypes.Queries.GetSlotTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLAB.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SlotTypesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SlotTypesController> _logger;

        public SlotTypesController(IMediator mediator, ILogger<SlotTypesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<SlotTypeDto>>> GetSlotTypes([FromQuery] int? campusId)
        {
            var query = new GetSlotTypesQuery { CampusId = campusId };
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
