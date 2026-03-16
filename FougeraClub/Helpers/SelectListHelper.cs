using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace KhaledTeamRecycling.Helpers
{
    public static class SelectListHelper
    {
        public static void Configure()
        {
        }
        public static IEnumerable<SelectListItem> GetEnumSelectList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new SelectListItem
                {
                    Value = Convert.ToInt32(e).ToString(),
                    Text = GetDisplayName(e)
                });
        }
        public static IEnumerable<SelectListItem> GetEnumSelectList<T>(int? selectedId) where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new SelectListItem
                {
                    Value = Convert.ToInt32(e).ToString(),
                    Text = GetDisplayName(e),
                    Selected = selectedId == Convert.ToInt32(e)
                });
        }

        private static string GetDisplayName<T>(T enumValue) where T : Enum
        {
            var memberInfo = typeof(T).GetMember(enumValue.ToString()).FirstOrDefault();
            if (memberInfo != null)
            {
                var displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute != null)
                {
                    return displayAttribute.GetName() ?? enumValue.ToString();
                }
            }

            return enumValue.ToString(); // fallback to enum name if no display attribute
        }
        public static IEnumerable<SelectListItem> BindSelectList<T>(
       List<T> list,int? selected = null,string valueProperty = "Id",string nameAr = "NameAr",string nameEn = "NameEn")
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var type = typeof(T);

            var valueProp = type.GetProperty(valueProperty);
            var textProp = type.GetProperty(lang == "ar" ? nameAr : nameEn);

            if (valueProp == null || textProp == null)
                throw new Exception("Invalid property names provided.");

            string selectedValueStr = selected?.ToString();

            return list.Select(item => new SelectListItem
            {
                Value = valueProp.GetValue(item)?.ToString(),
                Text = textProp.GetValue(item)?.ToString(),
                Selected = selectedValueStr != null &&
                           valueProp.GetValue(item)?.ToString() == selectedValueStr
            });
        }

        public static IEnumerable<SelectListItem> BindSelectListIdString<T>(List<T> list,string? selected = null,string valueProperty = "Id",string nameAr = "FullNameAr",string nameEn = "FullNameEn")
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var type = typeof(T);

            var valueProp = type.GetProperty(valueProperty);
            var textProp = type.GetProperty(lang == "ar" ? nameAr : nameEn);

            if (valueProp == null || textProp == null)
                throw new Exception("Invalid property names provided.");

            return list.Select(item => new SelectListItem
            {
                Value = valueProp.GetValue(item)?.ToString(),
                Text = textProp.GetValue(item)?.ToString(),
                Selected = valueProp.GetValue(item)?.ToString() == selected
            });
        }
        public static IEnumerable<SelectListItem> BindSelectListWithDataFromUsers<TItem, TUser>(
     List<TItem> items,
     List<TUser> users,
     int? selected = null,
     string itemValueProperty = "Id",
     string userKeyProperty = "Id",
     string userMatchProperty = "UserId",
     string userNameAr = "FullNameAr",
     string userNameEn = "FullNameEn")
        {
            var lang = SessionHelper.GetCurrentLanguage(); // e.g. "ar" or "en"

            var itemType = typeof(TItem);
            var userType = typeof(TUser);

            var itemValueProp = itemType.GetProperty(itemValueProperty);
            var itemUserIdProp = itemType.GetProperty(userMatchProperty);
            var userKeyProp = userType.GetProperty(userKeyProperty);
            var userNameProp = userType.GetProperty(lang == "ar" ? userNameAr : userNameEn);

            if (itemValueProp == null || itemUserIdProp == null || userKeyProp == null || userNameProp == null)
                throw new Exception("Invalid property names provided.");

            return items.Select(item =>
            {
                var userId = itemUserIdProp.GetValue(item)?.ToString();
                var user = users.FirstOrDefault(u => userKeyProp.GetValue(u)?.ToString() == userId);
                var text = user != null ? userNameProp.GetValue(user)?.ToString() : "Unknown";

                return new SelectListItem
                {
                    Value = itemValueProp.GetValue(item)?.ToString(),
                    Text = text,
                    Selected = Convert.ToInt32(itemValueProp.GetValue(item)) == selected
                };
            });
        }


    }

}
