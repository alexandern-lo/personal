using System;
using Foundation;
using UIKit;
using System.Drawing;
using System.Globalization;
using CoreGraphics;
using CoreAnimation;
using StudioMobile;
using System.Collections.Generic;

namespace StudioMobile
{
	[Register ("CalendarView")]
	public class CalendarView : UIScrollView
	{
		//data
		internal WallCalendar grid;
		//taps
		UITapGestureRecognizer tap;

		public CalendarView ()
		{
			InitCalendarView ();
		}

		public CalendarView (IntPtr handle) : base (handle)
		{
			InitCalendarView ();
		}

		public CalendarView (NSObjectFlag t) : base (t)
		{
			InitCalendarView ();
		}

		public CalendarView (CGRect frame) : base (frame)
		{
			InitCalendarView ();
		}

		void InitCalendarView ()
		{
			grid = new WallCalendar ();

			HeaderHeight = 20;
			PagingEnabled = true;
			ShowsVerticalScrollIndicator = false;
			ShowsHorizontalScrollIndicator = false;
			ContentOffset = new CGPoint (Bounds.Width, 0);
			grid.Style = CalendarStyle.Week;

			headers = new HeadersCollection (this);
			cells = new CellsCollection (this);

			HeaderStyle = CalendarHeaderStyle.Static;

			tap = new UITapGestureRecognizer ();
			tap.AddTarget (OnTap);
			AddGestureRecognizer (tap);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				RemoveGestureRecognizer (tap);
				tap.Dispose ();
			}

			base.Dispose (disposing);
		}

		ICalendarViewDelegate viewDelegate = new CalendarViewDelegate ();

		public new ICalendarViewDelegate Delegate { 
			get { return viewDelegate; }
			set { 
				if (value == null)
					throw new ArgumentNullException ();
				if (viewDelegate != value) {
					viewDelegate = value; 
					SetContentOffset (new CGPoint (Bounds.Width, 0), false);
					ReloadData ();
				}
			}
		}
						
		HeadersCollection headers;
		CellsCollection cells;
		
		//LayoutSubviews called whenever scroll happens changes
		public override void LayoutSubviews ()
		{
			var contentWidth = Bounds.Width * 3;
			if (ContentSize.Width != contentWidth) {
				ContentSize = new CGSize (contentWidth, Bounds.Height);
				ContentOffset = CurrentPageOffset;
			}
			if (ContentOffset.X <= 0) {
				ContentOffset = CurrentPageOffset;
				MoveTo (grid.PreviousRange ().Begin);
			} else if (ContentOffset.X >= ContentSize.Width - Bounds.Width) {
				ContentOffset = CurrentPageOffset;
				MoveTo (grid.NextRange ().Begin);
			}

			var rangeSize = new SizeF((float)Bounds.Width, (float)Bounds.Height);
			LayoutRange (grid.ActiveRange (), new RectangleF ((PointF)CurrentPageOffset, rangeSize));
			if (ContentOffset.X + Bounds.Width >= NextPageOffset.X) {
				LayoutRange (grid.NextRange (), new RectangleF ((PointF)NextPageOffset, rangeSize));
			}
			if (ContentOffset.X <= PrevPageOffset.X + Bounds.Width) {
				LayoutRange (grid.PreviousRange (), new RectangleF ((PointF)PrevPageOffset, rangeSize));
			}

			if (HeaderStyle == CalendarHeaderStyle.Static) {
				var headerBounds = this.LayoutBox ()
					.Top (0).Left (0).Right (0).Height (HeaderHeight).Bounds;
				LayoutHeader (grid.ActiveRange(), headerBounds);
			}

			cells.Cleanup ();

			headers.Cleanup ();
		}

		void LayoutRange (DayRange range, RectangleF rangeBounds)
		{
			if (HeaderStyle == CalendarHeaderStyle.Paged) {
				var headerBounds = new LayoutBox (rangeBounds)
					.Top (0).Left (0).Right (0).Height (HeaderHeight).Bounds;
				LayoutHeader (range, headerBounds);
				rangeBounds = new LayoutBox (rangeBounds)
					.Top (HeaderHeight).Left (0).Right (0).Bottom (0).Bounds;
			}

			foreach (var kv in range.Cells(rangeBounds)) {
				var date = kv.Key;
				var rect = kv.Value;
				var cell = cells.GetCell (date);
				if (rect.IntersectsWith ((RectangleF)Bounds)) {
					if (cell == null && Delegate != null) {
						cell = Delegate.GetCell (this, date, range);
						if (cell != null) {
							cells.Add (date, cell);
						}
					}
					if (cell != null) {
						cell.Frame = rect;
					}
				} else if (cell != null) {
					cells.Remove (date);
				}
			}
		}
			
