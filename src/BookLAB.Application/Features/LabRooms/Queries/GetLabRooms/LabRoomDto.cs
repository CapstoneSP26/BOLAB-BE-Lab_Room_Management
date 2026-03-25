namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRooms
{
    public class LabRoomDto
    {
        public int Id { get; init; }
        public string RoomName { get; init; } = null!;
        public string RoomNo { get; init; } = null!;
        public string? Location { get; init; }
        public int Capacity { get; init; }
        public bool HasEquipment { get; init; }
        public string? Description { get; init; }

        // Thông tin từ bảng liên quan
        public string BuildingName { get; init; } = null!;
    }
}
