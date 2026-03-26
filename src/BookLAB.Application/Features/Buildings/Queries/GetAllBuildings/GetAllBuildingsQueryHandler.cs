using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Buildings.DTOs;
using BookLAB.Application.Features.Buildings.Queries.GetBuildingByName;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Buildings.Queries.GetAllBuildings
{
    public class GetAllBuildingsQueryHandler : IRequestHandler<GetAllBuildingsQuery, List<BuildingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllBuildingsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BuildingDto>> Handle(GetAllBuildingsQuery request, CancellationToken cancellationToken)
        {
            var buildings = await _unitOfWork.Repository<Building>().Entities
                .AsNoTracking()
                .Include(b => b.Campus)
                .ToListAsync(cancellationToken);

            return buildings.Select(b => new BuildingDto
            {
                Id = b.Id,
                CampusId = b.CampusId,
                BuildingName = b.BuildingName,
                Description = b.Description,
                BuildingImageUrl = b.BuildingImageUrl,
                CampusName = b.Campus?.CampusName
            }).ToList();
        }
    }
}