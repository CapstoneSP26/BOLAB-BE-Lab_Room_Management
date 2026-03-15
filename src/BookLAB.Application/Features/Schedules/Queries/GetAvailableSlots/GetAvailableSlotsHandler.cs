using AutoMapper;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Queries.GetAvailableSlots
{
    public class GetAvailableSlotsHandler : IRequestHandler<GetAvailableSlotsCommand, List<AvailableScheduleResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAvailableSlotsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<AvailableScheduleResponse>> Handle(GetAvailableSlotsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var startDateRequest = DateTimeOffset.ParseExact(
                    request.query.startDate,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture
                ).ToOffset(TimeSpan.Zero);

                var endDateRequest = DateTimeOffset.ParseExact(
                    request.query.endDate,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture
                ).ToOffset(TimeSpan.Zero);

                var result = await _unitOfWork.Repository<Schedule>().Entities.Include(s => s.User).Include(s => s.Group)
                    .Where(s => s.LabRoomId.ToString().Equals(request.query.roomId) &&
                        (s.StartTime > startDateRequest && s.StartTime < endDateRequest) ||
                        (s.EndTime > startDateRequest && s.EndTime < endDateRequest)).ToListAsync();

                var resultMapper = _mapper.Map<List<Schedule>, List<AvailableScheduleResponse>>(result);

                return resultMapper;
            } catch (Exception ex)
            {
                return new List<AvailableScheduleResponse>();
            }
            
        }
    }
}
