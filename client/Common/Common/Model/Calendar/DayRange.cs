using System;
using System.Collections.Generic;
using System.Collections;

namespace StudioMobile
{

	public class DayRange : IEnumerable<DateTime>
	{
		public CalendarStyle Style { get; internal set; }

		public DateTime End { get; internal set; }

		public DateTime Begin { get; internal set; }

		public DateTime FirstVisibleDate { get; internal set; }

		public DateTime LastVisibleDate { get; internal set; }

		public int Width { 
			get { 
				return WallCalendar.DAYS_IN_WEEK;
			}
		}

		public int Height {
			get { 
				switch (Style) {
				case CalendarStyle.Month:
					return 6;
				case CalendarStyle.Week:
					return 1;
				default:
					return 0;
				}
			}
		}

		public bool Include (DateTime date)
		{
			return date >= Begin && date <= End;
		}

		public IEnumerable<DateTime> Range ()
		{
			DateTime start = FirstVisibleDate;
			while (start <= LastVisibleDate) {
				yield return start;
				start = start.AddDays (1);
			}
		}

		public DateTime At(int x, int y) {
			if (x >= Width || x < 0)
				throw new ArgumentOutOfRangeException ("x");
			if (y >= Height || y < 0)
				throw new ArgumentOutOfRangeException ("y");
			return FirstVisibleDate.AddDays (x + y*Width);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return Range ().GetEnumerator ();
		}

		public IEnumerator<DateTime> GetEnumerator ()
		{
			return Range ().GetEnumerator ();
		}

		public int Compare (DateTime dt)
		{
			if (dt > End) {
				return 1;
			} else if (dt < Begin) {
				return -1;
			} else {
				return 0;
			}
		}
	}
	
}
