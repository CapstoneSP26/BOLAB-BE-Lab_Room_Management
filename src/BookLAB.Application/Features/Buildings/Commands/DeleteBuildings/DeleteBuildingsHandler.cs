using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Buildings.Commands.DeleteBuildings
{
    public class DeleteBuildingsHandler : IRequestHandler<DeleteBuildingsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteBuildingsHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(DeleteBuildingsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var building = await _unitOfWork.Repository<Building>().GetByIdAsync(request.Id);

                if (building == null)
                    return false;

                if (System.IO.File.Exists("wwwroot\\" +building.BuildingImageUrl))
                {
                    System.IO.File.Delete(building.BuildingImageUrl);
                }
                
                await _unitOfWork.BeginTransactionAsync();
                _unitOfWork.Repository<Building>().Delete(building);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return false;
            }
        }
    }
}
