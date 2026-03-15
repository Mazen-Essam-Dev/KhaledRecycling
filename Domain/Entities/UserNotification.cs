using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserNotification
    {
        public int Id { get; set; }

        public int NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
    }

}
