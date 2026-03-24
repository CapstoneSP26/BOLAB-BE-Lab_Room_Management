using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRoomPolicies
{
    public class GetLabRoomPoliciesQueryHandler : IRequestHandler<GetLabRoomPoliciesQuery, List<LabRoomPolicyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLabRoomPoliciesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<LabRoomPolicyDto>> Handle(GetLabRoomPoliciesQuery request, CancellationToken ct)
        {
            var labRoom = await _unitOfWork.Repository<LabRoom>().Entities
                .Include(l => l.RoomPolicies)
                .FirstOrDefaultAsync(l => l.Id == request.LabRoomId, ct);

            if(labRoom == null)
            {
                throw new Exception($"Lab room with ID {request.LabRoomId} not found.");
            }

            var policies = labRoom.RoomPolicies.Select(p => new LabRoomPolicyDto
            {
                PolicyKey = p.PolicyKey,
                Value = p.PolicyValue
            }).ToList();

            return policies;
        }
    }
}
