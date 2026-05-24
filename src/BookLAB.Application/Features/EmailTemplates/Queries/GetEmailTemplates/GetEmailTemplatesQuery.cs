using BookLAB.Application.Common.Models;
using MediatR;

namespace BookLAB.Application.Features.EmailTemplates.Queries.GetEmailTemplates
{
    public class GetEmailTemplatesQuery
        : IRequest<List<EmailTemplateDto>>
    {
    }
}