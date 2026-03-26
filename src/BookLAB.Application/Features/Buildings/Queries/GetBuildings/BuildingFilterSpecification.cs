
using BookLAB.Application.Common.Specifications;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Buildings.Queries.GetBuildings
{
    public class BuildingFilterSpecification : BaseSpecification<Building>
    {
        public BuildingFilterSpecification(GetBuildingsQuery query)
        {
            if (query.CampusId.HasValue)
                AddCriteria(x => x.CampusId == query.CampusId);

            if (!string.IsNullOrWhiteSpace(query.BuildingName)) 
                AddCriteria(x => x.BuildingName == query.BuildingName);

            ApplyOrderBy(x => x.BuildingName);
        }
    }
}
