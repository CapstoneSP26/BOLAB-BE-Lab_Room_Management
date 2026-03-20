using BookLAB.Application.Features.IncidentReports.Queries.GetUnresolvedIncidents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLAB.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IncidentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public IncidentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of unresolved incidents/reports for Lab Manager dashboard
    /// </summary>
    /// <param name="limit">Maximum number of incidents to return (default: 10)</param>
    /// <returns>List of unresolved incidents</returns>
    [HttpGet("unresolved")]
    [AllowAnonymous] // Temporary for testing - remove after verification
    [ProducesResponseType(typeof(List<IncidentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnresolvedIncidents([FromQuery] int? limit, CancellationToken cancellationToken)
    {
        var query = new GetUnresolvedIncidentsQuery { Limit = limit };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
