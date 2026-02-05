using BookLAB.Application.Features.Bookings.AddSchedule;
using BookLAB.Application.Features.Bookings.CheckConflict;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookLAB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SchedulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/<SchedulesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<SchedulesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SchedulesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SchedulesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SchedulesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet("check-conflict")]
        public async Task<bool> CheckConflictAsync([FromBody] Booking booking)
        {
            if (booking == null) return true;

            CheckConflictCommand command = new CheckConflictCommand
            {
                booking = booking
            };

            var isConflict = await _mediator.Send(command);

            if (isConflict) return true;

            return false;
        }

        [HttpGet("add")]
        public async Task<bool> AddScheduleAsync([FromBody] Guid lecturerId, int labRoomId, string scheduleType, DateTimeOffset startTime, DateTimeOffset endTime, bool fromAdmin)
        {
            if (lecturerId == null || labRoomId == null || scheduleType == null || startTime == null || endTime == null) return false;

            Schedule schedule = new Schedule
            {
                LecturerId = lecturerId,
                LabRoomId = labRoomId,
                ScheduleType = scheduleType,
                ScheduleStatus = "Not yet",
                StartTime = startTime,
                EndTime = endTime,
                CreatedAt = DateTimeOffset.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            AddScheduleCommand command = new AddScheduleCommand
            {
                Schedule = schedule,
            };

            var isSuccess = await _mediator.Send(command);

            if (isSuccess) return true;

            return false;
        }
    }
}
