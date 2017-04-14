using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using LiveOakApp.Resources;

namespace LiveOakApp.Droid.Views
{
    public class SubscriptionExpiredView : FrameLayout
    {
        public SubscriptionExpiredView(Context context) : base(context)
        {
            Initialize();
        }

        public SubscriptionExpiredView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public SubscriptionExpiredView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.SubscriptionExpiredView, this);
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            SetBackgroundResource(Resource.Drawable.login_bg);

            MessageTextView = FindViewById<TextView>(Resource.Id.message_textview);
            RecheckButton = FindViewById<Button>(Resource.Id.recheck_button);
            LogoutButton = FindViewById<Button>(Resource.Id.logout_button);
            ProgressBar = FindViewById<ProgressBar>(Resource.Id.recheck_progress_bar);
        }

        public Button RecheckButton { get; private set; }
        public Button LogoutButton { get; private set; }
        public TextView TermsTextView { get; private set; }
        public TextView MessageTextView { get; private set; }
        public ProgressBar ProgressBar { get; private set; }

        bool recheckRunning;
        public bool RecheckRunning
        {
            get { return recheckRunning; }
            set
            {
                recheckRunning = value;
                RefreshProgressBar();
            }
        }

        void RefreshProgressBar()
        {
            var show = recheckRunning;
            ProgressBar.Visibility = show ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}