		void LayoutHeader (DayRange range, RectangleF headerBounds)
		{
			foreach (var kv in grid.WeekDays ().Cells (headerBounds)) {
				var rect = kv.Value;
				var cell = headers.GetHeader (range, kv.Key);
				if (rect.IntersectsWith ((RectangleF)Bounds)) {
					if (cell == null && Delegate != null) {
						cell = Delegate.GetHeader (this, kv.Key, range);
						if (cell != null) {
							headers.AddHeader (cell, range, kv.Key);
						}
					}
					if (cell != null) {
						cell.Frame = rect;
					}
				} else if (cell != null) {
					headers.Remove (range, kv.Key);
				}
			}
		}

		readonly Pool<CalendarViewElement> pool = new Pool<CalendarViewElement> ();
		public CalendarViewElement DequeReusableElement (string reuseId)
		{
			return pool.Deque (reuseId);
		}

		CalendarHeaderStyle headerStyle = CalendarHeaderStyle.Static;

		public CalendarHeaderStyle HeaderStyle { 
			get { return headerStyle; }
			set { 
				headerStyle = value;
				headers.Clear ();
				SetNeedsLayout ();
			}
		}

		float headerHeight;

		public float HeaderHeight { 
			get { return headerHeight; }
			set {
				if (Math.Abs (headerHeight - value) > 0.5) {
					headerHeight = value;
					SetNeedsLayout ();
				}
			}
		}

		public CalendarViewElement GetVisibleCellAt (DateTime dt)
		{
			return cells.GetCell (dt);
		}

		public CalendarViewElement GetVisibleHeaderAt (DateTime date)
		{
			if (grid.ActiveRange ().Compare (date) == 0) {
				return headers.GetHeader (grid.ActiveRange (), date.DayOfWeek);
			}
			if (grid.NextRange ().Compare (date) == 0) {
				return headers.GetHeader (grid.NextRange (), date.DayOfWeek);
			}
			if (grid.PreviousRange ().Compare (date) == 0) {
				return headers.GetHeader (grid.PreviousRange (), date.DayOfWeek);
			}
			return null;
		}


		void OnTap ()
		{
			var location = tap.LocationInView (this);
			var p = new PointF ((float)(location.X - Bounds.Width), (float)(location.Y - HeaderHeight));
			var date = grid.ActiveRange ().DateAtPoint (p, new RectangleF(PointF.Empty, (Size)Bounds.Size));
			if (Delegate != null) {
				Delegate.CellSelected (this, date, GetVisibleCellAt (date));
			}
		}

		public void MoveTo (DateTime dt, bool animated)
		{
			if (animated) {
				//If selected date is not in displayed range
				//setup next or prev range view with and trigger UIScrolLView animation.
				//Once it is done callbacks from UIScrollView will call this method
				//again with animated = false
				//See MoveTo and ScrollFinished methods
				//(averbin)
				var compareResult = grid.ActiveRange ().Compare (dt); 
				if (compareResult > 0) {
					SetContentOffset (NextPageOffset, true);
				} else if (compareResult < 0) {
					SetContentOffset (PrevPageOffset, true);
				} else if (compareResult == 0) {
					//do nothing, requested month already displayed
				}
			} else {
				MoveTo (dt);
			}
		}

		void MoveTo(DateTime dt)
		{
			if (grid.ActiveRange ().Compare (dt) != 0) {
				grid.MoveTo (dt);
				if (DisplayedRangeChange != null)
					DisplayedRangeChange (this, EventArgs.Empty);
			}
			SetNeedsLayout ();
		}

		public event EventHandler DisplayedRangeChange;

		public int DisplayedMonth {
			get { return grid.Date.Month; }
		}

		public int DisplayedYear {
			get { return grid.Date.Year; }
		}

		public string Title {
			get {
				return grid.Date.ToString ("Y");
			}
		}

		public DayRange DisplayedDayRange {
			get { return grid.ActiveRange (); }
		}

		public CalendarStyle Style {
			get { return grid.Style; }
			set {
				if (value != grid.Style) {
					grid.Style = value;
					ReloadData ();
				}
			}
		}

		public void ReloadData ()
		{
			cells.Clear ();
			headers.Clear ();
			SetNeedsLayout ();
		}

