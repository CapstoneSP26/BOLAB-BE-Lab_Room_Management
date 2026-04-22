using AutoMapper;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Commands.UpdateSchedule
{
    public class UpdateScheduleHandler : IRequestHandler<UpdateScheduleCommand, ResultMessage<ScheduleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateScheduleHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ResultMessage<ScheduleDto>> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;
                var schedule = _unitOfWork.Repository<Schedule>().GetById(request.Id);

                schedule.LecturerId = request.LecturerId;
                schedule.LabRoomId = request.LabRoomId;
                schedule.GroupId = request.GroupId != null ? request.GroupId : schedule.GroupId;
                var flag1 = Enum.TryParse<ScheduleType>(request.type, out var type);
                schedule.ScheduleType = flag1 ? type : schedule.ScheduleType;
                var flag2 = Enum.TryParse<ScheduleStatus>(request.status, out var status);
                schedule.ScheduleStatus = flag2 ? status : schedule.ScheduleStatus;
                schedule.SubjectCode = request.SubjectCode != null ? request.SubjectCode : schedule.SubjectCode;
                schedule.StartTime = request.StartTime;
                schedule.EndTime = request.EndTime;

                schedule.UpdatedBy = userId;
                schedule.UpdatedAt = DateTimeOffset.UtcNow;

                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Repository<Schedule>().UpdateAsync(schedule);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                var mappedSchedule = _mapper.Map<Schedule, ScheduleDto>(schedule);

                return new ResultMessage<ScheduleDto>
                {
                    Success = true,
                    Message = "Update schedule successfully",
                    Data = mappedSchedule,
                };
            } catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ResultMessage<ScheduleDto>
                {
                    Success = false,
                    Message = "Update schedule failed"
                };
            }
        }
    }
}
