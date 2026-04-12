using BookLAB.Application.Common.Specifications;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRooms
{
    public class LabRoomFilterSpecification : BaseSpecification<LabRoom>
    {
        public LabRoomFilterSpecification(GetLabRoomsQuery query)
        {
            if (query.BuildingId.HasValue)
                AddCriteria(x => x.BuildingId == query.BuildingId.Value);

            if (query.IsActive.HasValue)
                AddCriteria(x => x.IsActive == query.IsActive.Value);

            if (!string.IsNullOrWhiteSpace(query.RoomNo))
                AddCriteria(x => x.RoomNo == query.RoomNo);

            // 3. Chỉ lấy các phòng chưa bị xóa và đang hoạt động
            //AddCriteria(x => !x.IsDeleted && x.IsActive);

            if (query.IsActive != null)
                AddCriteria(x => x.IsActive == query.IsActive);

            // 4. Load các bảng liên quan

            ApplyOrderBy(x => x.RoomName);
        }
    }
}