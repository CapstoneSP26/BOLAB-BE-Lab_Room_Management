using BookLAB.Application.Features.Bookings.ViewBookingHistory;
using BookLAB.Domain.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<List<Booking>> GetBookingHistoryList([FromBody] ViewBookingHistoryDTO dto)
        {
            try
            {
                ViewBookingHistoryCommand command = new ViewBookingHistoryCommand
                {
                    userId = HttpContext.User.FindFirst("Id").Value ?? "11111111-1111-1111-1111-111111111111",
                    page = dto.page,
                    limit = dto.limit,
                    status = dto.status,
                    startDate = dto.startDate,
                    endDate = dto.endDate,
                };

                var result = await _mediator.Send(command);

                return result;
            } catch (Exception ex)
            {
                return null;
            }
            
        }
    }
}
