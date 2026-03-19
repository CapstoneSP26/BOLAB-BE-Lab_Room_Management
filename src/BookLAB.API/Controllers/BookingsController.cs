using BookLAB.Application.Features.Bookings;
using BookLAB.Application.Features.Bookings.Queries.ViewUncheckedBookingRequest;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Common.Security;
using BookLAB.Application.Features.Bookings.Commands.ApproveBooking;
using BookLAB.Application.Features.Bookings.Commands.CreateBooking;
using BookLAB.Application.Features.Bookings.Commands.DeleteCalendarEvent;
using BookLAB.Application.Features.Bookings.Commands.RejectBooking;
using BookLAB.Application.Features.Bookings.Commands.SyncToCalendar;
using BookLAB.Application.Features.Bookings.Commands.UpdateCalendarEvent;
using BookLAB.Application.Features.Bookings.Queries.GetBookings;
using BookLAB.Application.Features.Bookings.Queries.GetBookingStats;
using BookLAB.Application.Features.Bookings.Queries.ViewBookingHistory;
using BookLAB.Domain.DTOs;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace BookLAB.API.Controllers;

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
            return Ok(new { Message = "Booking request processed successfully." });
        }

        return BadRequest("Unable to process the booking approval. It might already be processed or cancelled.");
    }

    [HttpPut("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [HttpGet("get-booking-history")]
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
                status = dto.status,
                startDate = dto.startDate,
                endDate = dto.endDate,
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
                    userName = username
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
    [HttpGet("get-booking-stats")]
    public async Task<IActionResult> GetBookingStats([FromQuery] GetBookingStatsRequestDTO dto)
    {
        try
        {
            // Safely parse startDate and endDate from the query DTO.
            // If parsing fails, the variables will be default(DateTimeOffset).
            DateTimeOffset.TryParse(dto.startDate, out var startDate);
            DateTimeOffset.TryParse(dto.endDate, out var endDate);

            // Build the command object to send through Mediator.
            GetBookingStatsCommand command = new GetBookingStatsCommand
            {
                userId = HttpContext.User.FindFirst("Id").Value, // Retrieve user Id from claims.
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
    public async Task<List<BookingRequest>> GetUncheckedBookingRequestList()
    {
        ViewUncheckedBookingRequestCommand command = new ViewUncheckedBookingRequestCommand
        {
            userId = HttpContext.User.FindFirst("Id")?.Value ?? "11111111-1111-1111-1111-111111111111"
        };

        var result = await _mediator.Send(command);

        return result;
    }
}