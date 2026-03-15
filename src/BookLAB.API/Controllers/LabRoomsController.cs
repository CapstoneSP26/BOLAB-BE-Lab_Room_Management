using BookLAB.Application.Features.LabRooms.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookLAB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabRoomsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LabRoomsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/<LabRoomsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<LabRoomsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LabRoomsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<LabRoomsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LabRoomsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet("GetLabRoomsInBuilding")]
        public async Task<IActionResult> GetLabRoomsInBuilding([FromQuery] string buildingId)
        {
            GetLabRoomsInBuildingCommand command = new GetLabRoomsInBuildingCommand()
            {
                buildingId = buildingId
            };

            var result = await _mediator.Send(command);

            return Ok(new 
            {
                rooms = result,
                total = result.Count
            });
        }
    }
}
