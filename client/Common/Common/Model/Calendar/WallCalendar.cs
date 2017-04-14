using System;
using System.Globalization;

namespace StudioMobile
{
	public enum CalendarStyle
	{
		Month,
		Week
	}

	public enum CalendarHeaderStyle
	{
		Static,
		Paged
	}

	public class WallCalendar
	{
		public static readonly int DAYS_IN_WEEK = 7;
		public static readonly int ROWS_IN_MONTH_GRID = 6;
		public static readonly int ROWS_IN_WEEK_GRID = 1;
		private CultureInfo cultureInfo;

		public WallCalendar ()
		{
			var date = DateTime.Now.Date;
			Culture = CultureInfo.CurrentCulture;
			Style = CalendarStyle.Month;
			MoveTo (date);
		}

		public CalendarStyle Style { get; set; }

		public CultureInfo Culture {
			get { return cultureInfo; }
			set {
				cultureInfo = value;
				FirstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;
			}
		}

		public DayOfWeek FirstDayOfWeek { get; set; }

		public DayOfWeek LastDayOfWeek { 
			get { 
				var day = (int)(WallCalendar.DAYS_IN_WEEK - 1 + FirstDayOfWeek) % WallCalendar.DAYS_IN_WEEK;
				return (DayOfWeek)day;
			}
		}

		public string WeekDayTitle (DayOfWeek day)
		{
			return Culture.DateTimeFormat.AbbreviatedDayNames [(int)day];
		}

		public bool Contains (DateTime date)
		{
			if (ActiveRange ().Compare (date) == 0) {
				return true;
			}
			if (NextRange ().Compare (date) == 0) {
				return true;
			}
			if (PreviousRange ().Compare (date) == 0) {
				return true;
			}
			return false;
		}

		public DayRange ActiveRange ()
		{
			switch (Style) {
			case CalendarStyle.Month:
				return MonthRange (Date.Year, Date.Month);
			case CalendarStyle.Week:
				return WeekRange (Date);
			default:
				return null;
			}
		}

		public DayRange PreviousRange ()
		{
			switch (Style) {
			case CalendarStyle.Month:
				{
					var date = new DateTime (Date.Year, Date.Month, 1);
					date = date.AddMonths (-1);
					return MonthRange (date.Year, date.Month);	
				}
			case CalendarStyle.Week:
				return WeekRange (Date.AddDays (-DAYS_IN_WEEK));
			default:
				return null;
			}
		}

		public DayRange NextRange ()
		{
			switch (Style) {
			case CalendarStyle.Month:
				{
					var date = new DateTime (Date.Year, Date.Month, 1);
					date = date.AddMonths (1);
					return MonthRange (date.Year, date.Month);	
				}
			case CalendarStyle.Week:
				return WeekRange (Date.AddDays (DAYS_IN_WEEK));
			default:
				return null;
			}
		}

		public DayRange RangeForDate (DateTime date)
		{
			switch (Style) {
			case CalendarStyle.Month:
				return MonthRange (date.Year, date.Month);
			case CalendarStyle.Week:
				return WeekRange (date);
			default:
				return null;
			}
		}

		public void MoveTo (DateTime dt)
		{
			Date = dt.Date;
		}

		public WeekDays WeekDays ()
		{
			return new WeekDays (this);
		}

		public DateTime Date { get; private set; }

		static DateTime GetNextWeekday (DateTime start, DayOfWeek day)
		{
			// The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
			int daysToAdd = ((int)day - (int)start.DayOfWeek + WallCalendar.DAYS_IN_WEEK) % WallCalendar.DAYS_IN_WEEK;
			if (daysToAdd == 0)
				daysToAdd = WallCalendar.DAYS_IN_WEEK;
			return start.AddDays (daysToAdd);
		}

		DayRange MonthRange (int year, int month)
		{
			var begin = new DateTime (year, month, 1);
			var end = begin.AddMonths (1).AddDays (-1);
			var firstVisibleDate = GetNextWeekday (begin, FirstDayOfWeek).AddDays (-WallCalendar.DAYS_IN_WEEK);
			var lastVisibleDate = firstVisibleDate.AddDays (WallCalendar.DAYS_IN_WEEK * WallCalendar.ROWS_IN_MONTH_GRID);
			return new DayRange () { 
				Begin = begin,
				End = end,
				FirstVisibleDate = firstVisibleDate,
				LastVisibleDate = lastVisibleDate,
				Style = Style
			};
		}

		DayRange WeekRange (DateTime dt)
		{
			var begin = GetNextWeekday (dt, FirstDayOfWeek).AddDays (-WallCalendar.DAYS_IN_WEEK);
			var end = begin.AddDays (WallCalendar.DAYS_IN_WEEK - 1);
			return new DayRange () { 
				Begin = begin,
				End = end,
				FirstVisibleDate = begin,
				LastVisibleDate = end,
				Style = Style
			};
		}
	}




}
