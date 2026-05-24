using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.EmailTemplates.Queries.GetEmailTemplates
{
    public class GetEmailTemplatesQueryHandler
        : IRequestHandler<
            GetEmailTemplatesQuery,
            List<EmailTemplateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEmailTemplatesQueryHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<EmailTemplateDto>> Handle(
            GetEmailTemplatesQuery request,
            CancellationToken cancellationToken)
        {
            var templates = await _unitOfWork
                .Repository<EmailTemplate>()
                .Entities
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return templates.Select(x => new EmailTemplateDto
            {
                Id = x.Id,
                Subject = x.Subject,
                Content = x.Content,
                Type = (int)x.Type,
                Variables = x.GetVariables()
            }).ToList();
        }
    }
}