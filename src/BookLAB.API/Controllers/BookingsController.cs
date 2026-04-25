using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Bookings.CheckConflict;
using BookLAB.Application.Features.Bookings.Commands.ApproveBooking;
using BookLAB.Application.Features.Bookings.Commands.CreateBooking;
using BookLAB.Application.Features.Bookings.Commands.DeleteCalendarEvent;
using BookLAB.Application.Features.Bookings.Commands.RejectBooking;
using BookLAB.Application.Features.Bookings.Commands.SyncToCalendar;
using BookLAB.Application.Features.Bookings.Commands.UpdateCalendarEvent;
using BookLAB.Application.Features.Bookings.Queries.GetBookings;
using BookLAB.Application.Features.Bookings.Queries.GetBookingStats;
using BookLAB.Application.Features.Bookings.Queries.GetPurposeTypes;
using BookLAB.Application.Features.Bookings.Queries.ViewBookingHistory;
using BookLAB.Application.Features.Bookings.Queries.ViewUncheckedBookingRequest;
using BookLAB.Application.Features.Schedules.Queries.AddSchedule;
using BookLAB.Domain.DTOs;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookLAB.Application.Features.Bookings.Commands.CancelBooking;
using BookLAB.Application.Features.Bookings.Queries.GetResolvedBooking;

namespace BookLAB.API.Controllers;

