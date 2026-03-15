using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Application.Helpers
{
    public static class EnumHelper
    {
        public static string GetDisplayName(Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            var displayAttribute = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false
            ) as DisplayAttribute[];

            if (displayAttribute != null && displayAttribute.Length > 0)
            {
                return displayAttribute[0].GetName();
            }

            return enumValue.ToString();
        }

        public static string GetDisplayKey(this Enum value)
        {
            return value.GetType()
                .GetMember(value.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()?
                .Name
                ?? value.ToString();
        }
    }

}
