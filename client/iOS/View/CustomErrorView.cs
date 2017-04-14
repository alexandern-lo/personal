using System;
using CoreGraphics;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View
{
    public class CustomErrorView : CustomView
    {
        [View(1)]
        [CommonSkin("ErrorLabel")]
        public UILabel ErrorMessageLabel { get; private set; }

        [View(2)]
        [ButtonSkin("ReloadButton")]
        public UIButton ReloadButton { get; private set; }

        int buttonHeight = 40;

        protected override void CreateView()
        {
            base.CreateView();
            ErrorMessageLabel.LineBreakMode = UILineBreakMode.WordWrap;
            ErrorMessageLabel.Lines = 0;
            ErrorMessageLabel.TextAlignment = UITextAlignment.Center;

            this.BackgroundColor = Colors.ErrorBackgroundColorWhite;
            this.Layer.CornerRadius = 5.0f;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            ErrorMessageLabel.SizeToFit();


            ErrorMessageLabel.Frame = this.LayoutBox()
                .Width(this.Bounds.Width*0.9f)
                .Height(ErrorMessageLabel.Bounds.Height)
                .CenterHorizontally()
                .Top(7);
            ReloadButton.Frame = this.LayoutBox()
                .Left(50)
                .Right(50)
                .Height(buttonHeight)
                .Below(ErrorMessageLabel, 7);
        }

        public override void SizeToFit()
        {
            base.SizeToFit();
            ErrorMessageLabel.SizeToFit();

            var frame = this.Frame;
            frame.Height = ErrorMessageLabel.Bounds.Height + buttonHeight + 21;
            this.Frame = frame;
        }
    }
}
