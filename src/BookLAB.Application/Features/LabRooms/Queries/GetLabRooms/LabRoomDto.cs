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
        public int BuildingId { get; set; }
        public string BuildingName { get; init; } = null!;
        public bool IsActive { get; set; }

        public List<LabImageDto>? Images { get; init; }
    }
    public record LabImageDto
    {
        public Guid Id { get; init; }
        public string Url { get; init; } = null!;
        public bool IsPrimary { get; init; }
    }
}
