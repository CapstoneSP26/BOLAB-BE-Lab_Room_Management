using AutoMapper;
using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Schedules.Queries.GetImportBatches
{
    public class GetImportBatchesQueryHandler : IRequestHandler<GetImportBatchesQuery, PagedList<ImportBatchDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetImportBatchesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedList<ImportBatchDto>> Handle(GetImportBatchesQuery request, CancellationToken ct)
        {
            var spec = new ImportBatchFilterSpecification(request);

            // Lấy IQueryable từ Repository
            var query = _unitOfWork.Repository<ImportBatch>().Entities
                .ApplySpecification(spec)
                .AsNoTracking();

            // Thực hiện Projection sang DTO để giảm tải dữ liệu từ DB
            var projectedQuery = query.SelectImportBatch();

            if (request.PageSize <= 0)
            {
                var allItems = await projectedQuery.ToListAsync(ct);
                var normalizedPageSize = allItems.Count > 0 ? allItems.Count : 1;

                return new PagedList<ImportBatchDto>(allItems, allItems.Count, 1, normalizedPageSize);
            }

            return await PagedList<ImportBatchDto>.CreateAsync(
                projectedQuery,
                request.PageNumber,
                request.PageSize,
                ct,
                countItems: true);
        }
    }
}