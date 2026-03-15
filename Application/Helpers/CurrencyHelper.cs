using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public static class CurrencyHelper
    {
        // ====== Arabic (العربية) ======

        public static readonly CurrencyInfoDTO AED_Main_Ar = new CurrencyInfoDTO
        {
            Singular = "درهم",
            Dual = "درهمان",
            Plural = "دراهم",
            IsMasculine = true,
            Suffix = "إماراتي",
            Language = "ar"
        };

        public static readonly CurrencyInfoDTO AED_Sub_Ar = new CurrencyInfoDTO
        {
            Singular = "فلس",
            Dual = "فلسان",
            Plural = "فلوس",
            IsMasculine = true,
            Suffix = "إماراتي",
            Language = "ar"
        };

        // ====== English ======

        public static readonly CurrencyInfoDTO AED_Main_En = new CurrencyInfoDTO
        {
            Singular = "Dirham",
            Dual = "", // Dual not used in English
            Plural = "Dirhams",
            IsMasculine = true,
            Suffix = "UAE",
            Language = "en"
        };

        public static readonly CurrencyInfoDTO AED_Sub_En = new CurrencyInfoDTO
        {
            Singular = "Fils",
            Dual = "", // Dual not used
            Plural = "Fils", // Same plural form in English
            IsMasculine = true,
            Suffix = "UAE",
            Language = "en"
        };
    }
}
