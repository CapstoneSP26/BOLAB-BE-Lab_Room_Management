using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Interfaces.Jobs
{
    public interface ICancelBookingEmailJob 
    { 
        Task Execute(Guid targetId, bool isCancelledByAdmin, Guid actionByUserId); 
    }
}
