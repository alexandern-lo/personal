using UIKit;
using StudioMobile;
using LiveOakApp.iOS.View.Skin;

namespace LiveOakApp.iOS.View
{
    public class StartupView : CustomView
    {
        [View(0)]
        public UIImageView BackgroundImageView { get; private set; }

        [View(1)]
        public UIActivityIndicatorView ActivityIndicatorView { get; private set; }

        [View(2)]
        [CommonSkin("StartupProgressLabel")]
        public UILabel ProgressLabel { get; private set; }

        protected override void CreateView()
        {
            base.CreateView();
            BackgroundImageView.Image = UIImage.FromBundle("background");
            BackgroundImageView.ContentMode = UIViewContentMode.ScaleAspectFill;

            ActivityIndicatorView.StartAnimating();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var parentHeight = Bounds.Height;
            var parentWidth = Bounds.Width;

            BackgroundImageView.Frame = Bounds;

            ActivityIndicatorView.Frame = this.LayoutBox()
                .Width(44)
                .Height(44)
                .CenterVertically()
                .CenterHorizontally();

            ProgressLabel.Frame = this.LayoutBox()
                .Width(parentWidth)
                .Height(44)
                .Below(ActivityIndicatorView, 10f);
        }
    }
}
