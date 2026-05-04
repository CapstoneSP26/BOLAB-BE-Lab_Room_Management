using BookLAB.Application.Common.Models;
using BookLAB.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Commands.CreateSchedule
{
    public class CreateScheduleCommand : IRequest<ResultMessage<ScheduleDto2>>
    {
        public Guid LecturerId { get; set; }
        public int LabRoomId { get; set; }
        public Guid? GroupId { get; set; }
        public string? GroupName { get; set; }
        public int? SlotTypeId { get; set; }
        public ScheduleType ScheduleType { get; set; }
        public int StudentCount { get; set; } = 0;
        public string? SubjectCode { get; set; }
        public string? ImportHash { get; set; } // For tracking imported schedules
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
