using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Infrastructure.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task<bool> AddScheduleAsync(Schedule schedule)
        {
            return await _scheduleRepository.AddScheduleAsync(schedule);
        }

        public async Task<bool> CheckConflictAsync(int roomId, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            if (startTime > endTime) return true;
            
            var conflicts = await _scheduleRepository.CheckConflictAsync(roomId, startTime, endTime);

            return conflicts;
        }
    }
}
