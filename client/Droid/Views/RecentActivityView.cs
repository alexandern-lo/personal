
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using LiveOakApp.Models.ViewModels;
using StudioMobile;

namespace LiveOakApp.Droid.Views
{
    public class RecentActivityView : FrameLayout
    {
        public RecentActivityView(Context context) :
            base(context)
        {
            Initialize();
        }

        public RecentActivityView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public RecentActivityView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        public TextView EventView { get; private set; }
        public EditText SearchText { get; private set; }
        public SwipeRefreshLayout LeadActivityRefresher { get; private set; }
        public ListView LeadList { get; private set; }
        public Button BusinessCardScanButton { get; private set; }
        public Button QrBarCardScanButton { get; private set; }
        public Button ManualEntryButton { get; private set; }
        public ErrorMessageView ErrorView { get; private set; }

        public View ContentView { get; private set; }
        public View ProgressBar { get; private set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.RecentActivityLayout, this);

            ContentView = FindViewById(Resource.Id.content_view);
            LeadActivityRefresher = FindViewById<SwipeRefreshLayout>(Resource.Id.leads_list_refresher);
            LeadList = FindViewById<ListView>(Resource.Id.lead_list);
            BusinessCardScanButton = FindViewById<Button>(Resource.Id.business_scan_btn);
            QrBarCardScanButton = FindViewById<Button>(Resource.Id.qr_bar_scan_btn);
            ManualEntryButton = FindViewById<Button>(Resource.Id.manual_entry_btn);
            ErrorView = FindViewById<ErrorMessageView>(Resource.Id.error_view);

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

        public void ShowError()
        {
            ErrorView.Visibility = ViewStates.Visible;
            ErrorView.SetShowsButton(false);
            LeadList.Visibility = ViewStates.Gone;
        }

        public void ShowErrorWithButton()
        {
            ErrorView.Visibility = ViewStates.Visible;
            ErrorView.SetShowsButton(true);
            LeadList.Visibility = ViewStates.Gone;
        }

        public void HideError()
        {
            ErrorView.Visibility = ViewStates.Gone;
            LeadList.Visibility = ViewStates.Visible;
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

        public ObservableAdapter<LeadRecentActivityViewModel> LeadActivityListAdapter(ObservableList<LeadRecentActivityViewModel> leadAcitivities)
        {
            return leadAcitivities.GetAdapter(GetPersonItemView);
        }

        public static View GetPersonItemView(int position, LeadRecentActivityViewModel leadActivity, View convertView, View parent)
        {
            var view = (LeadActitvityItemView)convertView ?? new LeadActitvityItemView(parent.Context);
            view.Lead = leadActivity;
            //return new LeadActitvityItemView(parent.Context);
            return view;
        }
    }
}
