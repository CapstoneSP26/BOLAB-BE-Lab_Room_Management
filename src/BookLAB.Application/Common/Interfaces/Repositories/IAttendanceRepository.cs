using BookLAB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Interfaces.Repositories
{
    public interface IAttendanceRepository
    {
        Task<bool> AddAsync(Attendance attendance);
    }
}
