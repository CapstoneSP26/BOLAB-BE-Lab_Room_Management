using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Queries.SearchFreeSlots
{
    public class SearchFreeSlotsHandler : IRequestHandler<SearchFreeSlotsQuery, ResultMessage<List<ScheduleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchFreeSlotsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultMessage<List<ScheduleDto>>> Handle(SearchFreeSlotsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Schedule> schedules = _unitOfWork.Repository<Schedule>().Entities.Include(x => x.LabRoom);

            if (request.BuildingId.HasValue)
                schedules = schedules.Where(s => s.LabRoom.BuildingId == request.BuildingId.Value);

            if (request.LabRoomId.HasValue)
                schedules = schedules.Where(s => s.LabRoomId == request.LabRoomId.Value);

            if (request.StartTime.HasValue)
                schedules = schedules.Where(s => s.StartTime.TimeOfDay >= request.StartTime.Value.ToTimeSpan());

            if (request.EndTime.HasValue)
                schedules = schedules.Where(s => s.EndTime.TimeOfDay <= request.EndTime.Value.ToTimeSpan());

            var result = await schedules
                .OrderBy(x => x.LabRoomId)
                .ThenBy(x => x.StartTime)
                .ThenBy(x => x.EndTime)
                .ToListAsync(cancellationToken);

            var freeSlots = new List<ScheduleDto>();
            var currRoom = result[0].LabRoomId;

            for (int i = 0; i < result.Count - 1; i++)
            {
                if (result[i].LabRoomId != currRoom)
                {
                    currRoom = result[i].LabRoomId;
                    continue;
                }

                if (result[i + 1].StartTime < result[i].EndTime)
                    continue;

                if (result[i + 1].StartTime.TimeOfDay - result[i].EndTime.TimeOfDay >=
                    (request.Duration.HasValue ? request.Duration.Value.ToTimeSpan() : new TimeSpan(1, 0, 0)))
                {
                    freeSlots.Add(new ScheduleDto
                    {
                        LabRoomId = result[i].LabRoomId,
                        StartTime = result[i].EndTime,
                        EndTime = result[i + 1].StartTime
                    });
                }
            }

            return new ResultMessage<List<ScheduleDto>>
            {
                Success = true,
                Message = "Free slots retrieved successfully.",
                Data = freeSlots
            };
        }
    }
}
