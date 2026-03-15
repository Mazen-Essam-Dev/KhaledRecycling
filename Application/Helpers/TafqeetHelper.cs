using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public static class TafqeetHelper
    {
        public static string Tafqeet(decimal amount, CurrencyInfoDTO mainCurrency, CurrencyInfoDTO subCurrency)
        {
            return mainCurrency.Language == "ar"
                ? TafqeetArabic(amount, mainCurrency, subCurrency)
                : TafqeetEnglish(amount, mainCurrency, subCurrency);
        }

        // ---------------------------- Arabic ----------------------------
        private static string TafqeetArabic(decimal amount, CurrencyInfoDTO mainCurrency, CurrencyInfoDTO subCurrency)
        {
            string result = "";

            if (amount < 0) // if Minus
            { amount = amount * -1; result = "سالب "; }

            long integerPart = (long)Math.Floor(amount);
            int fractionPart = (int)Math.Round((amount - integerPart) * 100);


            if (integerPart > 0)
                result += TafqeetWithCurrencyArabic(integerPart, mainCurrency);

            if (fractionPart > 0)
            {
                if (result != "")
                    result += " و ";
                result += TafqeetWithCurrencyArabic(fractionPart, subCurrency);
            }

            if (result == "")
                return $"صفر {mainCurrency.Singular} {mainCurrency.Suffix}";

            // Return the assembled words (main currency then sub currency if present)
            return result.Trim();
        }

        private static string TafqeetWithCurrencyArabic(long number, CurrencyInfoDTO currency)
        {
            string numberWords = TafqeetNumberArabic(number, currency.IsMasculine);

            if (number == 1)
                return $"{currency.Singular} واحد";
            else if (number == 2)
                return currency.Dual;
            else if (number >= 3 && number <= 10)
                return numberWords + " " + currency.Plural;
            else
                return numberWords + " " + currency.Singular;
        }

        private static string TafqeetNumberArabic(long number, bool masculine)
        {
            if (number == 0) return "صفر";
            if (number < 0) number = number * -1;
            if (number > 100_000_000_000) return "الرقم أكبر من 100 مليار";
            //throw new ArgumentOutOfRangeException(nameof(number), "الرقم أكبر من 100 مليار");

            string[] onesMasculine = { "", "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة", "ستة", "سبعة", "ثمانية", "تسعة",
                                       "عشرة", "أحد عشر", "اثنا عشر", "ثلاثة عشر", "أربعة عشر", "خمسة عشر",
                                       "ستة عشر", "سبعة عشر", "ثمانية عشر", "تسعة عشر" };

            string[] onesFeminine = { "", "واحدة", "اثنتان", "ثلاث", "أربع", "خمس", "ست", "سبع", "ثمان", "تسع",
                                      "عشر", "إحدى عشرة", "اثنتا عشرة", "ثلاث عشرة", "أربع عشرة", "خمس عشرة",
                                      "ست عشرة", "سبع عشرة", "ثماني عشرة", "تسع عشرة" };

            string[] tens = { "", "", "عشرون", "ثلاثون", "أربعون", "خمسون",
                              "ستون", "سبعون", "ثمانون", "تسعون" };

            string[] hundreds = { "", "مائة", "مئتان", "ثلاثمائة", "أربعمائة",
                                  "خمسمائة", "ستمائة", "سبعمائة", "ثمانمائة", "تسعمائة" };

            string[] scales = { "", "ألف", "مليون", "مليار" };

            string result = "";
            int scaleIndex = 0;

            while (number > 0)
            {
                int group = (int)(number % 1000);
                number /= 1000;

                if (group > 0)
                {
                    string groupWords = ConvertGroupArabic(group,
                                                           masculine ? onesMasculine : onesFeminine,
                                                           tens,
                                                           hundreds);

                    if (scaleIndex > 0)
                    {
                        if (group == 1)
                            groupWords = scales[scaleIndex];
                        else if (group == 2)
                            groupWords = scales[scaleIndex] + "ان";
                        else if (group >= 3 && group <= 10)
                            groupWords += " " + scales[scaleIndex] + "ات";
                        else
                            groupWords += " " + scales[scaleIndex];
                    }

                    if (result != "")
                        result = groupWords + " و " + result;
                    else
                        result = groupWords;
                }

                scaleIndex++;
            }

            return result.Trim();
        }

        private static string ConvertGroupArabic(int number, string[] ones, string[] tens, string[] hundreds)
        {
            string result = "";
            int h = number / 100;
            int t = number % 100;

            if (h > 0)
                result += hundreds[h];

            if (t > 0)
            {
                if (result != "")
                    result += " و ";

                if (t < 20)
                    result += ones[t];
                else
                {
                    int unit = t % 10;
                    int ten = t / 10;
                    if (unit > 0)
                        result += ones[unit] + " و " + tens[ten];
                    else
                        result += tens[ten];
                }
            }
            return result.Trim();
        }

        // ---------------------------- English ----------------------------
        private static string TafqeetEnglish(decimal amount, CurrencyInfoDTO mainCurrency, CurrencyInfoDTO subCurrency)
        {
            string result = "";

            if (amount < 0) // if Minus
            { amount = amount * -1; result = "Minus "; }

            long integerPart = (long)Math.Floor(amount);
            int fractionPart = (int)Math.Round((amount - integerPart) * 100);

            if (integerPart > 0)
                result += TafqeetWithCurrencyEnglish(integerPart, mainCurrency);

            if (fractionPart > 0)
            {
                if (result != "")
                    result += " and ";
                result += TafqeetWithCurrencyEnglish(fractionPart, subCurrency);
            }

            if (result == "")
                result = $"Zero {mainCurrency.Plural} {mainCurrency.Suffix}";

            return result.Trim();
        }

        private static string TafqeetWithCurrencyEnglish(long number, CurrencyInfoDTO currency)
        {
            string numberWords = TafqeetNumberEnglish(number);

            if (number == 1)
                return $"{numberWords} {currency.Singular}";
            else
                return $"{numberWords} {currency.Plural}";
        }

        private static string TafqeetNumberEnglish(long number)
        {
            if (number == 0) return "Zero";

            if (number < 0) number = number * -1;

            if (number > 100_000_000_000) return "Number is greater than 100 billion";
            //throw new ArgumentOutOfRangeException(nameof(number), "Number is greater than 100 billion");

            string[] ones = { "", "One", "Two", "Three", "Four", "Five", "Six",
                              "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve",
                              "Thirteen", "Fourteen", "Fifteen", "Sixteen",
                              "Seventeen", "Eighteen", "Nineteen" };

            string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty",
                              "Sixty", "Seventy", "Eighty", "Ninety" };

            string[] scales = { "", "Thousand", "Million", "Billion" };

            string result = "";
            int scaleIndex = 0;

            while (number > 0)
            {
                int group = (int)(number % 1000);
                number /= 1000;

                if (group > 0)
                {
                    string groupWords = ConvertGroupEnglish(group, ones, tens);

                    if (scaleIndex > 0)
                        groupWords += " " + scales[scaleIndex];

                    if (result != "")
                        result = groupWords + " " + result;
                    else
                        result = groupWords;
                }

                scaleIndex++;
            }

            return result.Trim();
        }

        private static string ConvertGroupEnglish(int number, string[] ones, string[] tens)
        {
            string result = "";

            int h = number / 100;
            int t = number % 100;

            if (h > 0)
                result += ones[h] + " Hundred";

            if (t > 0)
            {
                if (result != "")
                    result += " ";

                if (t < 20)
                    result += ones[t];
                else
                {
                    int unit = t % 10;
                    int ten = t / 10;
                    result += tens[ten];
                    if (unit > 0)
                        result += "-" + ones[unit];
                }
            }

            return result.Trim();
        }
    }

}