namespace VisionAPI.Extensions
{
    public static class PathExtensions
    {
        public static string GetOutputPath(this string channel)
        {
            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");
            return Path.Combine(fullPath, channel);
        }
        public static string AppendDay(this string path)
        {
            var currentDay = DateTimeExtensions.GetNow().ToString(@"yyyy_MM_d");
            return Path.Combine(path, currentDay);
        }

        public static string AppendHour(this string path)
        {
            var currentHour = DateTimeExtensions.GetNow().ToString(@"HH");
            return Path.Combine(path, currentHour);
        }

        public static string AppendTimestamp(this string path)
        {
            var time = DateTimeExtensions.GetNow().ToString(@"HH.mm.ss");

            return Path.Combine(path, $"{time}");
        }
    }
}
