using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Commands.ImportSchedule;
using BookLAB.Application.Features.Schedules.Commands.ValidateImport;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Application.Features.Bookings.CheckConflict;
using BookLAB.Domain.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using BookLAB.Domain.Enums;
using BookLAB.Application.Features.Bookings.Commands.CreateBooking;
using BookLAB.Application.Features.Schedules.Queries.AddSchedule;

namespace BookLAB.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SchedulesController> _logger;

    public SchedulesController(IMediator mediator, ILogger<SchedulesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
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

    [HttpPost("check-conflict")]
    public async Task<IActionResult> CheckConflictAsync([FromBody] CreateBookingCommand booking)
    {
        if (booking == null) return Ok(new
        {
            success = false,
            mesage = "Invalid booking data"
        });

        CheckConflictCommand command = new CheckConflictCommand
        {
            booking = booking
        };

        var isConflict = await _mediator.Send(command);

        if (isConflict) return Ok(new
        {
            success = true,
            mesage = "No conflict"
        });

        return Ok(new
        {
            success = false,
            mesage = "Booking is conflict"
        });
    }

    /// <summary>
    /// Handles the HTTP POST request to add a new schedule.
    /// The method maps the incoming DTO to a Schedule entity, 
    /// sends an AddScheduleCommand through MediatR, 
    /// and returns an appropriate HTTP response based on the outcome.
    /// </summary>
    /// <param name="dtos">The schedule data transfer object containing input details.</param>
    /// <param name="cancellationToken">Token provided by ASP.NET Core to cancel the request if the client disconnects or times out.</param>
    /// <returns>
    /// An IActionResult indicating the result of the operation:
    /// - 200 OK with success message if the schedule was added successfully.
    /// - 409 Conflict if the schedule addition failed due to conflict or other business logic.
    /// - 401 Unauthorized if the user identity is invalid.
    /// - 500 Internal Server Error if an unexpected exception occurs.
    /// </returns>
    [HttpPost("add")]
    public async Task<IActionResult> AddScheduleAsync([FromBody] ScheduleDto dtos, CancellationToken cancellationToken)
    {
        try
        {
            // Extract the current user's Id from JWT claims
            if (!Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out Guid userId))
                return Unauthorized(new { success = false, message = "Invalid user identity" });

            // Map the incoming DTO to a Schedule entity
            var schedule = new Schedule
            {
                Id = Guid.NewGuid(),                        // Generate a new unique Id
                LecturerId = dtos.LecturerId,               // Assign lecturer
                LabRoomId = dtos.LabRoomId,                 // Assign lab room
                SlotTypeId = dtos.SlotTypeId,               // Assign slot type
                ScheduleType = dtos.ScheduleType,           // Set schedule type
                ScheduleStatus = ScheduleStatus.Active,     // Default status is Active
                StudentCount = dtos.StudentCount,           // Number of students
                StartTime = dtos.StartTime.ToUniversalTime(), // Normalize start time
                EndTime = dtos.EndTime.ToUniversalTime(),     // Normalize end time
                CreatedAt = DateTimeOffset.UtcNow,          // Timestamp creation
                CreatedBy = userId,                         // Track who created it
                IsActive = true,                            // Mark as active
                IsDeleted = false                           // Not deleted
            };

            // Wrap the schedule entity in a command object
            var command = new AddScheduleCommand { Schedule = schedule };

            // Send the command through MediatR pipeline
            var result = await _mediator.Send(command, cancellationToken);

            // Return success response if schedule was added successfully
            if (result)
                return Ok(new { success = true, message = "Schedule added successfully" });

            // Return conflict response if schedule addition failed
            return Conflict(new { success = false, message = "Schedule conflict or failed" });
        }
        catch (Exception ex)
        {
            // Log the error for debugging
            _logger.LogError(ex, "Error while adding schedule");

            // Return internal server error response
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }


}