using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.LabRooms.Queries.GetLabRoomPolicies;
using BookLAB.Application.Features.LabRooms.Queries.GetLabRooms;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLAB.API.Controllers
{
    [Authorize]
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
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice_LabManager")]
        public async Task<IActionResult> GetPolicies(int id)
        {
            var query = new GetLabRoomPoliciesQuery { LabRoomId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedList<LabRoomDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice_LabManager")]
        public async Task<IActionResult> GetLabRooms([FromQuery] GetLabRoomsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

    }

}
