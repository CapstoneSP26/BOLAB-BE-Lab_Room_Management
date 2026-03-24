using BookLAB.Application.Features.Buildings.Queries.GetBuildingByName;
using BookLAB.Application.Features.Buildings.Queries.GetAllBuildings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        /// Lấy danh sách tất cả tòa nhà
        /// </summary>
        /// <returns>Danh sách tòa nhà với thông tin cơ bản</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<BuildingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllBuildings(CancellationToken cancellationToken)
        {
            var query = new GetAllBuildingsQuery();
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
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
    }
}