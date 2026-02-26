using BookLAB.Application.Common.Interfaces.Persistence;
using BookLAB.Domain.DTOs;
using MediatR;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.GetBookingStats
{
    public class GetBookingStatsHandler : IRequestHandler<GetBookingStatsCommand, GetBookingStatsResponseDTO>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBookingStatsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetBookingStatsResponseDTO> Handle(GetBookingStatsCommand request, CancellationToken cancellationToken)
        {
            _unitOfWork.
        }
    }
}