		public void Reload(DateTime dt)
		{
			var range = grid.ActiveRange ();
			if (grid.ActiveRange ().Compare (dt) == 0) {
				if (Delegate != null) {
					var cell = Delegate.GetCell (this, dt, range);
					cells.Remove (dt);
					cells.Add (dt, cell);
					if (HeaderStyle == CalendarHeaderStyle.Paged) {
						var header = Delegate.GetHeader (this, dt.DayOfWeek, range);
						headers.Remove (range, dt.DayOfWeek);
						headers.AddHeader (header, range, dt.DayOfWeek);
					}
				}
			}
		}

		public DayOfWeek FirstDayOfWeek {
			get { return grid.FirstDayOfWeek; }
			set { 
				grid.FirstDayOfWeek = value;
				ReloadData ();
			}
		}

		public DayOfWeek LastDayOfWeek {
			get { return grid.LastDayOfWeek; }
		}

		public CultureInfo Culture {
			get { return grid.Culture; }
			set { 
				grid.Culture = value;
				ReloadData ();
			}
		}
			
		class CellsCollection {
			Dictionary<DateTime, CalendarViewElement> cells = new Dictionary<DateTime, CalendarViewElement> ();

			public CellsCollection(CalendarView owner) {
				Owner = owner;
			}
			public CalendarView Owner { get; private set; }

			public CalendarViewElement GetCell (DateTime dt)
			{
				if (cells.ContainsKey (dt)) {
					return cells [dt];
				} else {
					return null;
				}
			}

			public void Add (DateTime date, CalendarViewElement cell)
			{
				cells.Add (date, cell);
				Owner.AddSubview (cell);
				cell.SetNeedsLayout ();
			}

			public void Remove (DateTime date)
			{
				var cell = cells [date];
				cells.Remove (date);
				RemoveCell (cell);
			}
				
			void RemoveCell(CalendarViewElement cell)
			{
				cell.RemoveFromSuperview ();
				Owner.pool.Enqueue (cell, cell.ReuseId);
			}
				
			public void Cleanup ()
			{
				foreach (var date in new List<DateTime>(cells.Keys)) {
					if (!Owner.grid.Contains (date)) {
						Remove (date);
					}
				}
			}

			public void Clear()
			{
				foreach (var kv in cells) {
					RemoveCell (kv.Value);
				}
				cells.Clear ();
			}
		}

		class HeadersCollection
		{
			Dictionary<DayOfWeekKey, CalendarViewElement> headers = new Dictionary<DayOfWeekKey, CalendarViewElement> ();

			public HeadersCollection (CalendarView view)
			{
				Owner = view;
			}

			public CalendarView Owner { get; private set; }

			public CalendarHeaderStyle Style { get { return Owner.HeaderStyle; } }

			public DayOfWeek FirstDayOfWeek { get { return Owner.FirstDayOfWeek; } }

			public CalendarViewElement GetHeader (DayRange range, DayOfWeek weekDay)
			{
				var key = MakeKey (range, weekDay);
				if (headers.ContainsKey (key)) {
					return headers [key];
				} else {
					return null;
				}
			}

			public void AddHeader (CalendarViewElement header, DayRange range, DayOfWeek weekDay)
			{
				var key = MakeKey (range, weekDay);
				headers.Add (key, header);
				Owner.AddSubview (header);
				header.SetNeedsLayout ();
			}

			public void Remove (DayRange range, DayOfWeek weekDay)
			{
				var key = MakeKey (range, weekDay);
				RemoveHeader (key);
			}

			void RemoveHeader (DayOfWeekKey key)
			{
				if (headers.ContainsKey (key)) {
					var cell = headers[key];
					cell.RemoveFromSuperview ();
					headers.Remove (key);
					Owner.pool.Enqueue (cell, cell.ReuseId);
				}
			}

			public void Clear ()
			{
				foreach (var kv in headers) {
					kv.Value.RemoveFromSuperview ();					
				}
				headers.Clear ();
			}

			public void Cleanup ()
			{
				if (Style == CalendarHeaderStyle.Paged) {
					foreach (var key in new List<DayOfWeekKey>(headers.Keys)) {
						var date = key.FirstDayOfWeek + TimeSpan.FromDays ((int)key.WeekDay);
						if (!Owner.grid.Contains (date)) {
							RemoveHeader (key);
						}
					}
				}
			}

