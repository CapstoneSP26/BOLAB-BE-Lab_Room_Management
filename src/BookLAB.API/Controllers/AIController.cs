using BookLAB.Application.Common.Models;
using BookLAB.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLAB.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly ILogger<AIController> _logger;
        private readonly IAIBookingService _aiBookingService;

        public AIController(ILogger<AIController> logger, IAIBookingService aiBookingService  ) 
        {
            _logger = logger;
            _aiBookingService = aiBookingService;
        }

        [HttpPost("parse-and-suggest")]
        public async Task<IActionResult> ParseAndSuggest(
            [FromBody] ParseAndSuggestRequest request,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.UserPrompt))
                return BadRequest(new { error = "User prompt cannot be empty" });

            try
            {
                var response = await _aiBookingService.ParseAndSuggestAsync(request.UserPrompt, ct);

                return Ok(new
                {
                    status = response.Status.ToString(),
                    message = response.Message,
                    confidence = response.Confidence,
                    primaryBooking = response.PrimaryBooking == null ? null : new
                    {
                        labRoomId = response.PrimaryBooking.LabRoomId,
                        roomName = response.PrimaryBooking.RoomName,
                        baseDate = response.PrimaryBooking.BaseDate,
                        startTime = response.PrimaryBooking.StartTime,
                        endTime = response.PrimaryBooking.EndTime,
                        studentCount = response.PrimaryBooking.StudentCount,
                        recurringCount = response.PrimaryBooking.RecurringCount,
                        purposeTypeId = response.PrimaryBooking.PurposeTypeId
                    },
                    suggestions = response.Suggestions.Select(s => new
                    {
                        title = s.Title,
                        description = s.Description,
                        labRoomId = s.LabRoomId,
                        date = s.Date,
                        startTime = s.StartTime,
                        endTime = s.EndTime,
                        reasonForSuggestion = s.ReasonForSuggestion,
                        matchScore = s.MatchScore,
                    }).ToList(),
                    conflictDetails = response.ConflictDetails,
                    requiresUserConfirmation = response.RequiresUserConfirmation
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing booking request");
                return StatusCode(500, new { error = "Failed to parse booking request" });
            }
        }

    }
}
