using AutoMapper;
using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Common.Policies;
using BookLAB.Application.Features.Bookings.Commands.CreateBooking;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Schedules.Commands.CreateSchedule
{
    public class CreateScheduleHandler : IRequestHandler<CreateScheduleCommand, ResultMessage<ScheduleDto2>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPolicyEvaluator _policyEvaluator;
        private readonly IMapper _mapper;

        public CreateScheduleHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IPolicyEvaluator policyEvaluator,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _policyEvaluator = policyEvaluator;
            _mapper = mapper;
        }

        public async Task<ResultMessage<ScheduleDto2>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
        {
            var scheduleId = Guid.NewGuid();
            var room = await _unitOfWork.Repository<LabRoom>().Entities
                .Include(r => r.RoomPolicies)
                .FirstOrDefaultAsync(r => r.Id == request.LabRoomId, cancellationToken);

            if (room == null || !room.IsActive)
                throw new NotFoundException("Phòng không tồn tại hoặc không hoạt động.");

                var weekStart = request.StartTime.ToUniversalTime();
                var weekEnd = request.EndTime.ToUniversalTime();

                // Kiểm tra lịch cá nhân của User (Schedules)
                var hasScheduleConflict = await _unitOfWork.Repository<Schedule>().Entities
                    .AnyAsync(s => s.LecturerId == request.LecturerId && s.IsActive && !s.IsDeleted &&
                                   s.StartTime < weekEnd && s.EndTime > weekStart, cancellationToken);

                if (hasScheduleConflict)
                    throw new BusinessException($"Trùng lịch vào ({weekStart:dd/MM/yyyy}). Vui lòng kiểm tra lại.");
            

            // 3. POLICY EVALUATION (Chỉ cần đánh giá cho tuần đầu tiên/request gốc)
            var activePolicies = room.RoomPolicies.Where(p => p.IsActive).ToList();
            CreateBookingCommand createBookingCommand = new CreateBookingCommand
            {
                LabRoomId = request.LabRoomId,
                SlotTypeId = request.SlotTypeId,
                PurposeTypeId = 0, // Không cần thiết cho Schedule, có thể bỏ qua hoặc set mặc định
                StartTime = request.StartTime.ToOffset(TimeSpan.FromHours(7)).DateTime,
                EndTime = request.EndTime.ToOffset(TimeSpan.FromHours(7)).DateTime,
                StudentCount = request.StudentCount,
                RecurringCount = 0, // Không cần thiết cho Schedule
                Reason = "Schedule Creation", // Không cần thiết cho Schedule
            };
            await _policyEvaluator.EvaluateAsync(createBookingCommand, activePolicies);

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