			DayOfWeekKey MakeKey (DayRange range, DayOfWeek weekDay)
			{
				var firstDayOfWeek = range.FirstVisibleDate;
				switch (Style) {
				case CalendarHeaderStyle.Paged:
					if (firstDayOfWeek.DayOfWeek != FirstDayOfWeek || firstDayOfWeek == DateTime.MinValue) {
						throw new ArgumentException ("range should start from FirstDayOfWeek", "range");
					}
					break;
				case CalendarHeaderStyle.Static:
					firstDayOfWeek = DateTime.MinValue;
					break;
				default:
					throw new NotImplementedException ();
				}
				return new DayOfWeekKey {
					FirstDayOfWeek = firstDayOfWeek,
					WeekDay = weekDay
				};
			}

			struct DayOfWeekKey
			{
				public DayOfWeek WeekDay;
				public DateTime FirstDayOfWeek;

				public override bool Equals (object obj)
				{
					if (!(obj is DayOfWeekKey))
						return base.Equals (obj);
					var other = (DayOfWeekKey)obj;
					return other.WeekDay == WeekDay && other.FirstDayOfWeek == FirstDayOfWeek;
				}

				public override int GetHashCode ()
				{
					var result = FirstDayOfWeek.Year;
					result = 29 * result + FirstDayOfWeek.Month;
					result = 29 * result + FirstDayOfWeek.Day;
					result = 29 * result + (int)WeekDay;
					return result;
				}
			}
		}

		CGPoint NextPageOffset {
			get { return new CGPoint (Bounds.Width * 2, 0); }
		}

		CGPoint PrevPageOffset {
			get { return new CGPoint (0, 0); }
		}

		CGPoint CurrentPageOffset {
			get { return new CGPoint (Bounds.Width, 0); }
		}
	}

	public class CalendarDayEventArgs : EventArgs
	{
		public DateTime New { get; internal set; }

		public DateTime Old { get; internal set; }
	}

	public class CalendarRangeEventArgs : EventArgs
	{
		public DayRange New { get; internal set; }

		public DayRange Old { get; internal set; }
	}

	public class CalendarViewElement : UIView
	{
		public CalendarViewElement ()
		{
		}
		

		public CalendarViewElement (NSCoder coder) : base (coder)
		{
		}
		

		public CalendarViewElement (NSObjectFlag t) : base (t)
		{
		}
		

		public CalendarViewElement (IntPtr handle) : base (handle)
		{
		}
		

		public CalendarViewElement (CGRect frame) : base (frame)
		{
		}
		

		UILabel title;

		public UILabel Title {
			get {
				if (title == null) {
					title = new UILabel ();
					AddSubview (title);
					SendSubviewToBack (title);
				}
				return title;
			}
		}

		public override void LayoutSubviews ()
		{
			if (title != null) {
				title.SizeToFit ();
				title.Frame = this.LayoutBox ()
					.CenterVertically ().CenterHorizontally ().Width (title).Height (title);
			}
		}

		public string ReuseId { get; set; }
	}

	public interface ICalendarViewDelegate
	{
		CalendarViewElement GetHeader (CalendarView view, DayOfWeek weekDay, DayRange range);

		CalendarViewElement GetCell (CalendarView view, DateTime day, DayRange range);

		void CellSelected(CalendarView view, DateTime day, CalendarViewElement cell);
	}

	public class CalendarViewDelegate : ICalendarViewDelegate
	{
		public virtual CalendarViewElement GetHeader (CalendarView view, DayOfWeek weekDay, DayRange range)
		{
			var cell = view.DequeReusableElement ("Header") ?? new CalendarViewElement { ReuseId = "Header" };
			cell.Title.Text = GetHeaderTitle (view, weekDay, range);
			return cell;
		}

		public virtual CalendarViewElement GetCell (CalendarView view, DateTime day, DayRange range)
		{
			var cell = view.DequeReusableElement ("Day") ?? new CalendarViewElement { ReuseId = "Day" };
			cell.Title.Text = GetCellTitle (view, day, range);
			return cell;
		}

		public virtual void CellSelected(CalendarView view, DateTime day, CalendarViewElement cell)
		{
		}

		public virtual string GetHeaderTitle (CalendarView view, DayOfWeek weekDay, DayRange range)
		{
			return view.Culture.DateTimeFormat.AbbreviatedDayNames [(int)weekDay];
		}

		public virtual string GetCellTitle (CalendarView view, DateTime day, DayRange range)
		{
			return day.Day.ToString ();
		}
	}

}

