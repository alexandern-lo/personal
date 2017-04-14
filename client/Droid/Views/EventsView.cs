using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using SE.Emilsjolander.Stickylistheaders;
using LiveOakApp.Droid.CustomViews.Adapters;
using Android.Animation;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V4.Content;
using Android.Graphics;

namespace LiveOakApp.Droid.Views
{
    public class EventsView : FrameLayout
	{
		
		public EventsView(Context context) :
			base(context)
		{
			Initialize();
		}

		public EventsView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public EventsView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

        public SwipeRefreshLayout EventListRefresher { get; private set; }
		public StickyListHeadersListView EventList { get; private set; }
		public ImageButton LeadButton { get; private set; }
		public View ProgressBar { get; private set; }

        private bool isInShowMode;
        public bool IsInShowMode 
        {
            get
            {
                return isInShowMode;
            }
            set
            {
                isInShowMode = value;

                if (IsInShowMode)
                    LeadButton.Visibility = ViewStates.Visible;
                else
                    LeadButton.Visibility = ViewStates.Gone;
            }
        }

        bool isShowingProgressBar;
        public bool IsShowingProgressBar 
        { 
            get
            {
                return isShowingProgressBar;
            }
            set
            {
                isShowingProgressBar = value;

                if (isShowingProgressBar)
                {
                    if(ViewCompat.IsLaidOut(this))
                    {
                        animateShowProgressBar();
                    }
                    else
                    {
                        ProgressBar.Visibility = ViewStates.Visible;
                        EventList.SetY(TypedValue.ApplyDimension(ComplexUnitType.Dip, 50, Context.Resources.DisplayMetrics));
                    }
                }
                else if(ViewCompat.IsLaidOut(this))
                {
                    animateHideProgressBar();
                    if(IsInShowMode)
                        LeadButton.Visibility = ViewStates.Visible;
                }
            }
        }

        void animateShowProgressBar()
        {
            var animator = CreateEventsScroll(0, ProgressBar.Height);
            animator.AnimationStart += (sender, e) => ProgressBar.Visibility = ViewStates.Visible;
            animator.Start();
        }

        void animateHideProgressBar()
        {
            var animator = CreateEventsScroll(ProgressBar.Height, 0);
            animator.AnimationEnd += (sender, e) => ProgressBar.Visibility = ViewStates.Invisible;
            animator.Start();
        }

        ValueAnimator CreateEventsScroll(int y0, int y1)
        {
            var animator = ValueAnimator.OfInt(y0, y1);
            animator.SetDuration(500);
            animator.Update += (sender, e) => EventList.SetY((int)e.Animation.AnimatedValue);
            return animator;
        }

		void Initialize()
		{
			Inflate(Context, Resource.Layout.EventsFragment, this);
            EventListRefresher = FindViewById<SwipeRefreshLayout>(Resource.Id.events_list_refresher);
            EventList = FindViewById<StickyListHeadersListView>(Resource.Id.eventsList);
			LeadButton = FindViewById<ImageButton>(Resource.Id.leadButton);
			ProgressBar = FindViewById(Resource.Id.progressBar);
		}


        public StickyHeadersListViewAdapter<EventsViewModel.Section, EventViewModel> GetSectionsAdapter(ObservableList<EventsViewModel.Section> sections)
        {
            return sections.GetStickyHeadersAdapter<EventsViewModel.Section, EventViewModel>(GetEventItemView, GetSectionHeaderView);
        }

        public View GetEventItemView(int position, EventsViewModel.Section section, EventViewModel data, View convertView, View parent)
        {
            EventItemView view;

            if (convertView != null)
            {
                view = (EventItemView)convertView;
            }
            else
            {
                view = new EventItemView(parent.Context);
            }
            view.Event = data;

            view.Clickable = false; // Event item is clickable

            if (section.Header.Equals(EventsViewModel.SectionHeader.Recent))
            {
                view.Root.Background = ContextCompat.GetDrawable(Context, Resource.Drawable.selector_light);
            }
            else
            {
                view.Root.Background = ContextCompat.GetDrawable(Context, Resource.Drawable.selector_white_to_light);
            }

            if (section.Last() == data)
            {
                view.Divider.Visibility = ViewStates.Gone;
            }
            else
            {
                view.Divider.Visibility = ViewStates.Visible;
            }

            return view;
        }

        public View GetSectionHeaderView(int position, EventsViewModel.Section section, View convertView, View parent)
        {
            var context = parent.Context;

            EventSectionItemView view;
            if (convertView != null)
            {
                view = (EventSectionItemView)convertView;
            }
            else
            {
                view = new EventSectionItemView(context);
            }

            switch (section.Header)
            {
                case EventsViewModel.SectionHeader.Now:
                    {
                        view.HeaderText.Text = L10n.Localize("SectionNow", "Happening now");
                        var white_color = new Color(ContextCompat.GetColor(Context, Android.Resource.Color.White));
                        view.HeaderText.SetTextColor(white_color);
                        view.HeaderText.SetBackgroundResource(Resource.Color.primary_blue);
                        view.Divider.Visibility = ViewStates.Gone;
                        break;
                    }
                case EventsViewModel.SectionHeader.Upcoming:
                    {
                        view.HeaderText.Text = L10n.Localize("SectionUpcoming", "Upcoming events");
                        var gray_color = new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray));
                        view.HeaderText.SetTextColor(gray_color);
                        view.HeaderText.SetBackgroundResource(Resource.Color.primary_light);
                        view.Divider.Visibility = ViewStates.Gone;
                        break;
                    }
                case EventsViewModel.SectionHeader.Recent:
                    {
                        view.HeaderText.Text = L10n.Localize("SectionRecent", "Recent events");
                        var gray_color = new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray));
                        view.HeaderText.SetTextColor(gray_color);
                        view.HeaderText.SetBackgroundResource(Resource.Color.primary_light);
                        view.Divider.Visibility = ViewStates.Visible;
                        break;
                    }
            }

            view.Clickable = true; // Section header is not clickable

            return view;
        }
	}
}

