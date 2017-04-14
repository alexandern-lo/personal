using System;
using LiveOakApp.iOS.View.Skin;
using StudioMobile.FontAwesome;
using StudioMobile;
using UIKit;
using Foundation;

namespace LiveOakApp.iOS.View.Cells
{
    public class EventCell : CustomTableViewCell
    {
        public const string DefaultCellIdentifier = "EventCell";

        [View]
        [LabelSkin("NormalSemiboldBlackLabel")]
        public UILabel TitleLabel { get; private set; }

        [View]
        [LabelSkin("xSmallRegularBlackLabel")]
        public UILabel TimeLabel { get; private set; }

        [View]
        [LabelSkin("xSmallRegularBlackLabel")]
        public UILabel LocationLabel { get; private set; }

        [View]
        [CommonSkin("TimeIconImageView")]
        public UIImageView TimeIconImageView { get; private set; }

        [View]
        [CommonSkin("LocationIconImageView")]
        public UIImageView LocationIconImageView { get; private set; }

        public EventCell(string cellId = EventCell.DefaultCellIdentifier) : base(UIKit.UITableViewCellStyle.Default, cellId)
        {
            LocationLabel.LineBreakMode = UILineBreakMode.TailTruncation | UILineBreakMode.WordWrap;
            LocationLabel.Lines = 2;
        }

        public void SetupCell(string title, string time, string location, int section)
        {
            TitleLabel.Text = title;
            TimeLabel.Text = time;
            LocationLabel.Text = location;

            if (section == 2)
            {
                BackgroundColor = new UIColor(0.957f, 0.957f, 0.957f, 1f);
            }
        }


        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            LocationLabel.SizeToFit();
            TitleLabel.SizeToFit();
            TimeLabel.SizeToFit();

            TitleLabel.Frame = this.LayoutBox()
                .Height(TitleLabel.Frame.Height)
                .Top(5)
                .Right(5)
                .Left(15);

            TimeIconImageView.Frame = this.LayoutBox()
                .Height(TimeLabel.Frame.Height)
                .Width(15)
                .CenterVertically(-2)
                .Left(18);

            LocationIconImageView.Frame = this.LayoutBox()
                .Height(LocationLabel.Frame.Height)
                .Width(15)
                .Bottom(3)
                .Left(18);

            TimeLabel.Frame = this.LayoutBox()
                .Height(TimeLabel.Frame.Height)
                .CenterVertically(-2)
                .Right(5)
                .After(TimeIconImageView, 8);

            LocationLabel.Frame = this.LayoutBox()
                .Height(LocationLabel.Frame.Height)
                .Bottom(4)
                .Right(5)
                .After(LocationIconImageView, 8);
        }

        public static float RowHeight
        {
            get { return 84.0f; }
        }
    }
}

