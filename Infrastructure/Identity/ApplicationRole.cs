using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationRole : IdentityRole
{
    public int RoleNumber { get; set; }
}


