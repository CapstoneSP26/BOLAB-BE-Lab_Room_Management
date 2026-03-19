using BookLAB.Application.Features.Attendances.Commands.SubmitTraditionalAttendance;
using BookLAB.Application.Features.Attendances.Queries.GetAttendanceList;
using BookLAB.Application.Features.Attendance.Commands.GenerateAttendanceQrCode;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace BookLAB.Api.Controllers;

[Authorize] // Ensure only authenticated users can access attendance features
[ApiController]
[Route("api/[controller]")]
    [ApiController]
public class AttendancesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttendancesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets the list of students for attendance based on Schedule (Group + Subject logic)
    /// </summary>
    /// <param name="scheduleId">The ID of the specific schedule slot</param>
    /// <returns>A list of students with their current attendance status</returns>
    [HttpGet("schedule/{scheduleId:guid}")]
    [ProducesResponseType(typeof(List<AttendanceStudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAttendanceList(Guid scheduleId)
    {
        var query = new GetAttendanceListQuery(scheduleId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Submits or updates the attendance list for a specific schedule
    /// </summary>
    /// <param name="command">The attendance data submitted by the lecturer</param>
    /// <returns>Success status of the operation</returns>
    [HttpPost("submit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SubmitAttendance([FromBody] SubmitAttendanceCommand command)
    {
                    qrId = qrId,
                    scheduleId = scheduleId,
                    studentId = studentId
                };

        var result = await _mediator.Send(command);

        if (result)
        {
            return Ok(new { Message = "Attendance submitted successfully." });
        }

        return BadRequest("Failed to submit attendance.");
    }
}