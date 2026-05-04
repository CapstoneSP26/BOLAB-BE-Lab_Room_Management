using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;

namespace BookLAB.Infrastructure.Repositories
{
    public class AttendanceReposiotry : IAttendanceRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceReposiotry(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> AddAsync(Attendance attendance)
        {
            try
            {
                await _unitOfWork.Repository<Attendance>().AddAsync(attendance);
                return true;
            } catch (Exception ex)
            {
                return false;
            }
            
        }
    }
}
