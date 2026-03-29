using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Buildings.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Buildings.Queries.GetBuildings
{
    public class GetBuildingsQueryHandler : IRequestHandler<GetBuildingsQuery, PagedList<BuildingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetBuildingsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PagedList<BuildingDto>> Handle(GetBuildingsQuery request, CancellationToken ct)
        {
            request.CampusId = request.CampusId ?? _currentUserService.CampusId;
            var spec = new BuildingFilterSpecification(request);
            
            // 1. Lấy IQueryable đã được Filter bởi Spec (Chưa thực thi SQL)
            var query = _unitOfWork.Repository<Building>().Entities
            .ApplySpecification(spec)
            .AsNoTracking();

            // 2. Dùng Manual Projection để control SQL Select
            var projectedQuery = query.SelectBuilding();

            // Xử lý "Get All" nếu PageSize là con số đặc biệt
            if (request.PageSize <= 0)
            {
                var allItems = await projectedQuery.ToListAsync(ct);
                // Trả về PagedList với TotalCount = số lượng thực tế, PageSize = TotalCount
                return new PagedList<BuildingDto>(allItems, allItems.Count, 1, allItems.Count);
            }

            // 3. Phân trang và thực thi SQL (CreateAsync đã được sửa để Optional Count)
            return await PagedList<BuildingDto>.CreateAsync(
                projectedQuery,
                request.PageNumber,
                request.PageSize,
                ct);
        }
    }
}
