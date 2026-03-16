namespace KhaledTeamRecycling.Middelware
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;

    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public MaintenanceMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var isMaintenance = _config.GetValue<bool>("MaintenanceMode");
            if (!isMaintenance)
            {
                await _next(context);
                return;
            }

            var path = context.Request.Path;

            // Allow the maintenance page itself + common exceptions
            if (path.StartsWithSegments("/maintenance") ||
                path.StartsWithSegments("/health") ||
                path.StartsWithSegments("/css") ||
                path.StartsWithSegments("/js") ||
                path.StartsWithSegments("/images") ||
                path.StartsWithSegments("/favicon.ico"))
            {
                await _next(context);
                return;
            }

            // API requests: return 503 JSON (better than redirect)
            if (path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.Response.Headers["Retry-After"] = "300";
                await context.Response.WriteAsJsonAsync(new { message = "Service is under maintenance" });
                return;
            }

            // UI requests: redirect to maintenance page
            context.Response.Redirect("/maintenance");
        }
    }

}
