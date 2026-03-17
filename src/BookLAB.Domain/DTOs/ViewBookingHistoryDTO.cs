using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Domain.DTOs
{
    /// <summary>
    /// Only need userId
    /// </summary>
    public class ViewBookingHistoryDTO
    {
        public Guid userId { get; set; }
    }
}
