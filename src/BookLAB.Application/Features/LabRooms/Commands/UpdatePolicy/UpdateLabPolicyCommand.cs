using MediatR;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;

namespace BookLAB.Application.Features.LabRooms.Commands.UpdatePolicy
{
    public class UpdateLabPolicyCommand : IRequest<LabRoomPolicyUpdateDto>
    {
        public int LabRoomId { get; set; }
        public PolicyType PolicyKey { get; set; }

        // Các trường từ Payload
        public string? PolicyValue { get; set; }
        public bool? IsActive { get; set; }
    }
}
