using BookLAB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Models
{
    public class BookingRequestDto
    {
        public Guid BookingId { get; set; }
        public Guid RequestedByUserId { get; set; }
        public Guid? ResponsedByUserId { get; set; }
        public BookingRequestStatus BookingRequestStatus { get; set; }
        public string? ResponseContext { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
