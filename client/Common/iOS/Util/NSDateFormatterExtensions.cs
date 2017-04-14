using Foundation;

namespace StudioMobile
{
    public static class NSDateFormatterExtensions
    {
        public static bool Is24HourFormat(this NSDateFormatter dateFormatter)
        {
            dateFormatter.DateStyle = NSDateFormatterStyle.None;
            dateFormatter.TimeStyle = NSDateFormatterStyle.Short;
            var dateString = dateFormatter.ToString(NSDate.Now);
            var is12Hour = dateString.Contains(dateFormatter.AMSymbol) || dateString.Contains(dateFormatter.PMSymbol);
            return !is12Hour;
        }
    }
}
