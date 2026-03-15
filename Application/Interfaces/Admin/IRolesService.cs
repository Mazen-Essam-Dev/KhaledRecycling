namespace Application.Interfaces.Admin
{
    public interface IRolesService
    {
        Task<List<string>> GetPermissionsByRoleAsync(string roleName);
        Task<bool> UpdateRolePermissionsAsync(string roleName, List<string> selectedPermissions, string? newRoleName);
        Task<bool> RemoveRoleAsync(string roleName);

    }
}
