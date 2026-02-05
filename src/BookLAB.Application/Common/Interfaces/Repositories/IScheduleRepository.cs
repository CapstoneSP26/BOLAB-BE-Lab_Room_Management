using BookLAB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Interfaces.Repositories
{
    public interface IScheduleRepository
    {
        Task<bool> CheckConflictAsync(int roomId, DateTimeOffset startTime, DateTimeOffset endTime);
        Task<bool> AddScheduleAsync(Schedule schedule);
    }
}
