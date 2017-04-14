using System;
using UIKit;
using StudioMobile;
using LiveOakApp.iOS.View.Skin;

namespace LiveOakApp.iOS.View
{
    public class KeyValueSelectButton : UIButton
    {
        public KeyValueInfoView KeyValueInfoView { get; private set; } = new KeyValueInfoView();
        public UIImageView ArrowImageView { get; private set; } = new UIImageView();

        public KeyValueSelectButton(): base()
        {
            KeyValueInfoView.ValueLabel.Font = Fonts.LargeRegular;
            ArrowImageView.Image = UIImage.FromBundle("arrow_right");
            KeyValueInfoView.UserInteractionEnabled = false;
            ArrowImageView.UserInteractionEnabled = false;
            AddSubview(KeyValueInfoView);
            AddSubview(ArrowImageView);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            KeyValueInfoView.Frame = this.LayoutBox()
                .Top(0)
                .Left(0)
                .Right(0)
                .Bottom(0);
            
            ArrowImageView.SizeToFit();

            ArrowImageView.Frame = KeyValueInfoView.LayoutBox()
                .Height(ArrowImageView.Bounds.Height)
                .Width(ArrowImageView.Bounds.Width)
                .Top(KeyValueInfoView.Bounds.Height * 0.46f)
                .Right(10.0f);
        }
    }
}

