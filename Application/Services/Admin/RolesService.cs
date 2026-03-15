using Application.Interfaces.Admin;
using Infrastructure.Identity;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Application.Services.Admin
{
    public class RolesService : IRolesService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public RolesService(RoleManager<ApplicationRole> roleManager, IUnitOfWork unitOfWork)
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<string>> GetPermissionsByRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            var claims = await _roleManager.GetClaimsAsync(role);
            return claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();
        }

        public async Task<bool> UpdateRolePermissionsAsync(string roleName, List<string> selectedPermissions,string? newRoleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role==null)
            {
                return false;
            }

            var existingClaims = await _roleManager.GetClaimsAsync(role);

            // Update role name
            if (!string.IsNullOrEmpty(newRoleName) && role.Name != newRoleName)
            {
                var ifFoundExist = await _roleManager.FindByNameAsync(newRoleName);
                if (ifFoundExist == null)
                {
                    role.Name = newRoleName.Trim();

                    var updateResult = await _roleManager.UpdateAsync(role);
                    if (!updateResult.Succeeded)
                    {
                        // You can return errors if you like
                        return false;
                    }
                }
            }

            // Remove old permissions
            foreach (var claim in existingClaims.Where(c => c.Type == "Permission"))
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            // Add selected ones
            foreach (var permission in selectedPermissions)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }

            return true;
        }
        public async Task<bool> RemoveRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
                return true;
            }
            return false;
        }

    }


}
