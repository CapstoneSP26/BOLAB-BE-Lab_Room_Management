using BookLAB.Domain.Enums;
using System.Drawing;

namespace BookLAB.Application.Features.LabRooms.Commands.UpdatePolicy
{
    public class LabRoomPolicyUpdatePayload
    {
        public string? PolicyValue { get; set; }
        public bool? IsActive { get; set; }
    }
}
