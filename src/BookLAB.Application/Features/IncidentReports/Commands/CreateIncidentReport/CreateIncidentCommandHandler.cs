using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
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
            // Verify Schedule exists
            var scheduleExists = await _unitOfWork.Repository<Schedule>().Entities
                .AnyAsync(s => s.Id == request.ScheduleId, cancellationToken);

            if (!scheduleExists)
            {
                throw new NotFoundException("Schedule", request.ScheduleId);
            }

            // Verify Creator exists
            var creatorExists = await _unitOfWork.Repository<User>().Entities
                .AnyAsync(u => u.Id == request.CreatedBy, cancellationToken);

            if (!creatorExists)
            {
                throw new NotFoundException("User", request.CreatedBy);
            }

            var report = new Report
            {
                Id = Guid.NewGuid(),
                ScheduleId = request.ScheduleId,
                ReportTypeId = request.ReportTypeId,
                Description = request.Description.Trim(),
                IsResolved = false,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = request.CreatedBy
            };

            await _unitOfWork.Repository<Report>().AddAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateIncidentResponse(report.Id);
        }
    }
}