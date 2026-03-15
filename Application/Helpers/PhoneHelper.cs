using Domain.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Application.Helpers
{
    public static class PhoneHelper
    {

        public static async Task<string?> CheckAndDoPhoneStart971(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(phone))
            {
                return null;
            }
            // //Phone Dubai
            var PhoneNumber = phone?.Replace(" ", "");
            if (!string.IsNullOrEmpty(PhoneNumber) && PhoneNumber.StartsWith("0")) { PhoneNumber = PhoneNumber.Substring(1); }
            if (PhoneNumber != null && !PhoneNumber.StartsWith("971"))
            {
                if (!string.IsNullOrEmpty(PhoneNumber) && PhoneNumber.StartsWith("9710")) { PhoneNumber = PhoneNumber.Substring(4); }
                PhoneNumber = "971" + PhoneNumber;
            }

            return PhoneNumber;
        }

    }

}
