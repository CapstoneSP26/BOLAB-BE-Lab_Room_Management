using BookLAB.Application.Features.Bookings.Queries.GetBookingStats;
using BookLAB.Application.Features.Bookings.Queries.ViewBookingHistory;
using BookLAB.Domain.DTOs;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookLAB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// This endpoint retrieves the booking history for a specific user.
        /// </summary>
        /// <param name="dto">Nessesary info for endpoint</param>
        /// <returns>List of booking user has booked in the past</returns>
        [HttpPost("get-booking-history")]
        public async Task<IActionResult> GetBookingHistoryList([FromBody] ViewBookingHistoryDTO dto)
        {
            try
            {
                ViewBookingHistoryCommand command = new ViewBookingHistoryCommand
                {
                    //userId = HttpContext.User.FindFirst("Id").Value ?? "11111111-1111-1111-1111-111111111111",
                    userId = "11111111-1111-1111-1111-111111111111",
                    page = dto.page,
                    limit = dto.limit,
                    status = dto.status,
                    startDate = dto.startDate,
                    endDate = dto.endDate,
                };

                var result = await _mediator.Send(command);

                ViewBookingHistoryResponseDTO[] data = new ViewBookingHistoryResponseDTO[result.Count];

                for (int i = 0; i < result.Count; i++)
                {
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
                        userName = result[i].CreatedByUser.FullName
                    };
                }

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
                return null;
            }

        }

        [HttpGet("get-booking-stats")]
        public async Task<IActionResult> GetBookingStats([FromQuery] GetBookingStatsRequestDTO dto)
        {
            try
            {
                GetBookingStatsCommand command = new GetBookingStatsCommand
                {
                     //userId = "11111111-1111-1111-1111-111111111111"
                    userId = HttpContext.User.FindFirst("Id").Value,
                    startDate = DateTimeOffset.Parse(dto.startDate).ToUniversalTime(),
                    endDate = DateTimeOffset.Parse(dto.endDate).ToUniversalTime(),
                };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
