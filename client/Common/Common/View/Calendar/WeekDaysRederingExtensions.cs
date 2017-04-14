using System;
using System.Collections.Generic;
using System.Drawing;

namespace StudioMobile
{
	static class WeekDaysRederingExtensions
	{
		public static IEnumerable<KeyValuePair<DayOfWeek, RectangleF>> Cells (this WeekDays header, RectangleF bounds)
		{
			var cellWidth = bounds.Width / WallCalendar.DAYS_IN_WEEK;
			for (int i = 0; i < WallCalendar.DAYS_IN_WEEK; ++i) {
				yield return new KeyValuePair<DayOfWeek, RectangleF> (
					header [i],
					new RectangleF (bounds.X + i * cellWidth, bounds.Y, cellWidth, bounds.Height)
				);
			}
		}

	}
}
