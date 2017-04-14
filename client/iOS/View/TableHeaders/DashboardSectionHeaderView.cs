using System;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.TableHeaders
{
    public class DashboardSectionHeaderView : CustomView
    {
        [View(1)]
        [LabelSkin("xSmallRegularGrayLabel")]
        public UILabel LeftLabel { get; private set; }

        [View(2)]
        [LabelSkin("xSmallRegularGrayLabel")]
        public UILabel RightLabel { get; private set; }

        [View(3)]
        public UIView BottomSeparatorView { get; private set; }

        public DashboardSectionHeaderView(string leftLabelText, string rightLabelText)
        {
            LeftLabel.Text = leftLabelText;
            RightLabel.Text = rightLabelText;
        }

        protected override void CreateView()
        {
            base.CreateView();
            BottomSeparatorView.BackgroundColor = Colors.GraySeparatorColor;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            LeftLabel.SizeToFit();
            RightLabel.SizeToFit();

            LeftLabel.Frame = this.LayoutBox()
                .Left(15)
                .CenterVertically()
                .Width(LeftLabel.Bounds.Width)
                .Height(LeftLabel.Bounds.Height);

            RightLabel.Frame = this.LayoutBox()
                .Right(15)
                .CenterVertically()
                .Width(RightLabel.Bounds.Width)
                .Height(RightLabel.Bounds.Height);

            BottomSeparatorView.Frame = this.LayoutBox()
                .Bottom(0)
                .Height(1)
                .Left(15)
                .Right(15);
        }

        public static float HeaderHeight { get { return 45f; } }
    }
}
