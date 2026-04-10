namespace BookLAB.Application.Features.LabRooms.Commands.UpdatePolicy
{
    public class LabRoomPolicyUpdateDto
    {
        public int LabRoomId { get; set; }
        public string PolicyKey { get; set; } = string.Empty;
        public string PolicyValue { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
