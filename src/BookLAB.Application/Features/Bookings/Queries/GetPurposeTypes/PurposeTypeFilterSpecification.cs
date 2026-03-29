using BookLAB.Application.Common.Specifications;
using BookLAB.Domain.Entities;
using MediatR;


namespace BookLAB.Application.Features.Bookings.Queries.GetPurposeTypes
{
    public class PurposeTypeFilterSpecification : BaseSpecification<PurposeType>
    {
        public PurposeTypeFilterSpecification(GetPurposeTypesQuery query) 
        {
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                AddCriteria(x => x.PurposeName.Contains(query.SearchTerm));
            }

            ApplyOrderBy(s => s.PurposeName);

        }
    }
}
