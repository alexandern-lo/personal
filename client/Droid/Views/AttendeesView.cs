using System;
using Android.Animation;
using Android.Content;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using LiveOakApp.Models.Data;
using LiveOakApp.Models.ViewModels;
using StudioMobile;

namespace LiveOakApp.Droid.Views
{
	public class AttendeesView : FrameLayout
	{
		public AttendeesView(Context context) :
			base(context)
		{
			Initialize();
		}

		public AttendeesView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public AttendeesView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

        public Action<int> OnAttendeeAtIndexWillBeShown;

        public TextView EventView { get; private set; }
		public EditText SearchText { get; private set; }
		public ImageButton FilterButton { get; private set; }
        public SwipeRefreshLayout AttendeesListRefresher { get; private set; }
		public ListView AttendeesList { get; private set; }
		public Button BusinessCardScanButton { get; private set; }
		public Button QrBarCardScanButton { get; private set; }
		public Button ManualEntryButton { get; private set; }

        public View ContentView { get; private set; }
		public View ProgressBar { get; private set; }

        public ProgressBar FooterProgressBar { get; private set; }

        public ErrorMessageView ErrorMessage { get; private set; }


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
                    if (ViewCompat.IsLaidOut(this))
                    {
                        animateShowProgressBar();
                    }
                    else
                    {
                        ProgressBar.Visibility = ViewStates.Visible;
                        ContentView.SetY(TypedValue.ApplyDimension(ComplexUnitType.Dip, 50, Context.Resources.DisplayMetrics));
                    }
                }
                else if (ViewCompat.IsLaidOut(this))
                {
                    animateHideProgressBar();
                }
            }
        }

        void animateShowProgressBar()
        {
            var animator = CreateContentScroll(0, ProgressBar.Height);
            animator.AnimationStart += (sender, e) => ProgressBar.Visibility = ViewStates.Visible;
            animator.Start();
        }

        void animateHideProgressBar()
        {
            var animator = CreateContentScroll(ProgressBar.Height, 0);
            animator.AnimationEnd += (sender, e) => ProgressBar.Visibility = ViewStates.Invisible;
            animator.Start();
        }

        ValueAnimator CreateContentScroll(int y0, int y1)
        {
            var animator = ValueAnimator.OfInt(y0, y1);
            animator.SetDuration(500);
            animator.Update += (sender, e) => ContentView.SetY((int)e.Animation.AnimatedValue);
            return animator;
        }


		private bool _eventsLoading;

		public bool EventsLoading
		{
			get
			{
				return _eventsLoading;
			}
			set
			{
				_eventsLoading = value;
				RefreshProgressBarState();
			}
		}

		private bool _attendeesLoading;

		public bool AttendeesLoading
		{
			get
			{
				return _attendeesLoading;
			}
			set
			{
				_attendeesLoading = value;
				RefreshProgressBarState();
			}
		}

		void RefreshProgressBarState()
		{
            IsShowingProgressBar = EventsLoading || AttendeesLoading;
		}

        bool isShowingError = false;

        public void ShowError()
        {
            ErrorMessage.Visibility = ViewStates.Visible;
            ErrorMessage.SetShowsButton(false);
            AttendeesList.Visibility = ViewStates.Gone;
            isShowingError = true;
        }

        public void ShowErrorWithButton()
        {
            isShowingError = true;
            ErrorMessage.Visibility = ViewStates.Visible;
            ErrorMessage.SetShowsButton(true);
            AttendeesList.Visibility = ViewStates.Gone;
        }

        public void HideError()
        {
            isShowingError = false;
            ErrorMessage.Visibility = ViewStates.Gone;
            AttendeesList.Visibility = ViewStates.Visible;
        }

		void Initialize()
		{
			Inflate(Context, Resource.Layout.AttendeesListLayout, this);

            ContentView = FindViewById(Resource.Id.content_view);
            EventView = FindViewById<TextView>(Resource.Id.current_event);
			SearchText = FindViewById<EditText>(Resource.Id.search_txt);
			FilterButton = FindViewById<ImageButton>(Resource.Id.filter_button);
            AttendeesListRefresher = FindViewById<SwipeRefreshLayout>(Resource.Id.attendees_list_refresher);
			AttendeesList = FindViewById<ListView>(Resource.Id.attendees_list);
			BusinessCardScanButton = FindViewById<Button>(Resource.Id.business_scan_btn);
			QrBarCardScanButton = FindViewById<Button>(Resource.Id.qr_bar_scan_btn);
			ManualEntryButton = FindViewById<Button>(Resource.Id.manual_entry_btn);

			ProgressBar = FindViewById(Resource.Id.progressBar);

            ErrorMessage = FindViewById<ErrorMessageView>(Resource.Id.error_view);

            FooterProgressBar = new ProgressBar(Context);
            FooterProgressBar.LayoutParameters = new AbsListView.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                                                                            (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 50, Context.Resources.DisplayMetrics));
            FooterProgressBar.Indeterminate = true;

            AttendeesList.AddFooterView(FooterProgressBar);

            AttendeesList.Recycler += (sender, e) =>
            {
                var view = e.View as AttendeesItemView;
                if (view == null) return;
                view.Recycle();
            };

            AttendeesList.ScrollStateChanged += (sender, scrollArgs) =>
            {
                switch (scrollArgs.ScrollState)
                {
                    case ScrollState.Fling:
                        ImageService.Instance.SetPauseWork(true);
                        break;
                    case ScrollState.Idle:
                        ImageService.Instance.SetPauseWork(false);
                        break;
                }
            };
		}

        public void SetFooterProgressBarEnabled(bool isEnabled)
        {
            FooterProgressBar.Visibility = isEnabled ? ViewStates.Visible : ViewStates.Gone;
        }

		public ObservableAdapter<AttendeeViewModel> PersonListAdapter(ObservableList<AttendeeViewModel> persons)
		{
			return persons.GetAdapter(GetPersonItemView);
		}

		public View GetPersonItemView(int position, AttendeeViewModel person, View convertView, View parent)
		{
            if (OnAttendeeAtIndexWillBeShown != null)
            {
                OnAttendeeAtIndexWillBeShown(position);
            }
            var view = convertView != null ? (AttendeesItemView)convertView : new AttendeesItemView(parent.Context);
			view.Person = person;
			return view;
		}
	}
}

