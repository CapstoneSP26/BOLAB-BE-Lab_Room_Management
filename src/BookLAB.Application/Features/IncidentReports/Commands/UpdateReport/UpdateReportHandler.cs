using AutoMapper;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.IncidentReports.Commands.UpdateReport
{
    public class UpdateReportHandler : IRequestHandler<UpdateReportCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateReportHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
        {
            var report = await _unitOfWork.Repository<Report>().Entities.FirstOrDefaultAsync(x => x.Id == request.ReportId);

            if (report.Id != request.ReportId)
            {
                return false;
            }

            report.ReportTypeId = request.TempReport.ReportTypeId != 0 ? request.TempReport.ReportTypeId : report.ReportTypeId;
            report.Description = request.TempReport.Description == null ? request.TempReport.Description : report.Description;
            report.IsResolved = request.TempReport.IsResolved.Value;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Repository<Report>().UpdateAsync(report);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return true;
            } catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return false;
            }
            
        }
    }
}
