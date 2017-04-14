using System;
using CoreGraphics;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View
{
    public class LeadQualifyStateView : CustomView
    {
        [View(0)]
        public UIImageView ImageView { get; private set; }

        [View(1)]
        public UILabel TitleLabel { get; private set; }

        CGColor selectedShadowColor;
        public static LeadQualifyStateView LeadQualityColdStateView()
        {
            var result = new LeadQualifyStateView();
            result.TitleLabel.Text = L10n.Localize("QualifyStateCold", "Cold");
            result.ImageView.Image = UIImage.FromBundle("lead_cold");
            result.selectedShadowColor = Colors.MainBlueColor.CGColor;
            return result;
        }

        public static LeadQualifyStateView LeadQualityWarmStateView()
        {
            var result = new LeadQualifyStateView();
            result.TitleLabel.Text = L10n.Localize("QualifyStateWarm", "Warm");
            result.ImageView.Image = UIImage.FromBundle("lead_warm");
            result.selectedShadowColor = UIColor.Orange.CGColor;
            return result;
        }

        public static LeadQualifyStateView LeadQualityHotStateView()
        {
            var result = new LeadQualifyStateView();
            result.TitleLabel.Text = L10n.Localize("QualifyStateHot", "Hot");
            result.ImageView.Image = UIImage.FromBundle("lead_hot");
            result.selectedShadowColor = UIColor.Red.CGColor;
            return result;
        }

        public void SetSelected(bool selected)
        {
            if (selected)
            {
                Layer.ShadowRadius = 3f;
                Layer.ShadowOpacity = 1f;
                Layer.ShadowOffset = new CGSize(0, 0);
                Layer.ShadowColor = selectedShadowColor;
            }
            else {
                Layer.ShadowRadius = 0.5f;
                Layer.ShadowOpacity = 0.2f;
                Layer.ShadowOffset = new CGSize(0, 1);
                Layer.ShadowColor = Colors.DarkGray.CGColor;
            }
        }

        protected override void CreateView()
        {
            base.CreateView();
            BackgroundColor = UIColor.White;
            Layer.CornerRadius = 6;
            Layer.ShadowColor = Colors.DarkGray.CGColor;
            Layer.ShadowOffset = new CGSize(0, 1);
            Layer.ShadowRadius = 0.5f;
            Layer.ShadowOpacity = 0.2f;
            TitleLabel.Font = Fonts.xLargeSemibold;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            ImageView.SizeToFit();
            TitleLabel.SizeToFit();

            ImageView.Frame = this.LayoutBox()
                .CenterVertically()
                .CenterHorizontally(-ImageView.Bounds.Width - 7)
                .Width(ImageView.Bounds.Width)
                .Height(ImageView.Bounds.Height);

            TitleLabel.Frame = this.LayoutBox()
                .CenterVertically()
                .After(ImageView, 10)
                .Width(TitleLabel.Bounds.Width)
                .Height(TitleLabel.Bounds.Height);
        }
    }
}

