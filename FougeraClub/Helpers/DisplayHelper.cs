namespace KhaledTeamRecycling.Helpers
{
    public static class DisplayHelper
    {
        public static string FormatMainSubName(string? mainNameAr, string? mainNameEn, string? subNameAr, string? subNameEn)
        {
            var isAr = SessionHelper.GetCurrentLanguage() == "ar";
            var mainName = isAr ? mainNameAr ?? mainNameEn : mainNameEn ?? mainNameAr;
            var subName = isAr ? subNameAr ?? subNameEn : subNameEn ?? subNameAr;

            if (string.IsNullOrWhiteSpace(mainName))
            {
                return subName ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(subName))
            {
                return mainName;
            }

            return $"{mainName} - {subName}";
        }
    }
}
