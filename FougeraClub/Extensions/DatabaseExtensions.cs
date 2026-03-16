using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KhaledTeamRecycling.Extensions
{
    public static class DatabaseExtensions
    {
        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure()

            ));

        }
    }
}
