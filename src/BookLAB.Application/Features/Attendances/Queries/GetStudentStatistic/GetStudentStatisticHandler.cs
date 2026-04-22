using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Attendances.Queries.GetStudentStatistic
{
    public class GetStudentStatisticHandler : IRequestHandler<GetStudentStatisticQuery, GetStudentStatisticReturn>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public GetStudentStatisticHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<GetStudentStatisticReturn> Handle(GetStudentStatisticQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;

                var now = DateTime.Now;
                int monthStart, monthEnd;

                if (now.Month >= 1 && now.Month <= 4)
                {
                    monthStart = 1;
                    monthEnd = 4;
                }
                else if (now.Month >= 5 && now.Month <= 8)
                {
                    monthStart = 5;
                    monthEnd = 8;
                }
                else
                {
                    monthStart = 9;
                    monthEnd = 12;
                }

                int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
                var startOfWeek = now.AddDays(-diff).Date;
                var endOfWeek = startOfWeek.AddDays(7);

                var userGroup = await _unitOfWork.Repository<GroupMember>().Entities.Where(x => x.UserId == userId).Select(x => x.GroupId).ToListAsync();
                var total = _unitOfWork.Repository<Schedule>().Entities.Where(x => x.StartTime.Month >= monthStart && x.StartTime.Month <= monthEnd && userGroup.Contains(x.GroupId.Value));

                var attendances = _unitOfWork.Repository<Attendance>().Entities
                    .Include(x => x.Schedule)
                    .Where(x => x.UserId == userId &&
                    x.Schedule.StartTime.Year == now.Year &&
                    x.Schedule.StartTime.Month >= monthStart &&
                    x.Schedule.StartTime.Month <= monthEnd);

                int attendInWeek = (int)Math.Round((double)(attendances.Where(x => x.AttendanceStatus == Domain.Enums.AttendanceStatus.Present && x.Schedule.StartTime >= startOfWeek && x.Schedule.StartTime < endOfWeek).Count() /
                    total.Where(x => x.StartTime >= startOfWeek && x.StartTime < endOfWeek).Count()) * 100);
                int classInSemesterLeft = total.Where(x => x.StartTime >= now).Count();

                return new GetStudentStatisticReturn
                {
                    AttendInWeek = attendInWeek,
                    ClassInSemesterLeft = classInSemesterLeft
                };
            }
            catch (Exception ex)
            {
                return new GetStudentStatisticReturn
                {
                    AttendInWeek = 0,
                    ClassInSemesterLeft = 0
                };
            }
        }
    }
}
