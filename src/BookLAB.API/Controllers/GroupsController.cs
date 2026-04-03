using BookLAB.Application.Features.Groups.Commands.AddGroupMember;
using BookLAB.Application.Features.Groups.Commands.CreateGroup;
using BookLAB.Application.Features.Groups.Commands.DeleteGroup;
using BookLAB.Application.Features.Groups.Commands.RemoveGroupMember;
using BookLAB.Application.Features.Groups.Commands.UpdateGroup;
using BookLAB.Application.Features.Groups.Commands.UpdateGroupMember;
using BookLAB.Application.Features.Groups.DTOs;
using BookLAB.Application.Features.Groups.Queries.GetGroupById;
using BookLAB.Application.Features.Groups.Queries.GetGroupMembers;
using BookLAB.Application.Features.Groups.Queries.GetGroups;
//using BookLAB.Application.Common.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BookLAB.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(
            IMediator mediator,
            ILogger<GroupsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        #region Group Management Endpoints

        /// <summary>
        /// Create a new group
        /// </summary>
        /// <param name="command">Group creation details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>ID of the newly created group</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var groupId = await _mediator.Send(command, cancellationToken);
                return CreatedAtAction(nameof(GetGroupById), new { id = groupId }, new { groupId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating group");
                throw;
            }
        }

        /// <summary>
        /// Get all groups owned by the current user
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of groups</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<GroupDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice_LabManager")]
        public async Task<IActionResult> GetGroups(CancellationToken cancellationToken)
        {
            try
            {
                var groups = await _mediator.Send(new GetGroupsQuery(), cancellationToken);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving groups");
                throw;
            }
        }

        /// <summary>
        /// Get details of a specific group
        /// </summary>
        /// <param name="id">Group ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Group details</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice_LabManager")]
        public async Task<IActionResult> GetGroupById(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var group = await _mediator.Send(new GetGroupByIdQuery { GroupId = id }, cancellationToken);
                return Ok(group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving group {GroupId}", id);
                throw;
            }
        }

        /// <summary>
        /// Update a group
        /// </summary>
        /// <param name="id">Group ID</param>
        /// <param name="command">Update details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice")]
        public async Task<IActionResult> UpdateGroup(Guid id, [FromBody] UpdateGroupCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var updateCommand = command with { GroupId = id };
                await _mediator.Send(updateCommand, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating group {GroupId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a group (soft delete)
        /// </summary>
        /// <param name="id">Group ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice")]
        public async Task<IActionResult> DeleteGroup(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new DeleteGroupCommand { GroupId = id }, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting group {GroupId}", id);
                throw;
            }
        }

        #endregion

        #region Group Member Management Endpoints

        /// <summary>
        /// Add a student to a group
        /// </summary>
        /// <param name="groupId">Group ID</param>
        /// <param name="command">Member addition details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpPost("{groupId:guid}/members")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice")]
        public async Task<IActionResult> AddGroupMember(Guid groupId, [FromBody] AddGroupMemberCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var addCommand = command with { GroupId = groupId };
                await _mediator.Send(addCommand, cancellationToken);
                return CreatedAtAction(nameof(GetGroupMembers), new { groupId }, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding member to group {GroupId}", groupId);
                throw;
            }
        }

        /// <summary>
        /// Get all members of a group
        /// </summary>
        /// <param name="groupId">Group ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of group members</returns>
        [HttpGet("{groupId:guid}/members")]
        [ProducesResponseType(typeof(List<GroupMemberDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice")]
        public async Task<IActionResult> GetGroupMembers(Guid groupId, CancellationToken cancellationToken)
        {
            try
            {
                var members = await _mediator.Send(new GetGroupMembersQuery { GroupId = groupId }, cancellationToken);
                return Ok(members);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving group members {GroupId}", groupId);
                throw;
            }
        }

        /// <summary>
        /// Update a group member's information
        /// </summary>
        /// <param name="groupId">Group ID</param>
        /// <param name="userId">User/Student ID</param>
        /// <param name="command">Update details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpPut("{groupId:guid}/members/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice")]
        public async Task<IActionResult> UpdateGroupMember(Guid groupId, Guid userId, [FromBody] UpdateGroupMemberCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var updateCommand = command with { GroupId = groupId, UserId = userId };
                await _mediator.Send(updateCommand, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating group member {GroupId}/{UserId}", groupId, userId);
                throw;
            }
        }

        /// <summary>
        /// Remove a student from a group
        /// </summary>
        /// <param name="groupId">Group ID</param>
        /// <param name="userId">User/Student ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpDelete("{groupId:guid}/members/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice")]
        public async Task<IActionResult> RemoveGroupMember(Guid groupId, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new RemoveGroupMemberCommand { GroupId = groupId, UserId = userId }, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing group member {GroupId}/{UserId}", groupId, userId);
                throw;
            }
        }

        #endregion
    }
}
