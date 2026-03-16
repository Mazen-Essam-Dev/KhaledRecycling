using Application.Helpers;
using Application.Services.Admin;
using KhaledTeamRecycling.Helpers;
using KhaledTeamRecycling.Middelware;
using System.Globalization;

namespace KhaledTeamRecycling.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void ConfigureMiddleware(this WebApplication app)
        {
            // Ensure required services are resolved correctly
            var env = app.Services.GetRequiredService<IWebHostEnvironment>();
            var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
            // Static helper initializations
            FileHelper.Configure(env);
            ExcelStaticReport.ConfigureExcel(env);
            SessionHelper.Configure(httpContextAccessor);
            SelectListHelper.Configure();
            RadioButtonHelper.Configure();



            // ✅ 1. Use the general error page in the case of (Production Only)
            if (!app.Environment.IsDevelopment())
            {
                //Called if general errors as like as (500 and the like)
                app.UseExceptionHandler("/Admin/Home/Error");

                //Called if process Error status codes such as 404 and 403
                app.UseStatusCodePagesWithReExecute("/CustomError/Handle", "?statusCode={0}");
            }
            else
            {
                // In development mode, details appear (Not Production)
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession(); // Must be before localization
            app.Use(async (context, next) =>
            {
                var lang = context.Session.GetString("CurrentCulture");

                if (string.IsNullOrEmpty(lang))
                {
                    lang = "ar"; // default to Arabic
                    context.Session.SetString("CurrentCulture", lang);
                }

                var culture = new CultureInfo(lang);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                culture.NumberFormat.NumberDecimalSeparator = "."; //ar,En AsLike En
                culture.NumberFormat.CurrencyDecimalSeparator = ".";//ar,En AsLike En
                culture.DateTimeFormat.AMDesignator = "AM";//ar,En AsLike En
                culture.DateTimeFormat.PMDesignator = "PM";//ar,En AsLike En
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                await next();
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseMiddleware<NotificationMiddleware>();


            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapGet("/", context =>
            {
                context.Response.Redirect("/member/home/index");
                return Task.CompletedTask;
            });

            app.MapRazorPages();
        }
    }
}
