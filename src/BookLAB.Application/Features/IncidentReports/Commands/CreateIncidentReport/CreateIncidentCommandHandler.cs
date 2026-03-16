using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.IncidentReports.Commands.CreateIncidentReport
{
    public class CreateIncidentCommandHandler : IRequestHandler<CreateIncidentCommand, CreateIncidentResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateIncidentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateIncidentResponse> Handle(CreateIncidentCommand request, CancellationToken cancellationToken)
        {
            var reporterId = request.ReportedBy;

            var reporterExists = await _unitOfWork.Repository<User>().Entities
                .AnyAsync(user => user.Id == reporterId, cancellationToken);

            if (!reporterExists)
            {
                throw new NotFoundException("User", reporterId);
            }

            var incident = new Incident
            {
                Id = Guid.NewGuid(),
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                Severity = request.Severity,
                Environment = string.IsNullOrWhiteSpace(request.Environment) ? null : request.Environment.Trim(),
                StepsToReproduce = request.StepsToReproduce
                    .Where(step => !string.IsNullOrWhiteSpace(step))
                    .Select(step => step.Trim())
                    .ToList(),
                ExpectedResult = string.IsNullOrWhiteSpace(request.ExpectedResult) ? null : request.ExpectedResult.Trim(),
                ActualResult = string.IsNullOrWhiteSpace(request.ActualResult) ? null : request.ActualResult.Trim(),
                AttachmentUrl = string.IsNullOrWhiteSpace(request.AttachmentUrl) ? null : request.AttachmentUrl.Trim(),
                ReportedBy = reporterId,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = reporterId
            };

            await _unitOfWork.Incidents.AddAsync(incident);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateIncidentResponse(incident.Id);
        }
    }
}