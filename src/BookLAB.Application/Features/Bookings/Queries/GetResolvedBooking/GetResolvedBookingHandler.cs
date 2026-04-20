using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Bookings.Queries.ViewBookingHistory;
using BookLAB.Domain.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.GetResolvedBooking
{
    public class GetResolvedBookingHandler : IRequestHandler<GetResolvedBookingQuery, GetResolvedBookingReturn>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetResolvedBookingHandler> _logger;

        public GetResolvedBookingHandler(IUnitOfWork unitOfWork, ILogger<GetResolvedBookingHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<GetResolvedBookingReturn> Handle(GetResolvedBookingQuery request, CancellationToken cancellationToken)
        {
            var startUtc = request.startDate.ToUniversalTime();
            var endUtc = request.endDate.ToUniversalTime();

            var startBoundary = new DateTimeOffset(
                startUtc.Year,
                startUtc.Month,
                startUtc.Day,
                0,
                0,
                0,
                TimeSpan.Zero);

            var endBoundaryExclusive = new DateTimeOffset(
                endUtc.Year,
                endUtc.Month,
                endUtc.Day,
                0,
                0,
                0,
                TimeSpan.Zero).AddDays(1);

            try
            {
                var labRoomIds = await _unitOfWork.Repository<LabOwner>().Entities.Where(x => x.UserId == request.userId).Select(x => x.LabRoomId).ToListAsync(cancellationToken);

                var query = _unitOfWork.Repository<Booking>().Entities
                    .Include(x => x.LabRoom)
                    .Include(x => x.LabRoom.Building)
                    .Include(x => x.PurposeType)
                    .Where(b =>
                        b.StartTime >= startBoundary &&
                        b.StartTime < endBoundaryExclusive &&
                        labRoomIds.Contains(b.LabRoomId) // filter by labRoomIds
                    );

                if (request.requestStatus != null)
                    query = query.Where(x => x.BookingStatus == request.requestStatus);
                else
                    query = query.Where(x => x.BookingStatus == Domain.Enums.BookingStatus.Approved || x.BookingStatus == Domain.Enums.BookingStatus.Rejected);

                if (request.buildingId != null)
                    query = query.Where(x => x.LabRoom.BuildingId == request.buildingId);

                if (request.labRoomId != null)
                    query = query.Where(x => x.LabRoomId == request.labRoomId);

                if (request.keyword != null)
                    query = query.Where(x => x.LabRoom.RoomName.ToLower().Contains(request.keyword.ToLower()) || x.LabRoom.RoomNo.ToLower().Contains(request.keyword.ToLower()));

                if (!request.status.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(b => b.BookingStatus.ToString().ToLower() == request.status.ToLower());
                }

                var result = await query
                    .OrderByDescending(b => b.StartTime)
                    .ThenByDescending(b => b.CreatedAt)
                    .Skip((request.page - 1) * request.limit)
                    .Take(request.limit)
                    .ToListAsync();

                var username = string.Empty;
                List<ViewBookingHistoryResponseDTO> data = new List<ViewBookingHistoryResponseDTO>();

                for (int i = 0; i < result.Count; i++)
                {
                    // Fetch the user who created the booking. 
                    // ⚠️ This can cause N+1 query problem since it queries the database inside a loop.
                    username = (await _unitOfWork.Repository<User>().GetByIdAsync(result[i].CreatedBy)).FullName;

                    // Map entity fields into the response DTO.
                    data.Add(new ViewBookingHistoryResponseDTO
                    {
                        id = result[i].Id.ToString(),
                        roomId = result[i].LabRoomId.ToString(),
                        roomName = result[i].LabRoom.RoomName,
                        buildingName = result[i].LabRoom.Building.BuildingName,
                        startTime = result[i].StartTime.ToString("HH:mm"),
                        endTime = result[i].EndTime.ToString("HH:mm"),
                        date = result[i].StartTime.ToString("yyyy-MM-dd"),
                        status = result[i].BookingStatus.ToString(),
                        purpose = result[i].PurposeType.PurposeName,
                        reason = result[i].Reason,
                        userName = username
                    });
                }

                return new GetResolvedBookingReturn
                {
                    List = data,
                    total = query.Count()
                };
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching resolved bookings for user {UserId}", request.userId);
                return new GetResolvedBookingReturn();
            }
            
        }
    }
}
