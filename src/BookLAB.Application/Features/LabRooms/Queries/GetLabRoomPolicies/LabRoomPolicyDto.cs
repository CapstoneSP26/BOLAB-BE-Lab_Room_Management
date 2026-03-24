using BookLAB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRoomPolicies
{
    public class LabRoomPolicyDto
    {
        public PolicyType PolicyKey { get; init; } // Trả về Enum (hoặc Int/String tùy cấu hình)
        public string PolicyKeyName => PolicyKey.ToString(); // Tên chuỗi để FE dễ đọc
        public string Value { get; init; } = string.Empty;
    }
}
