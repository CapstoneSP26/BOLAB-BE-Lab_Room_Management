using BookLAB.Application.Features.Attendances.Commands.GenerateAttendanceQrCode;
using BookLAB.Application.Features.Attendances.Commands.SubmitTraditionalAttendance;
using BookLAB.Application.Features.Attendances.Queries.GetAttendanceList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using BookLAB.Application.Features.Attendances.Commands.ScanAttendanceQRCode;

namespace BookLAB.Api.Controllers;

[Authorize] // Ensure only authenticated users can access attendance features
[ApiController]
[Route("api/[controller]")]
public class AttendancesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AttendancesController> _logger;

    public AttendancesController(IMediator mediator, ILogger<AttendancesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
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
        var result = await _mediator.Send(command);

        if (result)
        {
            return Ok(new { Message = "Attendance submitted successfully." });
        }

        return BadRequest("Failed to submit attendance.");
    }

    /// <summary>
    /// HTTP GET endpoint to generate a QR code image for attendance.
    /// Accepts scheduleId and isCheckIn as query parameters, validates input,
    /// sends a command through MediatR, and returns the QR code image as PNG.
    /// </summary>
    /// <param name="scheduleId">The schedule identifier provided as a string.</param>
    /// <param name="isCheckIn">Flag indicating whether the QR code is for check-in or check-out.</param>
    /// <param name="cancellationToken">Token to cancel the operation if requested.</param>
    /// <returns>
    /// Returns a PNG file containing the QR code if successful.
    /// Returns BadRequest if scheduleId is invalid.
    /// Returns 500 Internal Server Error if QR code generation fails.
    /// </returns>
    [HttpGet("generate-qrcode")]
    public async Task<IActionResult> GenerateAttendanceQRCode([FromQuery] string scheduleId, [FromQuery] bool isCheckIn, CancellationToken cancellationToken)
    {
        try
        {
            // Validate that scheduleId is a valid Guid
            if (!Guid.TryParse(scheduleId, out var scheduleGuid))
                return BadRequest("Invalid scheduleId format.");

            // Create the command object to send via MediatR
            GenerateAttendanceQrCodeCommand command = new GenerateAttendanceQrCodeCommand
            {
                ScheduleId = scheduleGuid,
                IsCheckIn = isCheckIn
            };

            // Send the command to the handler and get the QR code image
            var qrCodeImage = await _mediator.Send(command, cancellationToken);

            // If QR code generation failed, return 500 error
            if (qrCodeImage == null || qrCodeImage.Length == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "QR code could not be generated.");
            }

            string base64string = Convert.ToBase64String(qrCodeImage);

            // Return the QR code image as a PNG file
            return Ok(new
            {
                success = true,
                data = base64string
            });
            //return File(qrCodeImage, "image/png");
        }
        catch (Exception ex)
        {
            // Log the exception with context information
            _logger.LogError(ex, "Error generating QR code for ScheduleId {ScheduleId}, IsCheckIn {IsCheckIn}", scheduleId, isCheckIn);

            // Return 500 Internal Server Error if an exception occurs
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while generating the QR code");
        }
    }


    /// <summary>
    /// Handles the HTTP POST request to scan a QR code for attendance.
    /// The method validates the current user's identity, 
    /// constructs a ScanAttendanceQrCodeCommand with the provided parameters, 
    /// and sends it through MediatR to process the attendance check-in or check-out.
    /// </summary>
    /// <param name="qrId">The unique identifier of the QR code.</param>
    /// <param name="scheduleId">The unique identifier of the schedule associated with the attendance.</param>
    /// <param name="studentId">The unique identifier of the student scanning the QR code.</param>
    /// <param name="isCheckIn">A flag indicating whether the action is a check-in (true) or check-out (false).</param>
    /// <param name="cancellationToken">
    /// Token provided by ASP.NET Core to cancel the request if the client disconnects or times out.
    /// </param>
    /// <returns>
    /// An IActionResult indicating the result of the operation:
    /// - 200 OK with a success message if the QR code was scanned successfully.
    /// - 400 BadRequest if the QR code scan failed due to invalid data.
    /// - 401 Unauthorized if the user identity is invalid.
    /// - 500 Internal Server Error if an unexpected exception occurs.
    /// </returns>
    [HttpGet("scan-qrcode")]
    public async Task<IActionResult> ScanAttendanceQRCode([FromQuery] Guid qrId, [FromQuery] Guid scheduleId, [FromQuery] Guid studentId, [FromQuery] bool isCheckIn, CancellationToken cancellationToken)
    {
        try
        {
            // Validate user identity from claims
            if (!Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out Guid userId))
                return Unauthorized(new
                {
                    success = false,
                    message = "Login is required to do the action"
                });

            // Construct the command with all required parameters
            ScanAttendanceQrCodeCommand command = new ScanAttendanceQrCodeCommand
            {
                LecturerId = userId,
                qrId = qrId,
                scheduleId = scheduleId,
                studentId = studentId,
                IsCheckIn = isCheckIn
            };

            // Send the command through MediatR pipeline
            var result = await _mediator.Send(command, cancellationToken);

            // Return failure response if the QR code scan was unsuccessful
            if (!result)
            {
                return BadRequest("Failed to scan the QR code. Please ensure the QR code is valid and try again.");
            }

            // Return success response if the QR code scan was successful
            return Ok(new
            {
                success = true,
                message = "QR code scanned successfully."
            });
        }
        catch (Exception ex)
        {
            // Log the exception with contextual details
            _logger.LogError(ex, "Error scanning QR code. ScheduleId: {ScheduleId}, StudentId: {StudentId}", scheduleId, studentId);

            // Return internal server error response
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while scanning the QR code");
        }
    }

    [HttpGet("remove-qrcode")]
    public async Task<IActionResult> RemoveAttendanceQRCode([FromQuery] string scheduleId, [FromQuery] bool isCheckIn, CancellationToken cancellationToken)
    {
        try
        {
            // Validate that scheduleId is a valid Guid
            if (!Guid.TryParse(scheduleId, out var scheduleGuid))
                return BadRequest("Invalid scheduleId format.");

            // Create the command object to send via MediatR
            GenerateAttendanceQrCodeCommand command = new GenerateAttendanceQrCodeCommand
            {
                ScheduleId = scheduleGuid,
                IsCheckIn = isCheckIn
            };

            // Send the command to the handler and get the QR code image
            var qrCodeImage = await _mediator.Send(command, cancellationToken);

            // If QR code generation failed, return 500 error
            if (qrCodeImage == null || qrCodeImage.Length == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "QR code could not be generated.");
            }

            string base64string = Convert.ToBase64String(qrCodeImage);

            // Return the QR code image as a PNG file
            return Ok(new
            {
                success = true,
                data = base64string
            });
        }
        catch (Exception ex)
        {
            // Log the exception with context information
            _logger.LogError(ex, "Error generating QR code for ScheduleId {ScheduleId}, IsCheckIn {IsCheckIn}", scheduleId, isCheckIn);

            // Return 500 Internal Server Error if an exception occurs
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while generating the QR code");
        }
    }

}