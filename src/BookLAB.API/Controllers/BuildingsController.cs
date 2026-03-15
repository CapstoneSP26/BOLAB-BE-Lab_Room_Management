using BookLAB.Application.Features.Buildings.Queries.GetBuildingsInCampus;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookLAB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BuildingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/<BuildingsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<BuildingsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<BuildingsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BuildingsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BuildingsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet("GetBuildingsInCampus")]
        public async Task<IActionResult> GetBuildingsInCampusAsync([FromQuery] int campusId)
        {
            GetBuildingsInCampusCommand command = new GetBuildingsInCampusCommand()
            {
                campusId = campusId
            };

            var result = await _mediator.Send(command);

            return Ok(new 
            {
                buildings = result,
                total = result.Count
            });
        }
    }
}
