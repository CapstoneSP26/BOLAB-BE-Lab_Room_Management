using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Integration;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Bookings.Events;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BookLAB.Application.Features.Bookings.Commands.CancelBooking
{
    public class CancelBookingHandler : IRequestHandler<CancelBookingCommand, ResultMessage<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        private readonly IMediator _mediator;

        public CancelBookingHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            INotificationService notificationService,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
            _mediator = mediator;
        }

        public async Task<ResultMessage<bool>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId ?? Guid.Empty;
            var now = DateTimeOffset.UtcNow;

            // 💡 request.BookingId lúc này đóng vai trò là "TargetId" đa năng nhận diện từ lưới Calendar
            var targetId = request.BookingId;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // ====================================================================================
                // 🔎 BƯỚC 1: ĐA NHIỆM TRUY VẤN - TỰ ĐỘNG PHÁT HIỆN THỰC THỂ THỰC TẾ TRONG DB
                // ====================================================================================

                // Thử tìm Schedule trực tiếp (Phục vụ trường hợp targetId chính là ScheduleId hoặc BookingId của lịch đã duyệt)
                var schedule = await _unitOfWork.Repository<Schedule>().Entities
                    .FirstOrDefaultAsync(s => s.Id == targetId || s.BookingId == targetId, cancellationToken);

                // Thử tìm Booking (Nếu targetId là BookingId hoặc thông qua mối quan hệ từ Schedule tìm thấy)
                var booking = await _unitOfWork.Repository<Booking>().Entities
                    .Include(b => b.LabRoom)
                    .Include(b => b.PurposeType)
                    .FirstOrDefaultAsync(b => b.Id == targetId || (schedule != null && b.Id == schedule.BookingId), cancellationToken);

                // Chặn lỗi cơ bản: Nếu cả 2 đều không tìm thấy băn đơn bốc hơi
                if (booking == null && schedule == null)
                {
                    return new ResultMessage<bool> { Success = false, Message = "Không tìm thấy thông tin yêu cầu đặt phòng hoặc lịch trình trên lưới." };
                }

                // Trích xuất thông tin định danh phòng Lab phục vụ kiểm tra phân quyền vĩ mô
                int labRoomId = booking?.LabRoomId ?? schedule?.LabRoomId ?? 0;
                string roomName = booking?.LabRoom?.RoomName ?? "Phòng máy";

                // ====================================================================================
                // 🔐 BƯỚC 2: KIỂM TRA BẢO MẬT PHÂN TẦNG (CHÍNH CHỦ HOẶC TRƯỞNG PHÒNG LAB MỞ RỘNG)
                // ====================================================================================
                bool isCreatedByMe = (booking != null && booking.CreatedBy == userId) || (schedule != null && schedule.CreatedBy == userId);
                bool isLabOwner = await _unitOfWork.LabOwners.IsUserOwnerAsync(labRoomId, userId);

                // Trích xuất cấp độ ưu tiên của lịch định tác động để phục vụ chặn đè quyền tối cao
                int targetPriorityLevel = booking?.PurposeType?.PriorityLevel
                                          ?? (schedule?.SchedulePriority == SchedulePriority.SCHOOL_EVENT ? 3 :
                                              schedule?.SchedulePriority == SchedulePriority.ACADEMIC ? 2 : 1);

                // Bộ lọc 1: Nếu không phải chính chủ tạo, cũng không phải Trưởng Lab quản lý phòng -> CHẶN ĐỨNG THẲNG LẬP TỨC
                if (!isCreatedByMe && !isLabOwner)
                {
                    return new ResultMessage<bool>
                    {
                        Success = false,
                        Message = "Bảo mật: Bạn không có quyền sở hữu đơn đặt này hoặc không phải là Trưởng phòng Lab quản lý căn phòng này.",
                    };
                }

                // Bộ lọc 2 (Chốt chặn tối cao): Trưởng phòng Lab dù quản lý phòng vật lý cũng KHÔNG ĐƯỢC PHÉP tự ý hủy lịch thi/sự kiện trường của PĐT (Mức 3)
                if (isLabOwner && !isCreatedByMe && targetPriorityLevel == 3)
                {
                    return new ResultMessage<bool>
                    {
                        Success = false,
                        Message = "Từ chối thao tác hành chính: Đây là Lịch sự kiện/Lịch thi độc quyền do Phòng Đào tạo thiết lập, Trưởng phòng Lab không có quyền hủy bỏ lịch trình này.",
                    };
                }

                // Ghi nhận cờ để biết hành động hủy này có phải do Admin phòng Lab ra quyết định hay không
                bool isCancelledByAdmin = isLabOwner && !isCreatedByMe;

                // ====================================================================================
                // 🚨 BƯỚC 3: ĐÁNH CHẶN NGHIỆP VỤ PHÂN TẦNG ĐỘNG (CHỈ ÉP LÝ DO VÀ FREEZE WINDOW KHI ĐÃ LÀ SCHEDULE)
                // ====================================================================================
                if (schedule != null) // Trường hợp lịch đã được phê duyệt lên lưới công cộng hoặc là lịch Excel Import
                {
                    // Chốt A: Kiểm tra Freeze Window (Phải hủy TRƯỚC khi giờ học bắt đầu xảy ra thực tế)
                    if (now >= schedule.StartTime)
                    {
                        return new ResultMessage<bool>
                        {
                            Success = false,
                            Message = "Không thể thực hiện hủy lịch trình. Thời gian bắt đầu sử dụng phòng máy đã hoặc đang diễn ra."
                        };
                    }

                    // Chốt B: Phá vỡ một cam kết phòng máy công cộng bắt buộc phải cung cấp lý do giải trình rõ ràng
                    if (string.IsNullOrWhiteSpace(request.CancelReason))
                    {
                        return new ResultMessage<bool>
                        {
                            Success = false,
                            Message = "Hủy lịch chính thức thất bại. Bạn bắt buộc phải cung cấp lý do hủy để lưu vết Auditing."
                        };
                    }
                }
                // Nếu schedule == null (Đơn đặt phòng vẫn nằm ở hàng đợi Chờ duyệt) -> Cho phép bấm hủy tự do, không bắt nhập lý do để tối ưu UX

                // ====================================================================================
                // 📡 BƯỚC 4: QUÉT TRACE LOG PHỤC HỒI NẠN NHÂN BỊ ĐÈ LỊCH TRONG QUÁ KHỨ (CHỈ CHẠY NẾU HỦY ĐƠN LỚN)
                // ====================================================================================
                var recoveredScheduleIds = new List<Guid>();
                var recoveredBookingIds = new List<Guid>();

                if (booking != null)
                {
                    // Tìm kiếm các lịch chính thức từng bị xóa mềm oan uổng bởi đơn đặt này trước đây
                    var affectedSchedules = await _unitOfWork.Repository<Schedule>().Entities
                        .IgnoreQueryFilters() // Bẻ gãy query filter xóa mềm của EF Core để lôi IsDeleted = true lên
                        .Where(s => s.AutoCancelledByBookingId == booking.Id)
                        .ToListAsync(cancellationToken);

                    recoveredScheduleIds = affectedSchedules.Select(s => s.Id).ToList();

                    // Tìm kiếm các yêu cầu đặt phòng từng bị từ chối tự động (Auto-Rejected)
                    var affectedBookings = await _unitOfWork.Repository<Booking>().Entities
                        .Where(b => b.AutoRejectedByBookingId == booking.Id && b.BookingStatus == BookingStatus.Rejected)
                        .ToListAsync(cancellationToken);

                    recoveredBookingIds = affectedBookings.Select(b => b.Id).ToList();

                    // Thực hiện gỡ vết tích dọn rác DB giải phóng trạng thái
                    foreach (var sch in affectedSchedules)
                    {
                        sch.AutoCancelledByBookingId = null;
                        _unitOfWork.Repository<Schedule>().Update(sch);
                    }
                    foreach (var bk in affectedBookings)
                    {
                        bk.AutoRejectedByBookingId = null;
                        _unitOfWork.Repository<Booking>().Update(bk);
                    }
                }

                // ====================================================================================
                // 💾 BƯỚC 5: ĐỒNG BỘ CẬP NHẬT TRẠNG THÁI TRONG DATABASE (AUDITING LOG)
                // ====================================================================================

                // 1. Cập nhật thực thể lịch Schedule chính thức (Nếu có)
                if (schedule != null)
                {
                    schedule.ScheduleStatus = ScheduleStatus.Cancelled;
                    schedule.IsActive = false;
                    schedule.IsDeleted = true; // Xóa mềm để lưới Calendar công cộng tự động ẩn đi

                    // Ghi dấu vết Auditing Log trực tiếp vào bảng Schedule phục vụ tra cứu vĩnh viễn
                    schedule.CancelledBy = userId;
                    schedule.CancelReason = request.CancelReason;
                    _unitOfWork.Repository<Schedule>().Update(schedule);
                }

                // 2. Cập nhật thực thể Booking (Nếu có)
                if (booking != null)
                {
                    booking.BookingStatus = BookingStatus.Cancelled;
                    _unitOfWork.Repository<Booking>().Update(booking);
                }

                // 3. Cập nhật bảng phụ tiến trình BookingRequest tinh gọn (Không ghi đè trường CancelReason thừa ở đây)
                if (booking != null)
                {
                    var bookingRequest = await _unitOfWork.Repository<BookingRequest>().Entities
                        .FirstOrDefaultAsync(x => x.BookingId == booking.Id, cancellationToken);

                    if (bookingRequest != null)
                    {
                        bookingRequest.BookingRequestStatus = BookingRequestStatus.Cancelled;
                        bookingRequest.UpdatedAt = now;
                        bookingRequest.UpdatedBy = userId;
                        _unitOfWork.Repository<BookingRequest>().Update(bookingRequest);
                    }
                }

                // ====================================================================================
                // 🔔 BƯỚC 6: KHỞI TẠO THÔNG BÁO THỜI GIAN THỰC CHO BAN QUẢN LÝ (LAB OWNERS KHÁC)
                // ====================================================================================
                var managerNotifications = new List<Notification>();
                if (labRoomId > 0)
                {
                    var metadataObject = new { bookingId = targetId, labRoomId = labRoomId };
                    var metadataJsonString = JsonSerializer.Serialize(metadataObject);

                    var ownerIds = await _unitOfWork.LabOwners.GetOwnerIdsByLabRoomIdAsync(labRoomId);
                    foreach (var managerId in ownerIds.Distinct())
                    {
                        if (managerId == userId) continue; // Né chính bản thân người vừa bấm nút hủy ra

                        var managerNotification = new Notification
                        {
                            UserId = managerId,
                            Title = isCancelledByAdmin ? "Trưởng phòng máy dùng quyền hủy lịch" : "Người dùng tự rút đơn/hủy lịch",
                            Message = isCancelledByAdmin
                                ? $"Đồng nghiệp quản lý đã dùng quyền hạn Admin để hủy lịch của phòng {roomName}. Lý do giải trình: {request.CancelReason}"
                                : $"Lịch trình/yêu cầu tại phòng {roomName} đã được người dùng chủ động hủy bỏ. Lý do: {(string.IsNullOrWhiteSpace(request.CancelReason) ? "Rút đơn chờ duyệt" : request.CancelReason)}",
                            Type = "BookingCancelled",
                            IsRead = false,
                            CreatedAt = now,
                            Metadata = JsonDocument.Parse(metadataJsonString).RootElement.Clone(),
                            IsGlobal = false
                        };
                        managerNotifications.Add(managerNotification);
                        await _unitOfWork.Repository<Notification>().AddAsync(managerNotification);
                    }
                }

                // Thực hiện lưu trữ đồng bộ dữ liệu an toàn xuống database vật lý
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(); // 🔒 Chốt Transaction thành công trước khi đẩy Job Mail chạy ngầm bên ngoài

                // ====================================================================================
                // 📡 BƯỚC 7: REALTIME NOTIFICATION CLIENT & TRIGGER MEDIATR EMAIL WORKER
                // ====================================================================================
                await _notificationService.NotifyBookingChangedAsync(userId, new { action = "cancelled", bookingId = targetId, occurredAt = now }, cancellationToken);

                foreach (var managerNotification in managerNotifications)
                {
                    if (managerNotification.UserId is Guid managerUserId)
                    {
                        await _notificationService.NotifyNotificationCreatedAsync(managerUserId, new
                        {
                            id = managerNotification.Id,
                            type = managerNotification.Type,
                            title = managerNotification.Title,
                            message = managerNotification.Message,
                            isRead = managerNotification.IsRead,
                            createdAt = managerNotification.CreatedAt,
                            metadata = managerNotification.Metadata
                        }, cancellationToken);
                    }
                }

                // Phát Event MediatR nạp tác vụ gửi Mail giải trình khẩn cấp và Mail phục hồi nạn nhân cho Hangfire Job chạy ngầm
                await _mediator.Publish(new BookingCancelledEvent(
                    targetId,
                    labRoomId,
                    recoveredBookingIds,
                    recoveredScheduleIds,
                    isCancelledByAdmin,
                    userId), cancellationToken);

                return new ResultMessage<bool> { Success = true, Message = "Xử lý hủy lịch trình/yêu cầu thành công.", Data = true };
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ResultMessage<bool> { Success = false, Message = "Có lỗi hệ thống xảy ra trong quá trình xử lý lệnh hủy.", Data = false };
            }
        }
    }
}