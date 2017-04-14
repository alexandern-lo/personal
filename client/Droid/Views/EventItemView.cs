using Android.Content;
using Android.Views;
using Android.Widget;
using LiveOakApp.Models.Data;
using LiveOakApp.Models.ViewModels;

namespace LiveOakApp.Droid.Views
{

	public class EventItemView : FrameLayout
	{
		EventViewModel @event;
        EventDetailsViewModel viewModel;

		public TextView DateRangeText { get; private set; }
		public TextView TitleText { get; private set; }
		public TextView LocationText { get; private set; }
		public View Divider { get; private set; }
		public View Root { get; private set; }

		public EventItemView (Context context) : base (context)
		{
			Initialize ();
		}

		void Initialize ()
		{
			Inflate (Context, Resource.Layout.EventItem, this);

			DateRangeText = FindViewById<TextView> (Resource.Id.event_time_range);
			TitleText = FindViewById<TextView> (Resource.Id.event_title);
			LocationText = FindViewById<TextView> (Resource.Id.event_location);
			Divider = FindViewById(Resource.Id.bot_divider);
			Root = GetChildAt(0);
		}


		public EventViewModel Event {
			get {
				return @event;
			}
			set {
				@event = value;
                viewModel = new EventDetailsViewModel(@event);
				TitleText.Text = viewModel.Title;
				DateRangeText.Text = viewModel.Date;
				LocationText.Text = viewModel.Location;
			}
		}

	}

}
