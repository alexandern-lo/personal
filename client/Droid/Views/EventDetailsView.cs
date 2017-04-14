using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

using StudioMobile;

namespace LiveOakApp.Droid.Views
{
    public class EventDetailsView : FrameLayout
    {
        public EventDetailsView(Context context) :
            base(context)
        {
            Initialize();
        }

        public EventDetailsView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public EventDetailsView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }


        public TextView EventType { get; private set; }
        public TextView EventDate { get; private set; }
        public TextView EventLocation { get; private set; }
        public TextView EventUrl { get; private set; }
        public TextView EventExpenses { get; private set; }
        public Button AddExpensesButton { get; private set; }
        public View EventAttendees { get; private set; }
        public View EventAgenda { get; private set; }
        public RemoteImageView EventLogo { get; private set; }
        public TextView EventIndustry { get; private set; }
        public TextView EventIsReccuring { get; private set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.EventDetailsLayout, this);

            EventIndustry = FindViewById<TextView>(Resource.Id.event_industry);
            EventType = FindViewById<TextView>(Resource.Id.event_type);
            EventDate = FindViewById<TextView>(Resource.Id.event_date_text);
            EventLocation = FindViewById<TextView>(Resource.Id.event_location);
            EventUrl = FindViewById<TextView>(Resource.Id.event_url);
            EventAttendees = FindViewById<View>(Resource.Id.attendees_holder);
            EventAgenda = FindViewById<View>(Resource.Id.agenda_holder);
            EventLogo = FindViewById<RemoteImageView>(Resource.Id.event_logo);
            EventExpenses = FindViewById<TextView>(Resource.Id.total_expenses);
            AddExpensesButton = FindViewById<Button>(Resource.Id.add_expense_button);
            EventIsReccuring = FindViewById<TextView>(Resource.Id.event_reccuring);

            EventUrl.PaintFlags = EventUrl.PaintFlags | Android.Graphics.PaintFlags.UnderlineText;

        }

        public void ShowReccuringStatus()
        {
            EventIsReccuring.Visibility = ViewStates.Visible;
        }

        public void HideReccuringStatus()
        {
            EventIsReccuring.Visibility = ViewStates.Gone;
        }

        public void HideAgendaAndAttendeesButtons() 
        {
            EventAttendees.Visibility = ViewStates.Gone;
            EventAgenda.Visibility = ViewStates.Gone;
        }

        public void ShowAgendaAndAttendeesButtons()
        {
            EventAttendees.Visibility = ViewStates.Visible;
            EventAgenda.Visibility = ViewStates.Visible;
        }
    }
}

