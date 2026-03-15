using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Notification
    {
     
            public int Id { get; set; }

            public string Title { get; set; } = null!;
            public string Message { get; set; } = null!;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
        

    }
}
