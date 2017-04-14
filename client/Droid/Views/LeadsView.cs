using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using LiveOakApp.Models.Data;
using LiveOakApp.Models.ViewModels;
using StudioMobile;
using Android.Support.V4.Widget;
using Android.Animation;
using Android.Support.V4.View;

namespace LiveOakApp.Droid.Views
{
	public class LeadsView : FrameLayout
	{
		public LeadsView(Context context) :
			base(context)
		{
			Initialize();
		}

		public LeadsView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public LeadsView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

        public TextView EventView { get; private set; }
		public EditText SearchText { get; private set; }
        public SwipeRefreshLayout LeadListRefresher { get; private set; }
		public ListView LeadList { get; private set; }
		public Button BusinessCardScanButton { get; private set; }
		public Button QrBarCardScanButton { get; private set; }
		public Button ManualEntryButton { get; private set; }

        public View ContentView { get; private set; }
		public View ProgressBar { get; private set; }
        public View BottomPanel { get; private set; }

        bool _eventsLoading;
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

		bool _leadsLoading;
		public bool LeadsLoading
		{
			get
			{
				return _leadsLoading;
			}
			set
			{
				_leadsLoading = value;
				RefreshProgressBarState();
			}
		}

		void RefreshProgressBarState()
		{
            IsShowingProgressBar = LeadsLoading || EventsLoading;
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

		void Initialize()
		{
			Inflate(Context, Resource.Layout.LeadsLayout, this);

            ContentView = FindViewById(Resource.Id.content_view);
            EventView = FindViewById<TextView>(Resource.Id.current_event);
			SearchText = FindViewById<EditText>(Resource.Id.search_txt);
            LeadListRefresher = FindViewById<SwipeRefreshLayout>(Resource.Id.leads_list_refresher);
			LeadList = FindViewById<ListView>(Resource.Id.lead_list);
			BusinessCardScanButton = FindViewById<Button>(Resource.Id.business_scan_btn);
			QrBarCardScanButton = FindViewById<Button>(Resource.Id.qr_bar_scan_btn);
			ManualEntryButton = FindViewById<Button>(Resource.Id.manual_entry_btn);
            BottomPanel = FindViewById(Resource.Id.create_lead_buttons_view);

			ProgressBar = FindViewById(Resource.Id.progressBar);

            LeadList.ScrollStateChanged += (sender, scrollArgs) =>
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

		public ObservableAdapter<LeadViewModel> PersonListAdapter(ObservableList<LeadViewModel> persons)
		{
			return persons.GetAdapter(GetPersonItemView);
		}

		public static View GetPersonItemView(int position, LeadViewModel person, View convertView, View parent)
		{
			var view = convertView != null ? (LeadItemView)convertView : new LeadItemView(parent.Context);
			view.Person = person;
			return view;
		}
	}
}

