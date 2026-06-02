using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Interfaces.Jobs
{
    public interface IRecoverOanBookingEmailJob 
    { 
        Task Execute(int labRoomId, List<Guid> autoCancelledScheduleIds, List<Guid> autoRejectedBookingIds); 
    }
}
