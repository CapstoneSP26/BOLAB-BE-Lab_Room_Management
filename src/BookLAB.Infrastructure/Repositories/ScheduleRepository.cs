using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Infrastructure.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly BookLABDbContext _context;

        public ScheduleRepository(BookLABDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckConflictAsync(int roomId, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            return await _context.Schedules.AnyAsync(s => 
                s.LabRoomId.Equals(roomId) &&                                   // Check for the same room
                (((startTime < s.EndTime) && (startTime > s.StartTime)) ||      // Start time overlaps
                ((endTime > s.StartTime) && (endTime < s.EndTime))));           // End time overlaps
        }

        public async Task<bool> AddScheduleAsync(Schedule schedule)
        {
            await _context.Schedules.AddAsync(schedule);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
