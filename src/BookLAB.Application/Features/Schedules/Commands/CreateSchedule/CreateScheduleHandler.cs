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
using System.Text.RegularExpressions;
using static QRCoder.PayloadGenerator;

namespace BookLAB.Application.Features.Schedules.Commands.CreateSchedule
{
    public class CreateScheduleHandler : IRequestHandler<CreateScheduleCommand, ResultMessage<ScheduleDto2>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateScheduleHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ResultMessage<ScheduleDto2>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
        {
            var scheduleId = Guid.NewGuid();

            Schedule schedule = new Schedule
            {
                Id = scheduleId,
                LecturerId = request.LecturerId,
                LabRoomId = request.LabRoomId,
                GroupId = request.GroupId,
                SlotTypeId = request.SlotTypeId,
                ScheduleType = request.ScheduleType,
                ScheduleStatus = ScheduleStatus.Active,
                StudentCount = request.StudentCount,
                SubjectCode = request.SubjectCode,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = _currentUserService.UserId,
                IsActive = true,
                IsDeleted = false,
            };

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Repository<Schedule>().AddAsync(schedule);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                var createdSchedule = await _unitOfWork.Repository<Schedule>().GetByIdAsync(scheduleId);

                return new ResultMessage<ScheduleDto2>
                {
                    Success = true,
                    Message = "Create Schedule successfully",
                    Data = _mapper.Map<Schedule, ScheduleDto2>(createdSchedule)
                };
            } catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ResultMessage<ScheduleDto2>
                {
                    Success = false,
                    Message = "Create Schedule fail"
                };
            }
        }
    }
}
