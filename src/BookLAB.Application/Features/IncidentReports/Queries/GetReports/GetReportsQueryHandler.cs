using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            .AsNoTracking();

        queryable = queryable.Where(x => _unitOfWork.Repository<LabOwner>().Entities.Any(y => y.LabRoomId == x.Schedule.LabRoomId &&
                                                                                        y.UserId == request.UserId));

        var projectedQuery = queryable.ProjectTo<ReportDto>(_mapper.ConfigurationProvider);

        if (request.Page <= 0)
        {
            var allItems = await projectedQuery.ToListAsync(ct);
            // Trả về PagedList với TotalCount = số lượng thực tế, PageSize = TotalCount
            return new PagedList<ReportDto>(allItems, allItems.Count, 1, allItems.Count);
        }

        return await PagedList<ReportDto>.CreateAsync(
            projectedQuery,
            request.Page.Value,
            request.Limit.Value,
            ct);
    }
}