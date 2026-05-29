using BookLAB.Application.Common.Interfaces.Jobs;
using BookLAB.Application.Common.Interfaces.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BookLAB.Application.Features.Bookings.Events
{
    public class BookingCancelledEventHandler : INotificationHandler<BookingCancelledEvent>
    {
        private readonly IBackgroundJobService _jobService;

        public BookingCancelledEventHandler(IBackgroundJobService jobService)
        {
            _jobService = jobService;
        }

        public Task Handle(BookingCancelledEvent notification, CancellationToken cancellationToken)
        {
            // 🚨 Đẩy Job gửi email khẩn cấp/thông báo cho chính chủ sở hữu lịch trình
            _jobService.Enqueue<ICancelBookingEmailJob>(
                x => x.Execute(
                    notification.TargetId,
                    notification.IsCancelledByAdmin,
                    notification.ActionByUserId));

            // 🚨 Đẩy Job gửi email khôi phục phòng trống cho tất cả các nạn nhân bị đè lịch trong quá khứ
            if (notification.AutoCancelledScheduleIds.Count > 0 || notification.AutoRejectedBookingIds.Count > 0)
            {
                _jobService.Enqueue<IRecoverOanBookingEmailJob>(
                    x => x.Execute(
                        notification.LabRoomId,
                        notification.AutoCancelledScheduleIds,
                        notification.AutoRejectedBookingIds));
            }

            return Task.CompletedTask;
        }
    }
}