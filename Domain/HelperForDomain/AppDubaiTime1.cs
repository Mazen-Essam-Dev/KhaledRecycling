using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.HelperForDomain
{
    public static class AppDubaiTime1
    {
        private static readonly TimeZoneInfo DubaiZone =
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Dubai");

        public static DateTime Now =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, DubaiZone);

        public static DateTimeOffset NowOffset =>
            TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, DubaiZone);

        public static DateTime ConvertToDubaiDateTime(DateTime dateTimeUtc)
        {
            //we make sure the date is specified as UTC
            var utc = DateTime.SpecifyKind(dateTimeUtc, DateTimeKind.Utc);

            return TimeZoneInfo.ConvertTimeFromUtc(utc, DubaiZone);
        }
        public static DateOnly ConvertToDubaiDateOnly(DateOnly dateOnlyUtc)
        {
            // We convert DateOnly to DateTime (on midnight) and specify that it is UTC
            var utc = DateTime.SpecifyKind(dateOnlyUtc.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

            // Convert to Dubai time
            var dubaiDateTime = TimeZoneInfo.ConvertTimeFromUtc(utc, DubaiZone);

            // We return DateOnly only
            return DateOnly.FromDateTime(dubaiDateTime);
        }
    }
}
