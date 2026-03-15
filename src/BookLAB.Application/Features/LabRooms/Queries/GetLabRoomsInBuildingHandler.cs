using AutoMapper;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.LabRooms.Queries
{
    public class GetLabRoomsInBuildingHandler : IRequestHandler<GetLabRoomsInBuildingCommand, List<LabRoomRequest>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetLabRoomsInBuildingHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<LabRoomRequest>> Handle(GetLabRoomsInBuildingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.Repository<LabRoom>().Entities.Where(lr => lr.BuildingId.ToString().Equals(request.buildingId)).ToListAsync();

                var resultMapper = _mapper.Map<List<LabRoom>, List<LabRoomRequest>>(result);

                return resultMapper;
            } catch (Exception ex)
            {
                return new List<LabRoomRequest>();
            }
            
        }
    }
}
