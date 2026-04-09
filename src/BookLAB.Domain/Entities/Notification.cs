using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }                          
        public Guid? UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ReadAt { get; set; }
        public string Metadata { get; set; }
        public bool IsGlobal { get; set; } = false;
        public User? User { get; set; }
    }

}
