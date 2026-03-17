using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Commands.ImportSchedule;
using BookLAB.Application.Features.Schedules.Commands.ValidateImport;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Application.Features.Bookings.AddSchedule;
using BookLAB.Application.Features.Bookings.CheckConflict;
using BookLAB.Domain.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BookLAB.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SchedulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Step 1: Validates the uploaded Excel file and returns a preview with potential errors
    /// </summary>
    /// <param name="file">The Excel file containing schedule data</param>
    /// <returns>A list of validated rows with status (Valid/Invalid) and error messages</returns>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ImportValidationResult<ScheduleImportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateSchedules([FromBody] ValidateImportQuery query)
    {
        // MediatR dispatches to ValidateImportHandler
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Step 2: Officially imports the validated data into the database
    /// </summary>
    /// <param name="command">The list of confirmed schedule items to be saved</param>
    /// <returns>A summary of the import operation (Total success/failure)</returns>
    [HttpPost("import")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmImport([FromBody] ConfirmImportCommand command)
    {
        // MediatR dispatches to ConfirmImportHandler (using AddRangeAsync logic)
        var result = await _mediator.Send(command);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}