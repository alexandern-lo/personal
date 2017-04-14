using System;
using System.Collections.Generic;
using System.Drawing;

namespace StudioMobile
{
	static class DatesRangeRendererExtension
	{
		public static DateTime DateAtPoint (this DayRange range, PointF point, RectangleF bounds)
		{
			var cellSize = new SizeF (
				               bounds.Width / range.Width,
				               bounds.Height / range.Height);
			var x = (int)(point.X / cellSize.Width);
			var y = (int)(point.Y / cellSize.Height);
			return range.FirstVisibleDate.AddDays (y * range.Width + x);
		}

		public static SizeF CellSize (this DayRange range, RectangleF bounds)
		{
			return new SizeF (
				bounds.Width / range.Width,
				bounds.Height / range.Height
			);
		}

		public static IEnumerable<KeyValuePair<DateTime, RectangleF>> Cells (this DayRange range, RectangleF bounds)
		{
			var cellSize = new SizeF (bounds.Width / range.Width, bounds.Height / range.Height);
			int i = 0;
			foreach (var date in range) {
				var rect = CellRect (bounds, cellSize, i);
				yield return new KeyValuePair<DateTime, RectangleF> (date, rect);
				i++;
			}
		}

		static RectangleF CellRect (RectangleF bounds, SizeF cellSize, int dayIdx)
		{
			return new RectangleF (
				bounds.X + cellSize.Width * (dayIdx % WallCalendar.DAYS_IN_WEEK), 
				bounds.Y + cellSize.Height * (dayIdx / WallCalendar.DAYS_IN_WEEK),
				cellSize.Width, 
				cellSize.Height);
		}
	}
	
}
