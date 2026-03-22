using AutoMapper;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;

namespace BookLAB.Application.Features.Bookings.Queries.ViewBookingHistory
{
    public class ViewBookingHistoryHandler : IRequestHandler<ViewBookingHistoryCommand, List<Booking>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ViewBookingHistoryHandler> _logger;

        public ViewBookingHistoryHandler(IUnitOfWork unitOfWork, ILogger<ViewBookingHistoryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Handles the ViewBookingHistoryCommand by retrieving a paginated list of bookings
        /// for the specified user within the given date range and status filter.
        /// </summary>
        /// <param name="request">
        /// The command containing userId, page, limit, status, startDate, and endDate filters.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to observe while waiting for the task to complete, useful for cancellation.
        /// </param>
        /// <returns>
        /// Returns a list of Booking entities that match the filter criteria.
        /// If an exception occurs, returns an empty list.
        /// </returns>
        public async Task<List<Booking>> Handle(ViewBookingHistoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Query the Booking repository with eager loading of related entities:
                // LabRoom, Building, and PurposeType.

                var query = _unitOfWork.Repository<Booking>().Entities
                    .Include(x => x.LabRoom)
                    .Include(x => x.LabRoom.Building)
                    .Include(x => x.PurposeType)
                    .Where(b =>
                        b.CreatedBy == request.userId &&
                        b.StartTime >= request.startDate &&
                        b.EndTime <= request.endDate &&
                        (request.labRoomId == null || b.LabRoomId == request.labRoomId) // thêm filter labRoomId
                    );

                if (!request.status.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(b => b.BookingStatus.ToString().ToLower() == request.status.ToLower());
                }

                var result = await query
                    .Skip((request.page - 1) * request.limit)
                    .Take(request.limit)
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                // In case of any exception, return an empty list.
                _logger.LogError(ex, "Error occurred while handling ViewBookingHistoryHandler");
                return new List<Booking>();
            }
        }

    }
}
