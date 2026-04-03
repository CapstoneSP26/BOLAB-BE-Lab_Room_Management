using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace BookLAB.Application.Features.IncidentReports.Queries.GetReports;

public class GetReportsQueryHandler : IRequestHandler<GetReportsQuery, PagedList<ReportDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetReportsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedList<ReportDto>> Handle(GetReportsQuery request, CancellationToken ct)
    {
        var spec = new ReportFilterSpecification(request);

        // Lấy IQueryable từ Repository
        var queryable = _unitOfWork.Repository<Report>().Entities
            .ApplySpecification(spec)
            .Include(x => x.ReportType)
            .AsNoTracking();

        queryable = queryable.Where(x => _unitOfWork.Repository<LabOwner>().Entities.Any(y => y.LabRoomId == x.Schedule.LabRoomId &&
                                                                                        y.UserId == request.UserId));

        var projectedQuery = queryable.ProjectTo<ReportDto>(_mapper.ConfigurationProvider);

        if (request.Page <= 0)
        {
            var allItems = await projectedQuery.ToListAsync(ct);
            allItems.ForEach( async x =>
            {
                var user = await _unitOfWork.Repository<User>().GetByIdAsync(x.CreatedBy);
                x.UserName = user != null ? user.FullName : "Unknown";
            });
            // Trả về PagedList với TotalCount = số lượng thực tế, PageSize = TotalCount
            return new PagedList<ReportDto>(allItems, allItems.Count, 1, allItems.Count);
        }

        var allItems2 = await projectedQuery.Skip((request.Page.Value - 1) * request.Limit.Value).Take(request.Limit.Value).ToListAsync(ct);

        allItems2.ForEach(x =>
        {
            var user = _unitOfWork.Repository<User>().GetById(x.CreatedBy);
            x.UserName = user != null ? user.FullName : "Unknown";
        });

        return new PagedList<ReportDto>(
            allItems2,
            allItems2.Count,
            request.Page.Value,
            request.Limit.Value);
    }
}