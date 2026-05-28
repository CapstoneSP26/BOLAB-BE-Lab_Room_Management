using BookLAB.Application.Features.EmailTemplates.Commands.UpdateEmailTemplate;
using BookLAB.Application.Features.EmailTemplates.Queries.GetEmailTemplates;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookLAB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailTemplateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmailTemplateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplates()
        {
            var result = await _mediator.Send(
                new GetEmailTemplatesQuery());

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(
            int id,
            [FromBody] UpdateEmailTemplateRequest request)
        {
            var result = await _mediator.Send(
                new UpdateEmailTemplateCommand
                {
                    Id = id,
                    Subject = request.Subject,
                    Content = request.Content
                });

            return Ok(result);
        }
    }
}