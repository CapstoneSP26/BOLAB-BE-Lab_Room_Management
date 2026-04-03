namespace BookLAB.Application.Features.LabRooms.Common
{
    public class LabRoomImportDto
    {
        public int BuildingId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string RoomNo { get; set; } = string.Empty;
        public string? Location { get; set; }
        public int Capacity { get; set; }
        public bool HasEquipment { get; set; }
        public int OverrideNumber { get; set; }
        public string? Description { get; set; }
    }
}
