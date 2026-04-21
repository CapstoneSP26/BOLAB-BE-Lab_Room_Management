using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookLAB.Application.Common.Extensions; // Chứa ApplySpecification extension
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.LabRooms.Queries.GetLabRooms;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Schedules.Queries.GetSchedules;

public class GetSchedulesQueryHandler : IRequestHandler<GetSchedulesQuery, PagedList<ScheduleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSchedulesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedList<ScheduleDto>> Handle(GetSchedulesQuery request, CancellationToken ct)
    {
        // 1. Khởi tạo Specification dựa trên tham số từ Query (FE gửi lên)
        // Đây là nơi logic lọc được đóng gói, giữ cho Handler luôn "mỏng" (Thin)
        var spec = new ScheduleFilterSpecification(request);

        // 2. Lấy IQueryable từ Repository
        var query = _unitOfWork.Repository<Schedule>().Entities
            .Include(x => x.LabRoom)
            .ApplySpecification(spec)
            .AsNoTracking();

        var projectedQuery = query.SelectSchedule();

        if (request.PageSize <= 0)
        {
            var allItems = await projectedQuery.ToListAsync(ct);
            // Trả về PagedList với TotalCount = số lượng thực tế, PageSize = TotalCount
            return new PagedList<ScheduleDto>(allItems, allItems.Count, 1, allItems.Count);
        }
        return await PagedList<ScheduleDto>.CreateAsync(
            projectedQuery,
            request.PageNumber,
            request.PageSize,
            ct);
    }
}