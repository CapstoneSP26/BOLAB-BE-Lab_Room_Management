using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Interfaces.Services
{
    public interface IBookingService
    {
        public Task<List<Booking>> GetBookingHistoryByUserIdAsync(Guid userId, int page, int limit, string status, DateTimeOffset startDate, DateTimeOffset endDate);
    }
}
