using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Bookings.Queries.GetPurposeTypes;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace BookLAB.Application.Features.Bookings.Queries.GetBookings
{
    public class GetBookingsQueryHandler : IRequestHandler<GetBookingsQuery, PagedList<BookingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetBookingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PagedList<BookingDto>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
        {
            // 1. Create the specification based on FE filters
            var spec = new BookingFilterSpecification(request);

            // 2. Execute query with Pagination & Sorting
            var query = _unitOfWork.Repository<BookingRequest>().Entities
                .ApplySpecification(spec) // Extension method to apply Includes and Criteria
                .AsNoTracking();

            var projectedQuery = query.SelectPurposeType();
            // Tận dụng logic PageSize <= 0 để Get All
            if (request.PageSize <= 0)
            {
                var allItems = await projectedQuery.ToListAsync(cancellationToken);
                return new PagedList<BookingDto>(allItems, allItems.Count, 1, allItems.Count);
            }


            return await PagedList<BookingDto>.CreateAsync(projectedQuery, request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}

