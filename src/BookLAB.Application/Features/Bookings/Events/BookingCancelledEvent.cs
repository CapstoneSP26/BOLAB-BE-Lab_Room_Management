using MediatR;
using System;
using System.Collections.Generic;

namespace BookLAB.Application.Features.Bookings.Events
{
    public record BookingCancelledEvent(
        Guid TargetId,                  // BookingId hoặc ScheduleId chủ thể bị hủy
        int LabRoomId,                  // ID phòng Lab
        List<Guid> AutoRejectedBookingIds,   // Danh sách đơn bị từ chối oan
        List<Guid> AutoCancelledScheduleIds, // Danh sách lịch chính thức bị hủy oan
        bool IsCancelledByAdmin,        // True: Trưởng Lab hủy | False: Chính chủ tự hủy
        Guid ActionByUserId             // ID người bấm nút thao tác thực tế
    ) : INotification;
}