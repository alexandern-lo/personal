using System;
using CoreGraphics;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View
{
    public class CustomTopBarButton : CustomButton
    {
        [View(0)]
        public UIView SeparatorView { get; set; }

        [View(1)]
        public UIView SelectionSeparatorView { get; set; }

        public CGSize ImageViewSize { get; set; }
        public SeparatorGravityType SeparatorGravity { get; set; }
        public float VerticalOffset { get; set; }
        public UIEdgeInsets TitleLabelMargin { get; set; }
        public bool EnableSeparator
        {
            get { return !SeparatorView.Hidden; }
            set { SeparatorView.Hidden = !value; }
        }

        public CustomTopBarButton(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public CustomTopBarButton()
        {
            Initialize();
        }

        void Initialize()
        {
            // Default settings
            ImageViewSize = new CGSize(24f, 24f);
            EnableSeparator = false;
            SeparatorGravity = SeparatorGravityType.Right;
            SeparatorView.BackgroundColor = new UIColor(0.33f, 0.33f, 0.33f, 1.0f);
            SelectionSeparatorView.BackgroundColor = new UIColor(0.365f, 0.624f, 0.988f, 1.0f);
            SelectionSeparatorView.Hidden = true;
            TitleLabelMargin = new UIEdgeInsets(0, 5, 0, 5);
            VerticalOffset = 0.0f;
            BackgroundColor = UIColor.Clear;
            TitleLabel.TextAlignment = UITextAlignment.Center;
        }

        public virtual void SetActive(bool active)
        {
            SelectionSeparatorView.Hidden = !active;
            if (active)
            {
                SetTitleColor(new UIColor(0.365f, 0.624f, 0.988f, 1.0f), UIControlState.Normal);
                TitleLabel.Font = Fonts.LargeSemibold;
            }
            else {
                SetTitleColor(UIColor.White, UIControlState.Normal);
                TitleLabel.Font = Fonts.LargeRegular;
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var pW = this.Bounds.Width;
            var pH = this.Bounds.Height;

            TitleLabel.SizeToFit();

            if (EnableSeparator)
            {
                if (SeparatorGravity == SeparatorGravityType.Left)
                {
                    SeparatorView.Frame = this.LayoutBox()
                        .Width(1)
                        .Left(0)
                        .Height(pH / 2)
                        .CenterVertically();
                }
                else {
                    SeparatorView.Frame = this.LayoutBox()
                        .Width(1)
                        .Right(0)
                        .Height(pH / 2)
                        .CenterVertically();
                }
            }

            var contentH = ImageViewSize.Height + TitleLabelMargin.Top + TitleLabelMargin.Bottom + TitleLabel.Bounds.Height;
            var topOffset = (pH - contentH) / 2.0f + VerticalOffset;

            SelectionSeparatorView.Frame = this.LayoutBox()
                .Width(pW)
                .Height(4)
                .Bottom(0)
                .CenterHorizontally();
                                  
            ImageView.Frame = this.LayoutBox()
                .Width(ImageViewSize.Width)
                .Height(ImageViewSize.Height)
                .Top(topOffset)
                .CenterHorizontally();

            TitleLabel.Frame = this.LayoutBox()
                .Height(TitleLabel.Bounds.Height)
                .Below(ImageView, TitleLabelMargin.Top)
                .Left(TitleLabelMargin.Left)
                .Right(TitleLabelMargin.Right);
        }

        public enum SeparatorGravityType
        {
            Left, Right
        }
    }
}

