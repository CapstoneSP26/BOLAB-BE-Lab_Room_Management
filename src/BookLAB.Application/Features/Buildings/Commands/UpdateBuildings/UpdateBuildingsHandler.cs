using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Buildings.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Buildings.Commands.UpdateBuildings
{
    public class UpdateBuildingsHandler : IRequestHandler<UpdateBuildingsCommand, PagedList<BuildingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateBuildingsHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PagedList<BuildingDto>> Handle(UpdateBuildingsCommand request, CancellationToken cancellationToken)
        {
            var building = await _unitOfWork.Repository<Building>().GetByIdAsync(request.Id);
            var imageUrl = building.BuildingImageUrl;

            if (building == null)
                return null;

            try
            {
                if (request.Images != null && request.Images.Length > 0)
                {
                    if (System.IO.File.Exists(building.BuildingImageUrl))
                        System.IO.File.Delete(building.BuildingImageUrl);

                    imageUrl = Path.Combine("Uploads/Buildings", request.Images.FileName);
                }
                
                using (var stream = new FileStream(imageUrl, FileMode.Create))
                {
                    await request.Images.CopyToAsync(stream);
                }

                building.BuildingName = request.BuildingName;
                building.Description = request.Descriptions;
                building.BuildingImageUrl = imageUrl;

                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Repository<Building>().UpdateAsync(building);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return new PagedList<BuildingDto>(new List<BuildingDto>
                {
                    new BuildingDto
                    {
                        Id = building.Id,
                        BuildingName = building.BuildingName,
                        Description = building.Description,
                        BuildingImageUrl = building.BuildingImageUrl
                    }
                }, 1, 1, 1);

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return null;
            }
        }
    }
}
