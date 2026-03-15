using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Bookings.Commands.ApproveBooking;
using BookLAB.Application.Features.Bookings.Commands.CreateBooking;
using BookLAB.Application.Features.Bookings.Commands.DeleteCalendarEvent;
using BookLAB.Application.Features.Bookings.Commands.RejectBooking;
using BookLAB.Application.Features.Bookings.Commands.SyncToCalendar;
using BookLAB.Application.Features.Bookings.Commands.UpdateCalendarEvent;
using BookLAB.Application.Features.Bookings.Queries.GetBookings;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Numerics;

namespace BookLAB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IMediator mediator, ILogger<BookingsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
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
    [ProducesResponseType(typeof(PagedList<BookingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBookings([FromQuery] GetBookingsQuery query)
    {
        // The Specification logic is encapsulated inside the Handler
        // Frontend can pass: ?pageNumber=1&pageSize=10&orderBy=StartTime&filter=Status eq 1
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
