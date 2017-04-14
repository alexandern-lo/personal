using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using LiveOakApp.Resources;
using Android.Text;
using Android.Text.Method;
using Android.Net;
using Android.App;

namespace LiveOakApp.Droid.Views
{
    public class WrongUserRoleView : FrameLayout
    {
        public WrongUserRoleView(Context context) :
            base(context)
        {
            Initialize();
        }

        public WrongUserRoleView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public WrongUserRoleView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.WrongUserRoleLayout, this);
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            SetBackgroundResource(Resource.Drawable.login_bg);

            MessageTextView = FindViewById<TextView>(Resource.Id.message_textview);
            WebsiteButton = FindViewById<Button>(Resource.Id.website_button);
            LogoutButton = FindViewById<Button>(Resource.Id.logout_button);

            MessageTextView.MovementMethod = LinkMovementMethod.Instance;
            MessageTextView.TextFormatted = Html.FromHtml(L10n.Localize("AdminUserRoleMessage", "Admin panel is not availble in mobile app. Please, proceed to the website to use it"));
        }

        public Button WebsiteButton { get; private set; }
        public Button LogoutButton { get; private set; }
        public TextView TermsTextView { get; private set; }
        public TextView MessageTextView { get; private set; }

    }
}
