using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using LiveOakApp.Resources;

namespace LiveOakApp.Droid.Views
{
    public class TermsOfUseView : FrameLayout
    {
        public TermsOfUseView(Context context) : base(context)
        {
            Initialize();
        }

        public TermsOfUseView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public TermsOfUseView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.TermsOfUseView, this);
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            SetBackgroundResource(Resource.Drawable.login_bg);

            TermsTextView = FindViewById<TextView>(Resource.Id.terms_textview);
            ErrorTextView = FindViewById<TextView>(Resource.Id.error_textview);
            AcceptButton = FindViewById<Button>(Resource.Id.accept_terms_button);
            DeclineButton = FindViewById<Button>(Resource.Id.decline_terms_button);
            ReloadButton = FindViewById<Button>(Resource.Id.reload_terms_button);
            ProgressBar = FindViewById<ProgressBar>(Resource.Id.terms_progress_bar);
        }

        public void ShowErrorMessage()
        {
            ErrorTextView.Visibility = ViewStates.Visible;
        }

        public void ShowErrorButton()
        {
            ReloadButton.Visibility = ViewStates.Visible;
        }

        public void HideError()
        {
            ErrorTextView.Visibility = ViewStates.Invisible;
            ReloadButton.Visibility = ViewStates.Invisible;
        }

        public Button AcceptButton { get; private set; }
        public Button DeclineButton { get; private set; }
        public Button ReloadButton { get; private set; }
        public TextView TermsTextView { get; private set; }
        public TextView ErrorTextView { get; private set; }
        public ProgressBar ProgressBar { get; private set; }

        bool loadTermsRunning;
        public bool LoadTermsRunning
        {
            get { return loadTermsRunning; }
            set
            {
                loadTermsRunning = value;
                RefreshProgressBar();
            }
        }

        bool acceptRunning;
        public bool AcceptRunning
        {
            get { return acceptRunning; }
            set
            {
                acceptRunning = value;
                RefreshProgressBar();
            }
        }

        void RefreshProgressBar()
        {
            var show = loadTermsRunning || acceptRunning;
            ProgressBar.Visibility = show ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}
