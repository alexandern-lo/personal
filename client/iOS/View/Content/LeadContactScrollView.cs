using System;
using CoreGraphics;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Content
{
    public class LeadContactScrollView : CustomView
    {
        [View(0)]
        public UIScrollView ContentScrollView { get; private set; }
        public LeadContactView LeadContactView { get; private set; }
        KeyboardScroller scroller;

        protected override void CreateView()
        {
            base.CreateView();
            LeadContactView = new LeadContactView();
            ContentScrollView.AddSubview(LeadContactView);

            scroller = new KeyboardScroller()
            {
                ScrollView = ContentScrollView
            };
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            ContentScrollView.Frame = this.LayoutBox()
                .Top(0)
                .Left(0)
                .Right(0)
                .Bottom(0);

            CGSize leadContactViewSize = LeadContactView.SizeThatFits(new CGSize(Bounds.Width, 10000));
            LeadContactView.Frame = new CGRect(new CGPoint(0, 0), leadContactViewSize);
            ContentScrollView.ContentSize = leadContactViewSize;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                scroller.Dispose();
            }
        }
    }
}
