using AutoMapper;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Buildings.Queries.GetBuildingsInCampus
{
    public class GetBuildingsInCampusHandler : IRequestHandler<GetBuildingsInCampusCommand, List<BuildingResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetBuildingsInCampusHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<BuildingResponse>> Handle(GetBuildingsInCampusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.Repository<Building>().Entities.Where(b => b.CampusId == request.campusId).ToListAsync();

                var resultMapper = _mapper.Map<List<Building>, List<BuildingResponse>>(result);

                return resultMapper.ToList();
            } catch (Exception ex)
            {
                return new List<BuildingResponse>();
            }
            
        }
    }
}
