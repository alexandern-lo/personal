using UIKit;
using StudioMobile;
using LiveOakApp.iOS.View.Skin;

namespace LiveOakApp.iOS.View.Content
{
    public class LeadDetailsView : CustomView
    {

        [View(1)]
        [CommonSkin("topBarContactButton")]
        public CustomTopBarButton TopBarContactButton { get; private set; }

        [View(2)]
        [CommonSkin("topBarQualifyButton")]
        public CustomTopBarButton TopBarQualifyButton { get; private set; }

        [View(3)]
        public UIView ChildViewContainer { get; private set; }
        public UIView ChildView { get; private set; }

        public void SetChildView(UIView childView)
        {
            ChildView = childView;
            ChildViewContainer.AddSubview(childView);
            SetNeedsLayout();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            TopBarContactButton.Frame = this.LayoutBox()
                .Top(0)
                .Left(0)
                .Width(Bounds.Width / 2)
                .Height(40);

            TopBarQualifyButton.Frame = this.LayoutBox()
                .Top(0)
                .Right(0)
                .Width(Bounds.Width / 2)
                .Height(40);

            ChildViewContainer.Frame = this.LayoutBox()
                .Below(TopBarContactButton, 0)
                .Left(0)
                .Right(0)
                .Bottom(0);
            ChildView.Frame = ChildViewContainer.Bounds;
        }

    }
}
