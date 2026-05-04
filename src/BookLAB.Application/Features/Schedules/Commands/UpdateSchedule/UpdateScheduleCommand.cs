using BookLAB.Application.Common.Models;
using BookLAB.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Commands.UpdateSchedule
{
    public class UpdateScheduleCommand : IRequest<ResultMessage<ScheduleDto>>
    {
        public Guid Id { get; set; }
        public Guid LecturerId { get; set; }
        public int LabRoomId { get; set; }
        public Guid? GroupId { get; set; }
        public string? GroupName { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string? SubjectCode { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
