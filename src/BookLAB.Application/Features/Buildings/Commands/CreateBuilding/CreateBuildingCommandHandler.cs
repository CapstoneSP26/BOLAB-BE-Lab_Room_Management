using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Buildings.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Buildings.Commands.CreateBuilding
{
    public class CreateBuildingCommandHandler : IRequestHandler<CreateBuildingCommand, BuildingDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateBuildingCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BuildingDto> Handle(CreateBuildingCommand request, CancellationToken cancellationToken)
        {
            var buildingRepo = _unitOfWork.Repository<Building>();

            var duplicate = await buildingRepo.Entities
                .AsNoTracking()
                .AnyAsync(
                    b => b.CampusId == request.CampusId
                         && b.BuildingName == request.BuildingName,
                    cancellationToken);

            if (duplicate)
            {
                throw new BusinessException("A building with the same name already exists in this campus.");
            }

            var building = new Building
            {
                CampusId = request.CampusId,
                BuildingName = request.BuildingName.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                BuildingImageUrl = request.BuildingImageUrl?.Trim() ?? string.Empty,
            };

            await buildingRepo.AddAsync(building);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var createdBuilding = await buildingRepo.Entities
                .AsNoTracking()
                .Include(b => b.Campus)
                .Where(b => b.Id == building.Id)
                .Select(b => new BuildingDto
                {
                    Id = b.Id,
                    CampusId = b.CampusId,
                    BuildingId = b.Id,
                    BuildingName = b.BuildingName,
                    Description = b.Description,
                    BuildingImageUrl = b.BuildingImageUrl,
                    RoomCount = 0,
                    CampusName = b.Campus.CampusName
                })
                .FirstAsync(cancellationToken);

            return createdBuilding;
        }
    }
}
