using System;
using StudioMobile;
using System.Globalization;

#if __ANDROID__
using Android.Text.Format;
using Android.Content;
#endif

#if __IOS__
using Foundation;
#endif

namespace LiveOakApp.Models.Services
{
    public class DateTimeService
    {
        const string SERVER_DATE_FORMAT = @"yyyy-MM-dd";
        const string SERVER_TIME_FORMAT = @"yyyy-MM-dd'T'HH:mm:ss.fff'Z'";
        const string SERVER_DATETIME_FORMAT = @"yyyy-MM-dd'T'HH:mm:ss.fff'Z'";

        const string DISPLAY_TIME_24H_FORMAT = "HH:mm";
        const string DISPLAY_TIME_12H_FORMAT = "hh:mm tt";
        const string DISPLAY_TIME_12H_SHORT_FORMAT = "hh:mm t";
        const string DISPLAY_DATE_FORMAT = "MMM d";
        const string DISPLAY_DATETIME_24H_FORMAT = "yyyy-MM-dd HH:mm";
        const string DISPLAY_DATETIME_12H_FORMAT = "yyyy-MM-dd hh:mm tt";
        const string DISPLAY_DATETIME_12H_SHORT_FORMAT = "yyyy-MM-dd hh:mm t";
        const string DISPLAY_SHORT_YEAR = "yy";

        #region Server datetime format methods

        private DateTime? ConvertServerStringToDateTime(string s, string format, bool fromUTC)
        {
            if (string.IsNullOrEmpty(s)) return null;
            var dt = DateTime.Parse(s);
            //var dt = DateTime.Parse(s, null, DateTimeStyles.RoundtripKind);
            //var dt = DateTime.ParseExact(s, format, CultureInfo.InvariantCulture);
            if (fromUTC)
            {
                dt = dt.ToLocalTime();
            }
            else
            {
                DateTime.SpecifyKind(dt, DateTimeKind.Local);
            }
            return dt;
        }

        public DateTime? ServerStringToDateTime(string s)
        {
            return ConvertServerStringToDateTime(s, SERVER_DATETIME_FORMAT, true);
        }

        public DateTime? ServerStringToDate(string s)
        {
            return ConvertServerStringToDateTime(s, SERVER_DATE_FORMAT, false);
        }

        public DateTime? ServerStringToTime(string s)
        {
            return ConvertServerStringToDateTime(s, SERVER_TIME_FORMAT, true);
        }


        private string ConvertToServerString(DateTime? datetime, string format, bool toUTC)
        {
            if (datetime == null) return null;
            var dt = datetime.Value;
            if (toUTC)
            {
                dt = dt.ToUniversalTime();
            }
            return dt.ToString(format);
        }

        public string DateTimeToServerString(DateTime? datetime)
        {
            return ConvertToServerString(datetime, SERVER_DATETIME_FORMAT, true);
        }

        public string DateToServerString(DateTime? datetime)
        {
            return ConvertToServerString(datetime, SERVER_DATE_FORMAT, false);
        }

        public string TimeToServerString(DateTime? datetime)
        {
            return ConvertToServerString(datetime, SERVER_TIME_FORMAT, true);
        }

        #endregion

        #region Display datetime format methods

        private string ConvertToDisplayString(DateTime? datetime, string format, bool toUTC)
        {
            if (datetime == null) return null;
            var dt = datetime.Value;

            if (toUTC)
            {
                dt = dt.ToUniversalTime();
                if (datetime.Value.Year != DateTime.Now.Year)
                {
                    var yearStr = "\'" + datetime.Value.ToString(DISPLAY_SHORT_YEAR);
                    return dt.ToString(format) + " " + yearStr;
                }
            }

            return dt.ToString(format);
        }

        public string DateToDisplayString(DateTime? datetime)
        {
            return ConvertToDisplayString(datetime, DISPLAY_DATE_FORMAT, true);
        }

#if __IOS__
        public string DateTimeToDisplayString(DateTime? datetime)
        {
            if (new NSDateFormatter().Is24HourFormat())
                return ConvertToDisplayString(datetime, DISPLAY_DATETIME_24H_FORMAT, false);
            else
            {
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru")
                    return ConvertToDisplayString(datetime, DISPLAY_DATETIME_12H_SHORT_FORMAT, false);
                else
                    return ConvertToDisplayString(datetime, DISPLAY_DATETIME_12H_FORMAT, false);
            }
        }

        public string TimeToDisplayString(DateTime? datetime)
        {
            if (new NSDateFormatter().Is24HourFormat())
                return ConvertToDisplayString(datetime, DISPLAY_TIME_24H_FORMAT, false);
            else
            {
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru")
                    return ConvertToDisplayString(datetime, DISPLAY_TIME_12H_SHORT_FORMAT, false);
                else
                    return ConvertToDisplayString(datetime, DISPLAY_TIME_12H_FORMAT, false);
            }
        }
#endif

#if __ANDROID__

        public string TimeToDisplayString(DateTime? datetime)
        {
            return ConvertToDisplayString(datetime, DISPLAY_TIME_24H_FORMAT, false);
        }

        public string TimeToDisplayString(DateTime? datetime, Context context)
        {
            if (DateFormat.Is24HourFormat(context))
                return ConvertToDisplayString(datetime, DISPLAY_TIME_24H_FORMAT, false);
            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru")
                return ConvertToDisplayString(datetime, DISPLAY_TIME_12H_SHORT_FORMAT, false);
            return ConvertToDisplayString(datetime, DISPLAY_TIME_12H_FORMAT, false);
        }

        public string DateTimeToDisplayString(DateTime? datetime)
        {
            return ConvertToDisplayString(datetime, DISPLAY_DATETIME_24H_FORMAT, false);
        }

        public string DateTimeToDisplayString(DateTime? datetime, Context context)
        {
            if (DateFormat.Is24HourFormat(context))
                return ConvertToDisplayString(datetime, DISPLAY_DATETIME_24H_FORMAT, false);
            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru")
                return ConvertToDisplayString(datetime, DISPLAY_DATETIME_12H_SHORT_FORMAT, false);
            return ConvertToDisplayString(datetime, DISPLAY_DATETIME_12H_FORMAT, false);
        }
#endif

        #endregion
    }
}

