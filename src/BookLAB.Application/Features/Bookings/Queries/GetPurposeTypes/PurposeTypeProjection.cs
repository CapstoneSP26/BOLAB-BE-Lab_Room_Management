using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Bookings.Queries.GetPurposeTypes
{
    public static class PurposeTypeProjection
    {
        public static IQueryable<PurposeTypeDto> SelectPurposeType(this IQueryable<PurposeType> query)
        {
            return query.Select(x => new PurposeTypeDto
            {
                Id = x.Id,
                PurposeName = x.PurposeName
            });
        }
    }
}
