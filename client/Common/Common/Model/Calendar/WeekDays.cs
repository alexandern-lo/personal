using System;
using System.Collections.Generic;
using System.Collections;

namespace StudioMobile
{
	public class WeekDays : IEnumerable<DayOfWeek>
	{
		readonly internal WallCalendar grid;

		public WeekDays (WallCalendar grid)
		{
			this.grid = grid;
		}

		public DayOfWeek FirstDayOfWeek { get; private set; }

		public string Title (DayOfWeek day)
		{
			return grid.Culture.DateTimeFormat.AbbreviatedDayNames [(int)day];
		}

		IEnumerable<DayOfWeek> Days ()
		{
			var day = grid.FirstDayOfWeek;
			for (int i = 0; i < WallCalendar.DAYS_IN_WEEK; ++i) {
				yield return day;
				day++;
				if (day > DayOfWeek.Saturday) {
					day = DayOfWeek.Sunday;
				}
			}			
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return Days ().GetEnumerator ();
		}

		public IEnumerator<DayOfWeek> GetEnumerator ()
		{
			return Days ().GetEnumerator ();
		}

		public DayOfWeek this [int idx] {
			get { 
				if (idx < 0 || idx >= WallCalendar.DAYS_IN_WEEK)
					throw new ArgumentOutOfRangeException ();
				idx = (int)(idx + grid.FirstDayOfWeek) % WallCalendar.DAYS_IN_WEEK;
				return (DayOfWeek)idx;
			}
		}
	}
	
}
