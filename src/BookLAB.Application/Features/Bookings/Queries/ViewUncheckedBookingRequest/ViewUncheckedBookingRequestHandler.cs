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
    public class ViewUncheckedBookingRequestHandler : IRequestHandler<ViewUncheckedBookingRequestCommand, List<BookingRequestDto>>
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
        public async Task<List<BookingRequestDto>> Handle(ViewUncheckedBookingRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Query the BookingRequest repository for requests that are either:
                // - Not yet responded (ResponsedByUserId == null)
                // - Responded by the current user (ResponsedByUserId == request.userId)
                var result = await _unitOfWork.Repository<BookingRequest>().Entities
                    .Where(x => x.ResponsedByUserId == null || x.ResponsedByUserId == request.userId)
                    // Use AutoMapper's ProjectTo to project entities directly into DTOs at the query level
                    .ProjectTo<BookingRequestDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken); // Execute query asynchronously with cancellation support

                // Return the mapped list of BookingRequestDto
                return result;
            }
            catch (Exception ex)
            {
                // Log the exception with details for debugging
                _logger.LogError(ex, "Something is wrong while querying unchecked booking request: " + ex.Message);

                // Return an empty list to avoid breaking the caller
                return new List<BookingRequestDto>();
            }
        }

    }
}
