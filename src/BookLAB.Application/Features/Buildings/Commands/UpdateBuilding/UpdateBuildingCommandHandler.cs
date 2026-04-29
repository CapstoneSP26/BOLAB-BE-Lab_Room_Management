using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Buildings.DTOs;
using BookLAB.Application.Features.Buildings.Queries.GetBuildings;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Buildings.Commands.UpdateBuilding
{
    public class UpdateBuildingCommandHandler : IRequestHandler<UpdateBuildingCommand, BuildingDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBuildingCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BuildingDto> Handle(UpdateBuildingCommand request, CancellationToken cancellationToken)
        {
            var buildingRepo = _unitOfWork.Repository<Building>();

            var building = await buildingRepo.Entities
                .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (building == null)
            {
                throw new NotFoundException($"Building with id {request.Id} not found");
            }

            var duplicateName = await buildingRepo.Entities
                .AsNoTracking()
                .AnyAsync(
                    b => b.Id != request.Id
                         && b.BuildingName == request.BuildingName
                         && b.CampusId == request.CampusId,
                    cancellationToken);

            if (duplicateName)
            {
                throw new BusinessException("A building with the same name already exists in this campus.");
            }

            building.CampusId = request.CampusId;
            building.BuildingName = request.BuildingName.Trim();
            building.Description = request.Description?.Trim() ?? string.Empty;
            building.BuildingImageUrl = request.BuildingImageUrl?.Trim() ?? string.Empty;

            buildingRepo.Update(building);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedBuilding = await buildingRepo.Entities
                .AsNoTracking()
                .Include(b => b.Campus)
                .Where(b => b.Id == request.Id)
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

            return updatedBuilding;
        }
    }
}
