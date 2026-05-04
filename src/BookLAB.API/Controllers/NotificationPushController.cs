using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Profile.DTOs;
using BookLAB.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace BookLAB.API.Controllers;

[Authorize]
[ApiController]
[Route("api/booking-notifications")]
public class NotificationPushController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationPushController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("{bookingId:guid}")]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatestBookingNotification(Guid bookingId, CancellationToken cancellationToken)
    {
        // PostgreSQL yêu cầu so sánh chuỗi (text)
        var bookingIdStr = bookingId.ToString();

        var notification = await _unitOfWork.Repository<Notification>().Entities
            .Where(x => x.Metadata.GetProperty("bookingId").GetString() == bookingIdStr)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new NotificationDto
            {
                Id = x.Id,
                UserId = x.UserId,
                Title = x.Title,
                Message = x.Message,
                Type = x.Type,
                IsRead = x.IsRead,
                CreatedAt = x.CreatedAt,
                ReadAt = x.ReadAt,
                Metadata = x.Metadata
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (notification == null)
            return NotFound();

        return Ok(notification);
    }
}
