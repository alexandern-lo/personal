using System;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View
{
    public class CustomMessageView : CustomView
    {
        [View(1)]
        [CommonSkin("MessageLabel")]
        public UILabel MessageLabel { get; private set; }

        protected override void CreateView()
        {
            base.CreateView();

            MessageLabel.LineBreakMode = UILineBreakMode.WordWrap;
            MessageLabel.TextAlignment = UITextAlignment.Center;
            MessageLabel.Lines = 0;

            this.BackgroundColor = Colors.ErrorBackgroundColorWhite;
            this.Layer.CornerRadius = 5.0f;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            MessageLabel.SizeToFit();


            MessageLabel.Frame = this.LayoutBox()
                .Width(this.Bounds.Width * 0.9f)
                .Height(MessageLabel.Bounds.Height)
                .CenterHorizontally()
                .CenterVertically();
        }

        public override void SizeToFit()
        {
            base.SizeToFit();
            MessageLabel.SizeToFit();

            var frame = this.Frame;
            frame.Height = MessageLabel.Bounds.Height + 16;
            this.Frame = frame;
        }

    }
}
