using System;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.TableHeaders
{
    public class AgendaHeaderView : CustomView
    {
        [View(0)]
        public UIImageView LocationImageView { get; private set; }

        [View(1)]
        [LabelSkin("AgendaLocationLabel")]
        public UILabel LocationLabel { get; private set; }

        protected override void CreateView()
        {
            base.CreateView();
            LocationImageView.Image = UIImage.FromBundle("edetails_location");
            this.BackgroundColor = Colors.DefaultTableViewBackgroundColor;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            LocationImageView.SizeToFit();
            LocationLabel.SizeToFit();
            LocationLabel.PreferredMaxLayoutWidth = this.Bounds.Width - 60;

            LocationLabel.Frame = this.LayoutBox()
                .CenterVertically()
                .Right(17)
                .Width(LocationLabel.Bounds.Width)
                .Height(LocationLabel.Bounds.Height);

            LocationImageView.Frame = this.LayoutBox()
                .CenterVertically()
                .Before(LocationLabel, 10)
                .Width(LocationImageView.Bounds.Width)
                .Height(LocationImageView.Bounds.Height);
        }

        public static float HeaderHeight
        {
            get
            {
                return 35f;
            }
        }
    }
}
