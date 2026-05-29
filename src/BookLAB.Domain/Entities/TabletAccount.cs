using BookLAB.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Domain.Entities
{
    public class TabletAccount : BaseEntity
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int RoomId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public LabRoom LabRoom { get; set; }
    }
}
