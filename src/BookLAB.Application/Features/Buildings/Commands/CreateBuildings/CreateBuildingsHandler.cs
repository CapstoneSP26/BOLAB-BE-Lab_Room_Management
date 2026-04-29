using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Buildings.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Buildings.Commands.CreateBuildings
{
    public class CreateBuildingsHandler : IRequestHandler<CreateBuildingsCommand, PagedList<BuildingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateBuildingsHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PagedList<BuildingDto>> Handle(CreateBuildingsCommand request, CancellationToken cancellationToken)
        {
            var imageUrl = "";
            var wrootImageUrl = "";
            var buildingId = await _unitOfWork.Repository<Building>().Entities.MaxAsync(b => b.Id) + 1;
            var campusId = _currentUserService.CampusId;

            try
            {
                if (request.Images != null && request.Images.Length > 0)
                {
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
                
                var building = new Building
                {
                    Id = buildingId,
                    BuildingName = request.BuildingName,
                    Description = request.Descriptions,
                    BuildingImageUrl = imageUrl,
                    CampusId = campusId
                };

                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Repository<Building>().AddAsync(building);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return new PagedList<BuildingDto>(new List<BuildingDto>
                {
                    new BuildingDto
                    {
                        Id = buildingId,
                        BuildingName = building.BuildingName,
                        Description = building.Description,
                        BuildingImageUrl = building.BuildingImageUrl
                    }
                }, 1, 1, 1);

            } catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return null;
            }

            
        }
    }
}
