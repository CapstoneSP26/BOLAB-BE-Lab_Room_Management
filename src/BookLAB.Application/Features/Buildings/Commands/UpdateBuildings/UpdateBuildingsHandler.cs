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
            var wrootImageUrl = "wwwroot\\" + building.BuildingImageUrl;

            if (building == null)
                return null;

            try
            {
                if (request.ImagesUrl != null && (request.ImagesUrl.EndsWith("Buildings") || request.ImagesUrl.EndsWith("Buildings\\")) && request.ImagesUrl != building.BuildingImageUrl)
                {
                    if (System.IO.File.Exists("wwwroot\\" + building.BuildingImageUrl))
                        System.IO.File.Delete("wwwroot\\" + building.BuildingImageUrl);


                    wrootImageUrl = Path.Combine("wwwroot", "Uploads", "Buildings", request.Images.FileName);
                    imageUrl = Path.Combine("Uploads", "Buildings", request.Images.FileName);

                    var folderPath = Path.Combine("wwwroot", "Uploads", "Buildings");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    using (var stream = new FileStream(wrootImageUrl, FileMode.Create))
                    {
                        await request.Images.CopyToAsync(stream);
                    }
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
