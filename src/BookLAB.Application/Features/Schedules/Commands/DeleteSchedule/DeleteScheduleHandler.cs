using AutoMapper;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Commands.DeleteSchedule
{
    public class DeleteScheduleHandler : IRequestHandler<DeleteScheduleCommand, ResultMessage<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public DeleteScheduleHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ResultMessage<bool>> Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = _unitOfWork.Repository<Schedule>().GetById(request.Id);

            if (schedule == null)
                return new ResultMessage<bool>
                {
                    Success = false,
                    Message = "Schedule is not exist"
                };

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                _unitOfWork.Repository<Schedule>().Delete(schedule);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return new ResultMessage<bool>
                {
                    Success = true,
                    Message = "Delete schedule successfully"
                };
            } catch (Exception ex)
            {
                return new ResultMessage<bool>
                {
                    Success = false,
                    Message = "Delete schedule failed"
                };
            }
            
        }
    }
}
