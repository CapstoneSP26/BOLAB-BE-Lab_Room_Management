using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Buildings.Queries.GetBuildingByName
{
    public class GetBuildingByNameQueryHandler : IRequestHandler<GetBuildingByNameQuery, BuildingDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBuildingByNameQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BuildingDto?> Handle(GetBuildingByNameQuery request, CancellationToken cancellationToken)
        {
            var building = await _unitOfWork.Repository<Building>().Entities
                .AsNoTracking()
                .Include(b => b.Campus)
                .FirstOrDefaultAsync(b => b.BuildingName == request.BuildingName, cancellationToken);

            if (building == null)
                return null;

            return new BuildingDto
            {
                Id = building.Id,
                CampusId = building.CampusId,
                BuildingName = building.BuildingName,
                Description = building.Description,
                BuildingImageUrl = building.BuildingImageUrl,
                CampusName = building.Campus?.CampusName
            };
        }
    }
}
