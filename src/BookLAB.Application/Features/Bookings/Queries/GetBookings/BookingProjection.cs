using BookLAB.Application.Features.Bookings.Queries.GetPurposeTypes;
using BookLAB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.GetBookings
{
    public static class BookingProjection 
    {
        public static IQueryable<BookingDto> SelectPurposeType(this IQueryable<BookingRequest> query)
        {
            return query.Select(x => new BookingDto
            {
                Id = x.Id,
                LabRoomId = x.Booking.LabRoomId,
                LabRoomName = x.Booking.LabRoom.RoomName,
                UserFullName = x.Requester.FullName,
                UserEmail = x.Requester.Email,
                UserCode = x.Requester.UserCode,
                StartTime = x.Booking.StartTime,
                EndTime = x.Booking.EndTime,
                StudentCount = x.Booking.StudentCount,
                Reason = x.Booking.Reason,
                Status = x.Booking.BookingStatus,
                CreatedAt = x.Booking.CreatedAt,
            });
        }
    }
}
