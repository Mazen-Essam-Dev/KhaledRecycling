using Application.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Application.Interfaces.Admin;
using FougeraClub.Middelware;

namespace FougeraClub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;
        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [NoLogging]
        [HttpGet]
        public async Task<IActionResult> GetUnread()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var notifications = await _notificationService.GetNotificationsForUserAsync(userId);
            var unread = notifications.Where(n => !n.IsRead).ToList();
            // Mark all unread notifications as read
            foreach (var n in unread)
            {
                await _notificationService.MarkNotificationAsReadAsync(userId, n.NotificationId);
            }
            var unreadResult = unread
                .Select(n => new
                {
                    id = n.Id,
                    title = n.Notification.Title,
                    message = n.Notification.Message,
                    createdAt = n.Notification.CreatedAt.ToString("g")
                }).ToList();
            return Json(unreadResult);
        }
        [NoLogging]
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Json(new { count = 0 });

            var notifications = await _notificationService.GetNotificationsForUserAsync(userId);
            var count = notifications.Count(n => !n.IsRead);
            return Json(new { count });
        }
    }
}

