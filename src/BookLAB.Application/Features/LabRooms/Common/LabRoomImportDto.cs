namespace BookLAB.Application.Features.LabRooms.Common
{
    public class LabRoomImportDto
    {
        public string RoomNo { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string BuildingCode { get; set; } = string.Empty;
        public bool HasEquipment { get; set; } // Chuyển từ string sang bool
        public int Capacity { get; set; }
        public bool IsUpdated { get; set; }
    }
}
