using BookLAB.Application.Common.Specifications;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.IncidentReports.Queries.GetReports
{
    public class ReportFilterSpecification : BaseSpecification<Report>
    {
        public ReportFilterSpecification(GetReportsQuery query)
        {
            // 1. Lọc theo tòa nhà

            //if (query.BuildingId.HasValue)
            //    AddCriteria(x => x.BuildingId == query.BuildingId.Value);

            //if (!string.IsNullOrWhiteSpace(query.RoomNo))
            //    AddCriteria(x => x.RoomNo == query.RoomNo);

            // 3. Chỉ lấy các phòng chưa bị xóa và đang hoạt động
            //AddCriteria(x => !x.IsDeleted && x.IsActive);

            // 4. Load các bảng liên quan

            //ApplyOrderBy(x => x.RoomName);

            if (!(query.Q == null) && query.Q.Length > 0)
                AddCriteria(x => x.Schedule.LabRoom.RoomName.Contains(query.Q) || x.ReportType.ReportTypeName.Contains(query.Q));

            if (query.BuildingId.HasValue)
                AddCriteria(x => x.Schedule.LabRoom.BuildingId == query.BuildingId);

            if (query.RoomId.HasValue)
                AddCriteria(x => x.Schedule.LabRoomId == query.RoomId);
        }
    }
}