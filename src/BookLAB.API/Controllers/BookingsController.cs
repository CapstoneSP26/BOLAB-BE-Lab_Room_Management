using BookLAB.Application.Features.Bookings.Commands.SyncToCalendar;
using BookLAB.Application.Features.Bookings.Commands.UpdateCalendarEvent;
using BookLAB.Application.Features.Bookings.Commands.DeleteCalendarEvent;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
}
