using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace FougeraClub.Extensions
{
    public static class IdentityExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();
        }
    }

}
