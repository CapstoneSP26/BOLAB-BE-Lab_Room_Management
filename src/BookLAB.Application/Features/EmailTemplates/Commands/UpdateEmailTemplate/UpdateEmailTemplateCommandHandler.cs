using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.EmailTemplates.Commands.UpdateEmailTemplate
{
    public class UpdateEmailTemplateCommandHandler
        : IRequestHandler<UpdateEmailTemplateCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEmailTemplateCommandHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            UpdateEmailTemplateCommand request,
            CancellationToken cancellationToken)
        {
            var template = await _unitOfWork
                .Repository<EmailTemplate>()
                .Entities
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id,
                    cancellationToken);

            if (template == null)
                return false;

            template.Subject = request.Subject;
            template.Content = request.Content;

            await _unitOfWork
                .Repository<EmailTemplate>()
                .UpdateAsync(template);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}