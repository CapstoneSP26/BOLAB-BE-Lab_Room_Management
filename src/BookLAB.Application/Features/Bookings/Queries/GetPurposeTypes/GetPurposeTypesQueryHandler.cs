using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Bookings.Queries.GetPurposeTypes
{
    public class GetPurposeTypesQueryHandler : IRequestHandler<GetPurposeTypesQuery, PagedList<PurposeTypeDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPurposeTypesQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedList<PurposeTypeDto>> Handle(GetPurposeTypesQuery request, CancellationToken ct)
        {
            var spec = new PurposeTypeFilterSpecification(request);
            var query = _unitOfWork.Repository<PurposeType>().Entities
                .ApplySpecification(spec)
                .AsNoTracking();

            var projectedQuery = query.SelectPurposeType();
            // Tận dụng logic PageSize <= 0 để Get All
            if (request.PageSize <= 0)
            {
                var allItems = await projectedQuery.ToListAsync(ct);
                return new PagedList<PurposeTypeDto>(allItems, allItems.Count, 1, allItems.Count);
            }

            return await PagedList<PurposeTypeDto>.CreateAsync(
                projectedQuery,
                request.PageNumber,
                request.PageSize,
                ct);
        }
    }
}
