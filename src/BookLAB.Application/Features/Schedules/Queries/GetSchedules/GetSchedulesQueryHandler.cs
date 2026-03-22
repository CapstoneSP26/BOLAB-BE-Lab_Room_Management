using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookLAB.Application.Common.Extensions; // Chứa ApplySpecification extension
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
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
        var queryable = _unitOfWork.Repository<Schedule>().Entities;

        // 3. Áp dụng Specification (Lọc + Include các bảng liên quan)
        // Sau đó Project trực tiếp sang DTO để tối ưu hiệu năng SQL (chỉ lấy cột cần thiết)
        var filteredQuery = queryable
            .ApplySpecification(spec)
            .ProjectTo<ScheduleDto>(_mapper.ConfigurationProvider)
            .AsNoTracking();

        // 4. Thực hiện phân trang và trả về kết quả
        // PagedList.CreateAsync sẽ thực thi Count và ToList trong 1 luồng xử lý
        return await PagedList<ScheduleDto>.CreateAsync(
            filteredQuery,
            request.PageNumber,
            request.PageSize,
            ct);
    }
}