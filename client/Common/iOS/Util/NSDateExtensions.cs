using System;
using Foundation;

namespace StudioMobile
{
	public static class NSDateExtensions
	{
		public static NSDate ToNSDate (this DateTime dt) {
			if (dt.Kind == DateTimeKind.Unspecified) {
				throw new ArgumentException ("DateTimeKind.Unspecified cannot be safely converted");
			}
			dt = dt.ToUniversalTime ();
			var seconds = (dt - ReferenceDate).TotalSeconds;
			return NSDate.FromTimeIntervalSinceReferenceDate (seconds);
		}

		private static DateTime ReferenceDate = new DateTime(2001,1,1,0,0,0, DateTimeKind.Utc);
		public static DateTime ToDateTime (this NSDate d) {
			double seconds = d.SecondsSinceReferenceDate;
			return ReferenceDate.AddSeconds (seconds);
		}
	}
}

