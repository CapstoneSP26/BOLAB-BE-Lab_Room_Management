using AutoMapper;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Queries.GetSchedulesStudent
{
    public class GetSchedulesStudentHandler : IRequestHandler<GetSchedulesStudentQuery, List<ScheduleDto2>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetSchedulesStudentHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<List<ScheduleDto2>> Handle(GetSchedulesStudentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;
                var now = DateTimeOffset.UtcNow;

                var userGroup = await _unitOfWork.Repository<GroupMember>().Entities.Where(x => x.UserId == userId).Select(x => x.GroupId).ToListAsync();
                var total = await _unitOfWork.Repository<Schedule>().Entities
                    .Include(x => x.LabRoom)
                    .Include(x => x.User)
                    .Include(x => x.SlotType)
                    .Where(x => x.StartTime > now && userGroup.Contains(x.GroupId.Value)).ToListAsync();

                var mappedTotal = _mapper.Map<List<Schedule>, List<ScheduleDto2>>(total);

                return mappedTotal;
            } catch (Exception ex)
            {
                return new List<ScheduleDto2>();
            }
        }
    }
}
