using BookLAB.Application.Features.Attendance.Commands.GenerateAttendanceQrCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace BookLAB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AttendancesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("generate-qrcode")]
        public async Task<IActionResult> GenerateAttendanceQRCode([FromQuery] string scheduleId)
        {
            try
            {
                GenerateAttendanceQrCodeCommand command = new GenerateAttendanceQrCodeCommand
                {
                    ScheduleId = scheduleId
                };

                var qrCodeImage = await _mediator.Send(command);

                if (qrCodeImage == null || qrCodeImage.Length == 0)
                {
                    return NotFound("QR code could not be generated.");
                }

                return File(qrCodeImage, "image/png");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while generating the QR code: {ex.Message}");
            }

        }

        [HttpPost("scan-qrcode")]
        public async Task<IActionResult> ScanAttendanceQRCode([FromQuery] string qrId, string scheduleId, string studentId)
        {
            try
            {
                ScanAttendanceQrCodeCommand command = new ScanAttendanceQrCodeCommand
                {
                    qrId = qrId,
                    scheduleId = scheduleId,
                    studentId = studentId
                };

                var result = await _mediator.Send(command);

                if (!result)
                {
                    return BadRequest("Failed to scan the QR code. Please ensure the QR code is valid and try again.");
                }

                return Ok("QR code scanned successfully.");
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while scanning the QR code: {ex.Message}");
            }
        }
    }
}
