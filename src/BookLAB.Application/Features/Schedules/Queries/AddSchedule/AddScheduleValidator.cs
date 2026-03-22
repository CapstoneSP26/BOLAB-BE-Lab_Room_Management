using BookLAB.Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Queries.AddSchedule
{
    public class AddScheduleValidator : AbstractValidator<AddScheduleCommand>
    {
        public AddScheduleValidator()
        {
            RuleFor(x => x.Schedule).NotNull().WithMessage("Schedule cannot be null");
            RuleFor(x => x.Schedule.Id)
                .Must(id => id != Guid.Empty)
                .WithMessage("Id is required");
            RuleFor(x => x.Schedule.LecturerId)
                .Must(id => id != Guid.Empty)
                .WithMessage("LecturerId is required");
            RuleFor(x => x.Schedule.LabRoomId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Lab Room is required and greater than 0");
            RuleFor(x => x.Schedule.BookingId)
                .Must(id => id == null || id != Guid.Empty)
                .WithMessage("BookingId must be null or a valid Guid");
            RuleFor(x => x.Schedule.GroupId)
                .Must(id => id == null || id != Guid.Empty)
                .WithMessage("GroupId must be null or a valid Guid");
            RuleFor(x => x.Schedule.SlotTypeId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Slot Type is required and greater than 0");
            RuleFor(x => x.Schedule.ScheduleType)
                .Must(type => Enum.IsDefined(typeof(ScheduleType), type))
                .WithMessage("Invalid Schedule Type");
            RuleFor(x => x.Schedule.ScheduleStatus)
                .Must(type => Enum.IsDefined(typeof(ScheduleStatus), type))
                .WithMessage("Invalid Schedule Status");
            RuleFor(x => x.Schedule.StudentCount)
                .NotEmpty()
                .GreaterThanOrEqualTo(0)
                .WithMessage("Number of student is required and greater or equal than 0");
            RuleFor(x => x.Schedule.StartTime)
                .NotEmpty()
                .WithMessage("Start Time is required");
            RuleFor(x => x.Schedule.EndTime)
                .NotEmpty()
                .WithMessage("End Time is required");
            RuleFor(x => x.Schedule.CreatedAt)
                .NotEmpty()
                .WithMessage("End Time is required");
            RuleFor(x => x.Schedule.CreatedBy)
                .Must(id => id == null || id != Guid.Empty)
                .WithMessage("Created By must be null or a valid Guid");
            RuleFor(x => x.Schedule.UpdatedBy)
                .Must(id => id == null || id != Guid.Empty)
                .WithMessage("Updated By must be null or a valid Guid");
        }
    }
}
