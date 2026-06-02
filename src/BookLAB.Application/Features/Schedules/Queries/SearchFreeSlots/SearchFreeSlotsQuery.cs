using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Queries.SearchFreeSlots
{
    public class SearchFreeSlotsQuery : IRequest<ResultMessage<List<FreeSlotDto>>>
    {
        public int? BuildingId { get; set; }
        public int? LabRoomId { get; set; }
        public DateOnly StartDay { get; set; }
        public DateOnly EndDay { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public TimeSpan? Duration { get; set; }
    }
}
