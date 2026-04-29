using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Buildings.DTOs;
using BookLAB.Application.Features.Buildings.Queries.GetBuildingByName;
using BookLAB.Application.Features.Buildings.Queries.GetBuildings;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookLAB.Application.Features.Buildings.Commands.CreateBuildings;
using BookLAB.Application.Features.Buildings.Commands.UpdateBuildings;
using BookLAB.Application.Features.Buildings.Commands.DeleteBuildings;

namespace BookLAB.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuildingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BuildingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get building information by building name
        /// </summary>
        /// <param name="buildingName">The name of the building to retrieve</param>
        /// <returns>Building information if found, otherwise 404 Not Found</returns>
        [HttpGet("{buildingName}")]
        [ProducesResponseType(typeof(BuildingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBuildingByName(string buildingName, CancellationToken cancellationToken)
        {
            var query = new GetBuildingByNameQuery { BuildingName = buildingName };
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound(new { message = $"Building '{buildingName}' not found" });

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<BuildingDto>>> GetBuildings([FromQuery] GetBuildingsQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpPost]
        public async Task<ActionResult<PagedList<BuildingDto>>> CreateBuildings([FromForm] CreateBuildingsCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PagedList<BuildingDto>>> UpdateBuildings([FromForm] UpdateBuildingsCommand command, [FromRoute] int id)
        {
            if (command.Id != id)
                return BadRequest(new { message = "ID in the route does not match ID in the command" });

            var result = await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteBuildings([FromRoute] int id, [FromForm] DeleteBuildingsCommand command)
        {
            if (command.Id != id)
                return BadRequest(new { message = "ID in the route does not match ID in the command" });

            var result = await _mediator.Send(command); 

            return Ok();
        }
    }
}