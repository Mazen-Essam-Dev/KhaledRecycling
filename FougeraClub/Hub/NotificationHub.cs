using Domain.Enums;
using FougeraClub.Helpers;
using Microsoft.AspNetCore.SignalR;

namespace FougeraClub.Hub
{
    public class NotificationHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly PermissionScanner _permissionScanner;

        public NotificationHub(PermissionScanner permissionScanner)
        {
            _permissionScanner = permissionScanner;
        }

        public override async Task OnConnectedAsync()
        {
            var user = Context.User;

            // Add user to groups based on roles
            if (_permissionScanner.ValidatePermission("Course", "Index"))
                await Groups.AddToGroupAsync(Context.ConnectionId, "CourseManagers");
                Console.WriteLine("Added to CourseManagers group");

            if (_permissionScanner.ValidatePermission("Activity", "Index"))
                await Groups.AddToGroupAsync(Context.ConnectionId, "EventManagers");
                Console.WriteLine("Added To Events Manger Group");

            if (_permissionScanner.ValidatePermission("Activity", "Index"))
                await Groups.AddToGroupAsync(Context.ConnectionId, "EventManagers");
                Console.WriteLine("Added To Events Manger Group");

            var roleNumber = _permissionScanner.ValidateRoleNumber();

            if (roleNumber.ToString() == "Manager")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Manager");
                Console.WriteLine("Added To Manger Group");
            }
            if (roleNumber == RoleNumber.Accountant)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Accountant");
                Console.WriteLine("Added To Accountant Group");
            }
            if (roleNumber == RoleNumber.ActivitiesSupervisor)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "ActivitiesSupervisor");
                Console.WriteLine("Added To ActivitiesSupervisor Group");
            }
            await base.OnConnectedAsync();
        }
    }
}
