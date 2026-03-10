using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.ViewUncheckedBookingRequest
{
    public class ViewUncheckedBookingRequestHandler : IRequestHandler<ViewUncheckedBookingRequestCommand, List<BookingRequest>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ViewUncheckedBookingRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BookingRequest>> Handle(ViewUncheckedBookingRequestCommand request, CancellationToken cancellationToken)
        {
            Guid.TryParse(request.userId, out var userId);

           return await _unitOfWork.Repository<BookingRequest>().Entities.Where(x => x.ResponsedByUserId == null || x.ResponsedByUserId == userId).ToListAsync();
        }
    }
}
