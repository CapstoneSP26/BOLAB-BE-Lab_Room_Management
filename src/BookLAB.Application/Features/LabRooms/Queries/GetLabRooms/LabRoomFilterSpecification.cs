using BookLAB.Application.Common.Specifications;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRooms
{
    public class LabRoomFilterSpecification : BaseSpecification<LabRoom>
    {
        public LabRoomFilterSpecification(GetLabRoomsQuery query)
        {
            // 1. Lọc theo tòa nhà
            if (query.LabRoomId.HasValue)
                AddCriteria(x => x.Id == query.LabRoomId.Value);

            if (query.BuildingId.HasValue)
                AddCriteria(x => x.BuildingId == query.BuildingId.Value);

            if (!string.IsNullOrWhiteSpace(query.RoomNo))
                AddCriteria(x => x.RoomNo == query.RoomNo);

            // 3. Chỉ lấy các phòng chưa bị xóa và đang hoạt động
            //AddCriteria(x => !x.IsDeleted && x.IsActive);

            // 4. Load các bảng liên quan
            AddInclude(x => x.Building);
            ApplyOrderBy(x => x.RoomName);
        }
    }
}