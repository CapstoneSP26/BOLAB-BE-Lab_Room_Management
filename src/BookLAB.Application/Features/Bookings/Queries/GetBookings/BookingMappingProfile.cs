using AutoMapper;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;

namespace BookLAB.Application.Features.Bookings.Queries.GetBookings
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            CreateMap<BookingRequest, BookingDto>()
                // 1. Map ID của Request (để FE gọi API Approve/Reject)
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))

                // 2. Truy cập xuyên qua Entity Booking để lấy thông tin phòng
                .ForMember(d => d.LabRoomId, opt => opt.MapFrom(s => s.Booking.LabRoomId))
                .ForMember(d => d.LabRoomName, opt => opt.MapFrom(s => s.Booking.LabRoom.RoomName))

                // 3. Thông tin người đặt (thường lấy từ Booking hoặc trực tiếp từ Request tùy thiết kế của bạn)
                .ForMember(d => d.UserFullName, opt => opt.MapFrom(s => s.Requester.FullName))
                .ForMember(d => d.UserEmail, opt => opt.MapFrom(s => s.Requester.Email))

                // 4. Thông tin thời gian và lý do
                .ForMember(d => d.StartTime, opt => opt.MapFrom(s => s.Booking.StartTime))
                .ForMember(d => d.EndTime, opt => opt.MapFrom(s => s.Booking.EndTime))
                .ForMember(d => d.StudentCount, opt => opt.MapFrom(s => s.Booking.StudentCount))
                .ForMember(d => d.Reason, opt => opt.MapFrom(s => s.Booking.Reason)) // Giả sử Content trong Request là lý do

                // 5. Trạng thái (Map từ Enum sang String)
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Booking.BookingStatus.ToString()))
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.CreatedAt))

                // 6. Logic tính toán cho UI
                .ForMember(d => d.IsOverdue, opt => opt.MapFrom(s =>
                    s.Booking.BookingStatus == BookingStatus.PendingApproval && s.Booking.StartTime < DateTime.UtcNow));
        }
    }
}
