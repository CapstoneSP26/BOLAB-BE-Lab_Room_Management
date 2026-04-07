using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Commands.ImportSchedule;
using BookLAB.Application.Features.Schedules.Commands.ValidateImport;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Application.Features.Schedules.Queries.GetSchedules;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BookLAB.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SchedulesController> _logger;
    private readonly ICurrentUserService _currentUserService;

    public SchedulesController(IMediator mediator, ILogger<SchedulesController> logger, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Step 1: Validates the uploaded Excel file and returns a preview with potential errors
    /// </summary>
    /// <param name="file">The Excel file containing schedule data</param>
    /// <returns>A list of validated rows with status (Valid/Invalid) and error messages</returns>
    [HttpPost("import/validate")]
    [ProducesResponseType(typeof(ImportValidationResult<ScheduleImportDto, Schedule>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "AcademicOffice")]
    public async Task<IActionResult> ValidateSchedules([FromBody] ValidateImportQuery query)
    {
        query.CampusId = _currentUserService.CampusId;
        // set index for each row (for error reporting)
        for (int i = 0; i < query.Schedules.Count; i++)
        {
            query.Schedules[i].Index = i + 1; // +1 to convert from 0-based to 1-based index
        }
        // MediatR dispatches to ValidateImportHandler
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Step 2: Officially imports the validated data into the database
    /// </summary>
    /// <param name="command">The list of confirmed schedule items to be saved</param>
    /// <returns>A summary of the import operation (Total success/failure)</returns>
    [HttpPost("import/commit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "AcademicOffice")]
    public async Task<IActionResult> ConfirmImport([FromBody] ConfirmImportCommand command)
    {
        // MediatR dispatches to ConfirmImportHandler (using AddRangeAsync logic)
        command.CampusId = _currentUserService.CampusId;
        var result = await _mediator.Send(command);

        if(result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    /// <summary>
    /// Step 1: Validates the uploaded Excel file and returns a preview with potential errors
    /// </summary>
    /// <param name="file">The Excel file containing schedule data</param>
    /// <returns>A list of validated rows with status (Valid/Invalid) and error messages</returns>
    [HttpPost("import/flexible-validate")]
    [ProducesResponseType(typeof(ImportValidationResult<ScheduleImportDto, Schedule>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateFlexibleSchedules([FromBody] ValidateFlexibleImportQuery query)
    {
        query.CampusId = _currentUserService.CampusId;
        // set index for each row (for error reporting)
        for (int i = 0; i < query.Schedules.Count; i++)
        {
            query.Schedules[i].Index = i + 1; // +1 to convert from 0-based to 1-based index
        }
        // MediatR dispatches to ValidateImportHandler
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Step 2: Officially imports the validated data into the database
    /// </summary>
    /// <param name="command">The list of confirmed schedule items to be saved</param>
    /// <returns>A summary of the import operation (Total success/failure)</returns>
    [HttpPost("import/flexible-commit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmFlexibleImport([FromBody] ConfirmFlexibleImportCommand command)
    {
        // MediatR dispatches to ConfirmImportHandler (using AddRangeAsync logic)
        command.CampusId = _currentUserService.CampusId;
        var result = await _mediator.Send(command);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpGet]
    [Authorize(Policy = "AcademicOffice_LabManager_Lecturer")]
    public async Task<IActionResult> GetSchedules([FromQuery] GetSchedulesQuery query)
    {
        // MediatR sẽ chuyển hướng query này đến GetSchedulesQueryHandler
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("schedule-attendance")]
    [Authorize(Policy = "AcademicOffice_LabManager_Lecturer")]
    public async Task<IActionResult> GetScheduleInAttendance([FromQuery] GetSchedulesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            // Validate user identity from claims
            if (!Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out Guid userId))
                return Unauthorized();


            GetSchedulesQuery command = new GetSchedulesQuery
            {
                SearchTerm = query.SearchTerm,
                Status = query.Status,
                LabRoomId = query.LabRoomId,
                FromDate = query.FromDate,
                ToDate = query.ToDate,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                SortBy = query.SortBy,
                IsDescending = query.IsDescending,
            };

            // Send the command through MediatR pipeline
            var result = await _mediator.Send(command, cancellationToken);

            // Return success response with the retrieved data
            return Ok(new
            {
                result = result
            });
        }
        catch (Exception ex)
        {
            // Log the error with details for debugging
            _logger.LogError(ex, "Something is wrong while getting unchecked booking requests: " + ex.Message);

            // Return internal server error response
            return Problem("Something is wrong while getting unchecked booking requests");
        }
    }
}