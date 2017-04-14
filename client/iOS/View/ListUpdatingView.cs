using System;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View
{
    public class ListUpdatingView : CustomView
    {
        [View(0)]
        public UIActivityIndicatorView ActivityIndicator { get; private set; }

        [View(1)]
        public UILabel TextLabel { get; private set; }

        protected override void CreateView()
        {
            base.CreateView();
            BackgroundColor = Colors.RefreshViewBackgroundColor;
            ActivityIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.White;
            ActivityIndicator.HidesWhenStopped = false;
            ActivityIndicator.StartAnimating();
            TextLabel.TextColor = UIColor.White;
            TextLabel.Font = Fonts.SmallSemibold;
            TextLabel.Text = L10n.Localize("UpdatingList", "Updating the list");
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            TextLabel.SizeToFit();

            TextLabel.Frame = this.LayoutBox()
                .CenterVertically()
                .CenterHorizontally()
                .Width(TextLabel.Bounds.Width)
                .Height(TextLabel.Bounds.Height);

            ActivityIndicator.Frame = this.LayoutBox()
                .Before(TextLabel, 12)
                .CenterVertically()
                .Width(16)
                .Height(16);
        }
    }
}