//[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingsController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public BookingsController(IMediator mediator,
        ILogger<BookingsController> logger,
        IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Sync an approved booking to Google Calendar
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Calendar Event ID</returns>
    [HttpPost("{id:guid}/sync-calendar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Lecturer")]
    public async Task<IActionResult> SyncToCalendar(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new SyncBookingToCalendarCommand(id);
            var eventId = await _mediator.Send(command, cancellationToken);

            return Ok(new
            {
                success = true,
                message = "Booking synced to Google Calendar successfully",
                calendarEventId = eventId,
                bookingId = id
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to sync booking {BookingId} to calendar", id);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing booking {BookingId} to calendar", id);
            return StatusCode(500, new { success = false, message = "An error occurred while syncing to calendar" });
        }
    }

    /// <summary>
    /// Update calendar event when booking is modified
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPut("{id:guid}/update-calendar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Lecturer")]
    public async Task<IActionResult> UpdateCalendarEvent(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdateCalendarEventCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update calendar event for booking {BookingId}", id);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating calendar event for booking {BookingId}", id);
            return StatusCode(500, new { success = false, message = "An error occurred while updating calendar event" });
        }
    }

    /// <summary>
    /// Delete calendar event when booking is cancelled
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpDelete("{id:guid}/delete-calendar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Lecturer")]
    public async Task<IActionResult> DeleteCalendarEvent(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeleteCalendarEventCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to delete calendar event for booking {BookingId}", id);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting calendar event for booking {BookingId}", id);
            return StatusCode(500, new { success = false, message = "An error occurred while deleting calendar event" });
        }
    }

    [HttpPut("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "AcademicOffice_LabManager")]
    public async Task<IActionResult> ApproveBooking(Guid id, [FromBody] ApproveBookingCommand command)
    {
        // Safety check: Ensure the ID in URL matches the ID in the command body
        if (id != command.BookingId)
        {
            return BadRequest("Booking ID mismatch between URL and Request Body.");
        }

        var result = await _mediator.Send(command);

        if (result)
        {
            return Ok(new ResultMessage<bool>
            {
                Success = true,
                Message = "Booking request processed successfully." 
            });
        }

        return BadRequest("Unable to process the booking approval. It might already be processed or cancelled.");
    }

    [HttpPut("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "AcademicOffice_LabManager")]
    public async Task<IActionResult> RejectBooking(Guid id, [FromBody] RejectBookingCommand command)
    {
        // Consistency check: The ID in URL must match the ID in the payload
        if (id != command.BookingId)
        {
            return BadRequest("Booking ID mismatch.");
        }

        // Dispatch the command to the RejectBookingCommandHandler
        var result = await _mediator.Send(command);

        if (result)
        {
            return Ok(new { Message = "Booking has been rejected and the student will be notified." });
        }

        return BadRequest("Unable to reject this booking. It may have been processed or already cancelled.");
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "Lecturer")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
    {
        // Execute command and get the resulting Guid
        var bookingId = await _mediator.Send(command);

        // If Guid is empty, it means the logic in Handler failed (e.g., overlapping schedule)
        if (bookingId == Guid.Empty)
        {
            return BadRequest(new { Message = "Unable to create booking. Room may be unavailable." });
        }

        // Return just the ID - Simple and Independent
        return Ok(new { Id = bookingId });
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<Application.Features.Bookings.Queries.GetBookings.BookingDto>), StatusCodes.Status200OK)]
    [Authorize(Policy = "AcademicOffice_LabManager_Lecturer")]
    public async Task<IActionResult> GetBookings([FromQuery] GetBookingsQuery query)
    {
        // The Specification logic is encapsulated inside the Handler
        // Frontend can pass: ?pageNumber=1&pageSize=10&orderBy=StartTime&filter=Status eq 1
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the booking history list for the current user.
    /// </summary>
    /// <param name="dto">
    /// The filter and pagination parameters passed via query string, including page, limit, status, startDate, and endDate.
    /// </param>
    /// <returns>
    /// Returns an HTTP 200 response with booking history data, total count, page, and limit.
    /// If an error occurs, returns a 500 Internal Server Error.
    /// </returns>
    [HttpGet("history")]
    [Authorize(Policy = "AcademicOffice_LabManager_Lecturer")]
    public async Task<IActionResult> GetBookingHistoryList([FromQuery] ViewBookingHistoryDTO dto)
    {
        try
        {
            // Try to parse the user Id from claims. If parsing fails, userId will be Guid.Empty.
            Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out var userId);

            // Build the command object to send through Mediator, collecting parameters from the DTO.
            ViewBookingHistoryCommand command = new ViewBookingHistoryCommand
            {
                userId = userId,
                page = dto.page,
                limit = dto.limit,
                status = dto.status.ToLower(),
                startDate = dto.startDate,
                endDate = dto.endDate,
                labRoomId = dto.labRoomId,
            };

            // Execute the command via Mediator to retrieve booking history.
            var result = await _mediator.Send(command);

            var username = string.Empty;

            // Initialize the response DTO array with the same size as the result count.
            ViewBookingHistoryResponseDTO[] data = new ViewBookingHistoryResponseDTO[result.Count];

            // Loop through each booking record to map it into the response DTO.
            for (int i = 0; i < result.Count; i++)
            {
                // Fetch the user who created the booking. 
                // ⚠️ This can cause N+1 query problem since it queries the database inside a loop.
                username = (await _unitOfWork.Repository<User>().GetByIdAsync(result[i].CreatedBy)).FullName;

                // Map entity fields into the response DTO.
                data[i] = new ViewBookingHistoryResponseDTO
                {
                    id = result[i].Id.ToString(),
                    roomId = result[i].LabRoomId.ToString(),
                    roomName = result[i].LabRoom.RoomName,
                    buildingName = result[i].LabRoom.Building.BuildingName,
                    startTime = result[i].StartTime.ToString("HH:mm"),
                    endTime = result[i].EndTime.ToString("HH:mm"),
                    date = result[i].StartTime.ToString("yyyy-MM-dd"),
                    status = result[i].BookingStatus.ToString(),
                    purpose = result[i].PurposeType.PurposeName,
                    reason = result[i].Reason,
                    userName = username,
                    createdAt = result[i].CreatedAt
                };
            }

            // Return the response as JSON with pagination info.
            return Ok(new
            {
                data = data,
                total = result.Count,
                page = dto.page,
                limit = dto.limit
            });
        }
        catch (Exception ex)
        {
            // Return a standardized 500 Internal Server Error response if something goes wrong.
            return Problem("An internal server error occurred", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Retrieves booking statistics for the current user within a specified date range.
    /// </summary>
    /// <param name="dto">
    /// The request DTO containing startDate and endDate as query parameters.
    /// These values are parsed into DateTimeOffset objects.
    /// </param>
    /// <returns>
    /// Returns an HTTP 200 response with booking statistics if successful.
    /// Returns a 500 Internal Server Error if an exception occurs.
    /// </returns>
    [HttpGet("stats")]
    [Authorize(Policy = "Lecturer")]
    public async Task<IActionResult> GetBookingStats([FromQuery] GetBookingStatsRequestDTO dto)
    {
        try
        {
            // Safely parse startDate and endDate from the query DTO.
            // If parsing fails, the variables will be default(DateTimeOffset).
            DateTimeOffset.TryParse(dto.startDate, out var startDate);
            DateTimeOffset.TryParse(dto.endDate, out var endDate);

            var userId = HttpContext.User.FindFirst("Id").Value;

            // Build the command object to send through Mediator.
            GetBookingStatsCommand command = new GetBookingStatsCommand
            {
                userId = userId, // Retrieve user Id from claims.
                startDate = startDate.ToUniversalTime(),         // Convert to UTC for consistency.
                endDate = endDate.ToUniversalTime(),
            };

            // Execute the command via Mediator and return the result.
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Return a standardized 500 Internal Server Error response if something goes wrong.
            return Problem("An internal server error occurred", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
    [HttpGet("get-unchecked-booking-request")]
    [Authorize(Policy = "AcademicOffice_LabManager")]
    public async Task<IActionResult> GetUncheckedBookingRequestList([FromQuery] ViewUncheckedBookingRequestCommand command)
    {
        try
        {
            Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out var userId);

            command.userId = userId;

            var result = await _mediator.Send(command);

            return Ok(new
            {
                data = result.list,
                total = result.total,
                page = command.page,
                limit = command.limit
            });
        }
        catch (Exception ex)
        {
            return Problem("Something is wrong");
        }

    }

    [HttpGet("history/labmanager")]
    [Authorize(Policy = "AcademicOffice_LabManager")]
    public async Task<IActionResult> GetResolvedBooking([FromQuery] GetResolvedBookingQuery query)
    {
        try
        {
            query.userId = Guid.Parse(User.FindFirst("Id").Value);
            var result = await _mediator.Send(query);
            return Ok(new
            {
                data = result.List,
                total = result.total,
                page = query.page,
                limit = query.limit
            });

        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("check-conflict")]
    [Authorize(Policy = "AcademicOffice_LabManager_Lecturer")]
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
    [Authorize(Policy = "AcademicOffice")]
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


    [HttpGet("purposes")] // api/bookings/purposes
    [Authorize(Policy = "AcademicOffice_LabManager_Lecturer")]
    public async Task<ActionResult<PagedList<PurposeTypeDto>>> GetPurposes([FromQuery] GetPurposeTypesQuery query)
    {
        return Ok(await _mediator.Send(query));
    }

    [HttpPost("cancel/{id:guid}")]
    [Authorize(Policy = "Lecturer")]
    public async Task<IActionResult> CancelBooking([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CancelBookingCommand { BookingId = id };
            var result = await _mediator.Send(command, cancellationToken);

            if (result.Success)
                return Ok(new { success = true, message = result.Message });
            return Conflict(new { success = false, message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while canceling booking with Id {BookingId}", id);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }
}