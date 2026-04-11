using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.LabRooms.Commands.CreateLabRoom;
using BookLAB.Application.Features.LabRooms.Commands.DeleteLabRoom;
using BookLAB.Application.Features.LabRooms.Commands.ImportLabRooms;
using BookLAB.Application.Features.LabRooms.Commands.UpdateLabRoom;
using BookLAB.Application.Features.LabRooms.Commands.UpdatePolicy;
using BookLAB.Application.Features.LabRooms.Commands.ValidateImportLabRooms;
using BookLAB.Application.Features.LabRooms.Queries.GetLabRoomPolicies;
using BookLAB.Application.Features.LabRooms.Queries.GetLabRooms;
using BookLAB.Domain.Enums;
using ClosedXML.Excel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BookLAB.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LabRoomController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LabRoomController> _logger;

        public LabRoomController(IMediator mediator, ILogger<LabRoomController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}/policies")]
        [Authorize(Policy = "AcademicOffice_LabManager_Lecturer")]
        public async Task<IActionResult> GetPolicies(int id)
        {
            var query = new GetLabRoomPoliciesQuery { LabRoomId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{labRoomId:int}/policies/{policyKey}")]
        public async Task<ActionResult<LabRoomPolicyUpdateDto>> UpdatePolicy(int labRoomId, string policyKey,[FromBody] LabRoomPolicyUpdatePayload payload)
        {
            if (Enum.TryParse<PolicyType>(policyKey, true, out var enumValue))
            {
                var command = new UpdateLabPolicyCommand
                {
                    LabRoomId = labRoomId,
                    PolicyKey = enumValue,
                    PolicyValue = payload.PolicyValue,
                    IsActive = payload.IsActive
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }

            
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedList<BookLAB.Application.Features.LabRooms.Queries.GetLabRooms.LabRoomDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = "AcademicOffice_LabManager_Lecturer")]
        public async Task<IActionResult> GetLabRooms([FromQuery] GetLabRoomsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AcademicOffice")]
        public async Task<IActionResult> CreateLabRoom([FromBody] CreateLabRoomCommand command)
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
        public async Task<IActionResult> UpdateLabRoom([FromBody] UpdateLabRoomCommand command, [FromRoute] int id)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Policy = "AcademicOffice")]
        public async Task<IActionResult> UpdateLabRoomStatus([FromBody] bool isActive, [FromRoute] int id)
        {
            try
            {
                UpdateLabRoomCommand command = new UpdateLabRoomCommand
                {
                    Id = id,
                    IsActive = isActive
                };
                var result = await _mediator.Send(command);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AcademicOffice")]
        public async Task<IActionResult> DeleteLabRoom([FromRoute] int id)
        {
            try
            {
                DeleteLabRoomCommand command = new DeleteLabRoomCommand
                {
                    LabRoomId = id,
                };
                var result = await _mediator.Send(command);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost("validate-import")]
        //[ProducesResponseType(typeof(ImportValidationResult<LabRoomImportDto>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> ValidateImport([FromBody] ValidateLabRoomImportQuery query)
        //{
        //    var result = await _mediator.Send(query);
        //    return Ok(result);
        //}



        //[HttpPost("import")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> ConfirmImport([FromBody] ConfirmLabRoomImportCommand command)
        //{
        //    var result = await _mediator.Send(command);
        //    return Ok(result);
        //}

    }
}
