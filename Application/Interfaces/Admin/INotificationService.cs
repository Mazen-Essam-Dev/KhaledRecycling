using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Admin
{
    public interface INotificationService
    {
        Task SendNotificationToUsersAsync(string title, string message, List<string> userIds);
        Task<IEnumerable<UserNotification>> GetNotificationsForUserAsync(string userId);
        Task MarkNotificationAsReadAsync(string userId, int notificationId);

        Task SendNotificationToPermissionAsync(string title, string message, string permission);

        Task SendNotificationToRoleAsync(string title, string message, int roleNumber);
    }

}
