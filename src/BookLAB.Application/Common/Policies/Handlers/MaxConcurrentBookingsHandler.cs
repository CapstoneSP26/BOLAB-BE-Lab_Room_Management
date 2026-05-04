using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Bookings.Commands.CreateBooking;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Policies.Handlers
{
    public class MaxConcurrentBookingsHandler : IPolicyHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public PolicyType PolicyType => PolicyType.MaxConcurrentBookings;

        public MaxConcurrentBookingsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PolicyValidationResult> ValidateAsync(CreateBookingCommand request, string value)
        {
            int maxCapacity = 1; // Default

            if (!string.IsNullOrWhiteSpace(value) &&
                int.TryParse(value.Trim(), out var parsed))
            {
                maxCapacity = parsed;
            }

            // Convert request time về UTC để đồng bộ với DB
            var requestStartUtc = request.StartTime.ToUniversalTime();
            var requestEndUtc = request.EndTime.ToUniversalTime();

            var currentCount = await _unitOfWork.Repository<Schedule>().Entities
                .CountAsync(x =>
                    x.LabRoomId == request.LabRoomId &&
                    x.StartTime < requestEndUtc &&
                    x.EndTime > requestStartUtc
                );

            if (currentCount >= maxCapacity)
            {
                return new PolicyValidationResult(
                    false,
                    "Phòng đã đạt giới hạn số lượt đặt trùng giờ."
                );
            }

            return new PolicyValidationResult(true);
        }
    }
}