using BookLAB.Application.Features.SlotTypes.Command.CreateSlotType;
using BookLAB.Application.Features.SlotTypes.Command.DeleteSlotType;
using BookLAB.Application.Features.SlotTypes.Command.UpdateSlotType;
using BookLAB.Application.Features.SlotTypes.GetSlotTypes;
using BookLAB.Application.Features.SlotTypes.Queries.GetSlotTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLAB.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SlotTypesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SlotTypesController> _logger;

        public SlotTypesController(IMediator mediator, ILogger<SlotTypesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "AcademicOffice_LabManager_Lecturer")]
        public async Task<ActionResult<List<SlotTypeDto>>> GetSlotTypes([FromQuery] int? campusId)
        {
            var query = new GetSlotTypesQuery { CampusId = campusId };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AcademicOffice")]
        public async Task<ActionResult> CreateSlotType([FromBody] CreateSlotTypeCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AcademicOffice")]
        public async Task<ActionResult> UpdateSlotType([FromBody] UpdateSlotTypeCommand command, [FromRoute] int id)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                if (result.Success)
                    return Ok(result);
                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AcademicOffice")]
        public async Task<ActionResult> DeleteSlotType([FromRoute] int id)
        {
            try
            {
                DeleteSlotTypeCommand command = new DeleteSlotTypeCommand
                {
                    Id = id
                };
                
                var result = await _mediator.Send(command);
                if (result.Success)
                    return Ok(result);
                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
