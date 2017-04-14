using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace LiveOakApp.Droid.Views
{
    public class LoginView : FrameLayout
    {
        public LoginView(Context context) : base(context)
        {
            Initialize();
        }

        public LoginView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public LoginView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.LoginView, this);
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            SetBackgroundResource(Resource.Drawable.login_bg);

            LoginButton = FindViewById<Button>(Resource.Id.login_button);
            ErrorTextView = FindViewById<TextView>(Resource.Id.error_text);
            ProgressBar = FindViewById<ProgressBar>(Resource.Id.login_progress_bar);
            ResetPasswordErrorTextView = FindViewById<TextView>(Resource.Id.reset_password_error_text_view);
            ResetPasswordTextView = FindViewById<TextView>(Resource.Id.restore_password_text_view);

            ResetPasswordTextView.Visibility = ViewStates.Gone; // temporarily hidden
            ResetPasswordErrorTextView.Visibility = ViewStates.Gone;
        }

        public TextView ErrorTextView { get; private set; }
        public ProgressBar ProgressBar { get; private set; }
        public Button LoginButton { get; private set; }
        public TextView ResetPasswordErrorTextView { get; private set; }
        public TextView ResetPasswordTextView { get; private set; }

        bool loginRunning;
        public bool LoginRunning
        {
            get { return loginRunning; }
            set
            {
                loginRunning = value;
                RefreshProgressBar();
            }
        }

        bool resetPasswordRunning;
        public bool ResetPasswordRunning
        {
            get { return resetPasswordRunning; }
            set
            {
                resetPasswordRunning = value;
                RefreshProgressBar();
            }
        }

        void RefreshProgressBar()
        {
            var showActivity = loginRunning || resetPasswordRunning;
            ProgressBar.Visibility = showActivity ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}
