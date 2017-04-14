using System;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;
using Foundation;

namespace LiveOakApp.iOS.View.Content
{
    public class SupportView : CustomView
    {
        [View]
        public UIWebView SupportWebView { get; private set; }

        public NSUrl webViewURL;
        protected override void CreateView()
        {
            base.CreateView();

            webViewURL = new NSUrl("http://www.liveoakinc.com/support");
            NSUrlRequest webViewRequest = new NSUrlRequest(webViewURL);
            SupportWebView.LoadRequest(webViewRequest);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            SupportWebView.Frame = this.LayoutBox()
                .Width(Bounds.Width)
                .Height(Bounds.Height)
                .Left(0)
                .Top(0);
        }
    }
}

