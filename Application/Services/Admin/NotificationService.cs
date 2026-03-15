using Application.Interfaces.Admin;
using Domain.Entities;
using Infrastructure.Identity;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Admin
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public NotificationService(IUnitOfWork unitOfWork , UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;

        }

        public async Task SendNotificationToRoleAsync(string title, string message, int roleNumber)
        {
            // Step 1: Find the role with the matching RoleNumber
            var role = await _roleManager.Roles
                .Where(r => r.RoleNumber == roleNumber)
                .FirstOrDefaultAsync();

            if (role == null)
                throw new Exception($"No role found with RoleNumber = {roleNumber}");

            // Step 2: Get all users in that role
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

            if (!usersInRole.Any())
                return;

            // Step 3: Create notification
            var notification = new Notification
            {
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);

            // Step 4: Assign notification to each user
            foreach (var user in usersInRole)
            {
                await _unitOfWork.UserNotifications.AddAsync(new UserNotification
                {
                    UserId = user.Id,
                    Notification = notification,
                    IsRead = false,
                });
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task SendNotificationToPermissionAsync(string title, string message, string permission)
        {
            // 1) Get roles that contain this permission
            var rolesList = await _roleManager.Roles.ToListAsync();
            var rolesWithClaims = new List<(ApplicationRole role, IList<Claim> claims)>();

            foreach (var role in rolesList)
            {
                var claims = await _roleManager.GetClaimsAsync(role);
                rolesWithClaims.Add((role, claims));
            }

            var matchedRoles = rolesWithClaims
                .Where(r => r.claims.Any(c => c.Type == "Permission" && c.Value == permission))
                .Select(r => r.role)
                .ToList();

            if (!matchedRoles.Any())
                return;

            // 2) Get the users inside these roles
            var userIds = new List<string>();

            foreach (var role in matchedRoles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                userIds.AddRange(usersInRole.Select(u => u.Id));
            }

            userIds = userIds.Distinct().ToList();
            if (!userIds.Any())
                return;

            // 3) Save main notification
            var notification = new Notification
            {
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);

            // 4) Save user notifications
            foreach (var userId in userIds)
            {
                await _unitOfWork.UserNotifications.AddAsync(new UserNotification
                {
                    UserId = userId,
                    Notification = notification,
                    IsRead = false
                });
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task SendNotificationToUsersAsync(string title, string message, List<string> userIds)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);

            foreach (var userId in userIds)
            {
                await _unitOfWork.UserNotifications.AddAsync(new UserNotification
                {
                    UserId = userId,
                    Notification = notification,
                    IsRead = false,
                });
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<UserNotification>> GetNotificationsForUserAsync(string userId)
        {
            return await _unitOfWork.UserNotifications
                .GetAsync(n => n.UserId == userId, n => n.Notification)
                .OrderByDescending(n => n.Notification.CreatedAt)
                .ToListAsync();
        }


        public async Task MarkNotificationAsReadAsync(string userId, int notificationId)
        {
            var entity = await _unitOfWork.UserNotifications
                .GetByColumnAsync(n => n.UserId == userId && n.NotificationId == notificationId);

            if (entity != null)
            {
                entity.IsRead = true;
                await _unitOfWork.CompleteAsync();
            }
        }
    }

}
