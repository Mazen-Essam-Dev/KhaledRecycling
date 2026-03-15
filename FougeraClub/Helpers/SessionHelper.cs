namespace FougeraClub.Helpers
{
    public static class SessionHelper
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string GetCurrentLanguage()
        {
            var lang = _httpContextAccessor?.HttpContext?.Session?.GetString("CurrentCulture");
            return lang ?? "ar";
        }

    }
}
