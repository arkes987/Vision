using System.Runtime.InteropServices;

namespace VisionAPI.Extensions
{
    public static class DateTimeExtensions
    {
        public static System.DateTime ConvertFromUtc(this System.DateTime dateTime, TimeZoneInfo destinationTimeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, destinationTimeZone);
        }
        public static System.DateTime GetNow()
        {
            return System.DateTime.UtcNow.ConvertFromUtc(TimeZones.CentralEuropeanTimeZone);
        }
    }

    public static class TimeZones
    {
        public static TimeZoneInfo CentralEuropeanTimeZone => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? GetTimeZone("Central European Standard Time")
            : GetTimeZone("Europe/Warsaw");

        private static TimeZoneInfo GetTimeZone(string timeZoneId)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
    }
}
