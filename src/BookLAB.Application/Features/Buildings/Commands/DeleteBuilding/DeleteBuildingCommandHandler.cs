using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Buildings.Commands.DeleteBuilding
{
    public class DeleteBuildingCommandHandler : IRequestHandler<DeleteBuildingCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBuildingCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteBuildingCommand request, CancellationToken cancellationToken)
        {
            var buildingRepo = _unitOfWork.Repository<Building>();

            var building = await buildingRepo.Entities
                .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (building == null)
            {
                throw new NotFoundException($"Building with id {request.Id} not found");
            }

            var hasActiveLabRooms = await _unitOfWork.Repository<LabRoom>().Entities
                .AsNoTracking()
                .AnyAsync(r => r.BuildingId == request.Id && !r.IsDeleted, cancellationToken);

            if (hasActiveLabRooms)
            {
                throw new BusinessException("Cannot delete a building that still has active lab rooms.");
            }

            buildingRepo.Delete(building);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
