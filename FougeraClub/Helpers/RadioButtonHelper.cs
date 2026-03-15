using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FougeraClub.Helpers
{
    public static class RadioButtonHelper
    {

        public static void Configure()
        {
        }

        /// <summary>
        /// Returns the localized display name of a single enum value.
        /// </summary>
        public static string GetDisplayName<TEnum>(this TEnum value)
            where TEnum : Enum
        {
            var memberInfo = typeof(TEnum).GetMember(value.ToString()).FirstOrDefault();
            if (memberInfo == null)
                return value.ToString();

            var displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute?.GetName() ?? value.ToString();
        }

        /// <summary>
        /// Returns all values of the enum with their localized names.
        /// </summary>
        public static List<(int Id, string Name)> GetAllWithNames<TEnum>()
            where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => (Convert.ToInt32(e), e.GetDisplayName()))
                .ToList();
        }

        /// <summary>
        /// Returns a list of SelectListItem for radio button binding.
        /// </summary>
        public static List<SelectListItem> GetRadioList<TEnum>(int? selectedId = null)
            where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = Convert.ToInt32(e).ToString(),
                    Text = e.GetDisplayName(),
                    Selected = selectedId.HasValue && selectedId.Value == Convert.ToInt32(e)
                })
                .ToList();
        }
    }
}
