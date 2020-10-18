using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Manifest.Services.Rds
{
    internal class DataParser
    {
        internal static bool ToBool(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            try
            {
                return Boolean.Parse(s);
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static TimeSpan ToTimeSpan(string timeString)
        {
            return ToTimeSpan(timeString, new TimeSpan());
        }

        internal static TimeSpan ToTimeSpan(string timeString, TimeSpan defaultTimeSpan )
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(timeString)) return TimeSpan.Parse(timeString);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return new TimeSpan();
        }

        internal static DateTime ToDateTime(string dateTimeString, DateTime defaultDateTime)
        {
            try
            {
                return DateTime.Parse(dateTimeString);
            }catch(Exception e)
            {
                Debug.WriteLine(e);
            }
            return defaultDateTime;
        }

        internal static DateTime ToDateTime(string dateTimeString)
        {
            return ToDateTime(dateTimeString, new DateTime());
        }

        internal static string BoolToString(bool b)
        {
            return (b) ? "true" : "false";
        }
    }
}
