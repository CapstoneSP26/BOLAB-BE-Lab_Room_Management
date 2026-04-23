using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRooms;

public class GetLabRoomsQueryHandler : IRequestHandler<GetLabRoomsQuery, PagedList<LabRoomDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLabRoomsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedList<LabRoomDto>> Handle(GetLabRoomsQuery request, CancellationToken ct)
    {
        var spec = new LabRoomFilterSpecification(request);

        // Lấy IQueryable từ Repository
        var queryable = _unitOfWork.Repository<LabRoom>().Entities
            .ApplySpecification(spec)
            .AsNoTracking();

        queryable = queryable.Where(x => x.RoomName.ToLower().Contains(request.SearchTerm.ToLower()) || x.RoomNo.ToLower().Contains(request.SearchTerm.ToLower()));

        // Thực hiện Projection sang DTO để giảm tải dữ liệu từ DB
        var projectedQuery = queryable.SelectLabRoom(request.IncludeImages, request.IncludeBuilding, request.IncludeLabOwner);

        if (request.PageSize <= 0)
        {
            var allItems = await projectedQuery.ToListAsync(ct);
            var normalizedPageSize = allItems.Count > 0 ? allItems.Count : 1;

            return new PagedList<LabRoomDto>(allItems, allItems.Count, 1, normalizedPageSize);
        }

        return await PagedList<LabRoomDto>.CreateAsync(
            projectedQuery,
            request.PageNumber,
            request.PageSize,
            ct,
            countItems: true);
    }
}