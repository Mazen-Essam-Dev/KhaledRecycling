using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Text;


namespace KhaledTeamRecycling.Extensions
{
    public static class LocalizationExtensions
    {
        public static IApplicationBuilder UseAppLocalization(this IApplicationBuilder app)
        {
            // Define supported cultures
            // GB used in en to make yyyy-MM-dd insted of yyyy-dd-MM
            var supportedCultures = new[]
            {
                new CultureInfo("en-GB"),
                new CultureInfo("ar")
            };

            // --------------------------------------------------
            // Override default short date formats
            // --------------------------------------------------
            // Arabic: 29-6-2030 (avoid '/' and RTL invisible marks)
            supportedCultures.First(c => c.Name == "ar")
                .DateTimeFormat.ShortDatePattern = "dd-M-yyyy";

            // English: 2030-6-29 (ISO-like format)
            supportedCultures.First(c => c.Name == "en-GB")
                .DateTimeFormat.ShortDatePattern = "yyyy-M-dd";

            // --------------------------------------------------
            // Configure localization options
            // --------------------------------------------------
            var options = new RequestLocalizationOptions
            {
                // Default culture if none is provided
                DefaultRequestCulture = new RequestCulture("en-GB"),

                // Cultures used for formatting dates, numbers, etc.
                SupportedCultures = supportedCultures,

                // Cultures used for UI strings (resources)
                SupportedUICultures = supportedCultures,

                // Custom provider: culture is stored in Session
                RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new CustomRequestCultureProvider(context =>
                    {
                        // Read culture from session
                        var lang = context.Session.GetString("CurrentCulture") ?? "en-GB";

                        // Apply culture to both Culture and UICulture
                        return Task.FromResult(new ProviderCultureResult(lang, lang));
                    })

                    //new CustomRequestCultureProvider(async context =>
                    //        {
                    //            var lang = context.Session.GetString("CurrentCulture") ?? "en";
                    //            return new ProviderCultureResult(lang, lang);
                    //        })
                }
            };

            // Apply localization middleware
            app.UseRequestLocalization(options);

            return app;
        }
    }
}
