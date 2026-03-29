using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.ViewUncheckedBookingRequest
{
    public class ViewUncheckedBookingRequestHandler : IRequestHandler<ViewUncheckedBookingRequestCommand, List<Booking>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ViewUncheckedBookingRequestHandler> _logger;

        public ViewUncheckedBookingRequestHandler(IUnitOfWork unitOfWork, 
            ILogger<ViewUncheckedBookingRequestHandler> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the ViewUncheckedBookingRequestCommand to retrieve booking requests 
        /// that are either not yet responded or responded by the specified user.
        /// The method queries the repository, projects the entities directly into DTOs using AutoMapper, 
        /// and returns the list of BookingRequestDto objects.
        /// </summary>
        /// <param name="request">The command containing the userId used for filtering booking requests.</param>
        /// <param name="cancellationToken">
        /// Token provided by ASP.NET Core to cancel the request if the client disconnects or times out.
        /// </param>
        /// <returns>
        /// A list of BookingRequestDto objects representing the unchecked booking requests.
        /// Returns an empty list if an exception occurs.
        /// </returns>
        public async Task<List<Booking>> Handle(ViewUncheckedBookingRequestCommand request, CancellationToken cancellationToken)
        {
            try
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

                var labOwners = await _unitOfWork.Repository<LabOwner>().Entities
                    .Where(lo => lo.UserId == request.userId)
                    .Select(lo => lo.LabRoomId)
                    .ToListAsync(cancellationToken);

                var query = _unitOfWork.Repository<BookingRequest>().Entities
                    .Include(x => x.Booking)
                    .Include(x => x.Booking.LabRoom)
                    .Include(x => x.Booking.LabRoom.Building)
                    .Include(x => x.Booking.PurposeType)
                    .Where(b =>
                        labOwners.Contains(b.Booking.LabRoomId) &&
                        b.Booking.StartTime >= startBoundary &&
                        b.Booking.StartTime < endBoundaryExclusive &&
                        (request.labRoomId == null || b.Booking.LabRoomId == request.labRoomId) // thêm filter labRoomId
                    );

                if (!request.status.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(b => b.Booking.BookingStatus.ToString().ToLower() == request.status.ToLower());
                }

                var result = await query
                    .OrderByDescending(b => b.Booking.StartTime)
                    .ThenByDescending(b => b.CreatedAt)
                    .Skip((request.page - 1) * request.limit)
                    .Take(request.limit)
                    .ToListAsync();

                List<Booking> resultBookings = new List<Booking>();

                foreach (var br in result)
                {
                    resultBookings.Add(br.Booking);
                }

                return resultBookings;
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
