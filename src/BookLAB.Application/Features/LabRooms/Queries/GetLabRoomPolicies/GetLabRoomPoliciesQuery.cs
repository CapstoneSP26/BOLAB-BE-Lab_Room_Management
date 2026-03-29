using MediatR;

namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRoomPolicies
{
    public record GetLabRoomPoliciesQuery : IRequest<List<LabRoomPolicyDto>>
    {
        public int LabRoomId { get; init; }
    }
}
