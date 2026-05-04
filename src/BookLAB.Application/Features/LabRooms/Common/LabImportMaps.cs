using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.LabRooms.Common
{
    public class LabImportMaps
    {
        // Ánh xạ Mã tòa nhà (BuildingCode) -> Thực thể Building
        // Giúp tìm BuildingId từ Code trong file Excel
        public Dictionary<string, Building> BuildingMap { get; set; } = new();

        // Ánh xạ Mã phòng (RoomNo) -> Thực thể LabRoom hiện có
        // Dùng để kiểm tra xem phòng đã tồn tại để thực hiện Update hay chưa
        public Dictionary<string, LabRoom> LabRoomMap { get; set; } = new();

        // Danh sách các Mã phòng đã xuất hiện trong file Excel đang đọc
        // Dùng để phát hiện trường hợp trùng lặp dữ liệu ngay trong cùng một file import
        public HashSet<string> SeenRoomNos { get; set; } = new();
    }
}
