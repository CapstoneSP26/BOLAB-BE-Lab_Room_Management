using AutoMapper;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Features.Schedules.Queries.AddSchedule;
using BookLAB.Application.Features.Schedules.Queries.GetSchedules;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Queries.GetCurrentScheduleInRoom
{
    public class GetCurrentScheduleInRoomHandler : IRequestHandler<GetCurrentScheduleInRoomQuery, List<ScheduleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetCurrentScheduleInRoomHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<List<ScheduleDto>> Handle(GetCurrentScheduleInRoomQuery request, CancellationToken cancellationToken)
        {
            var currentTime = DateTimeOffset.UtcNow;
            var schedules = await _unitOfWork.Repository<Schedule>().Entities
                .Include(x => x.User)
                .Include(x => x.LabRoom)
                .Include(x => x.Group)
                .Where(s => s.LabRoom.RoomNo.ToLower().Equals(request.roomNo.ToLower()) && 
                s.StartTime <= currentTime && s.EndTime >= currentTime).ToListAsync(cancellationToken);

            var scheduleDtos = _mapper.Map<List<ScheduleDto>>(schedules);
            return scheduleDtos;
        }
    }
}
