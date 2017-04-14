using Android.OS;
using Android.Views;
using Android.Widget;
using LiveOakApp.Resources;
using StudioMobile;
using Android.Webkit;

namespace LiveOakApp.Droid.Controller
{
    public class SupportFragment : CustomFragment
    {

        public static SupportFragment Create()
        {
            var fragment = new SupportFragment();

            fragment.Title = L10n.Localize("MenuSupport", "Support");

            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var webView = new WebView(inflater.Context);

            webView.LoadUrl("http://www.liveoakinc.com/support");

            return webView;
        }
    }
}

