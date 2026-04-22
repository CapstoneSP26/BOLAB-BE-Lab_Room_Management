using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Users.Commands.ImportUsers;
using BookLAB.Application.Features.Users.Commands.ValidateImportUsers;
using BookLAB.Application.Features.Users.Common;
using BookLAB.Application.Features.Users.Queries.GetUsers;
using BookLAB.Domain.Entities;
using ClosedXML.Excel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BookLAB.API.Controllers
{
    [Authorize(Policy = "AcademicOffice")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public UsersController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpPost("import/validate")]
        [ProducesResponseType(typeof(ImportValidationResult<UserImportDto, User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidateImport([FromBody] ValidateUserImportQuery query)
        {
            query.CampusId = _currentUserService.CampusId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("import/commit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmImport([FromBody] ConfirmUserImportCommand command)
        {
            command.CampusId = _currentUserService.CampusId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Policy = "AcademicOffice_Lecturer")]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        //[HttpPatch("{id}/status")]
        //[Authorize(Policy = "AcademicOffice")]
        //public async Task<IActionResult> UpdateStatusUsers([FromBody] bool IsActive, [FromRoute] Guid id)
        //{
        //    UpdateUserCommand command = new UpdateUserCommand
        //    {
        //        Id = id,
        //        IsActive = IsActive
        //    };
        //    var result = await _mediator.Send(command);
        //    return Ok(result);
        //}
    }
}
