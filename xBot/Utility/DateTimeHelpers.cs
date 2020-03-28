using System;
namespace xBot.Utility
{
    /// <summary>
    /// Extensions helper for DateTime
    /// </summary>
    public static class DateTimeHelpers
    {
        /// <summary>
        /// Returns the datetime in the shortest format
        /// </summary>
        public static string ToShortFormat(this DateTime DateTime)
        {
            return DateTime.ToString("[hh:mm:ss]");
        }
    }
}
