using System;
using LiveOakApp.iOS.View.Skin;
using Foundation;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Cells
{
    public class AgendaItemCell : CustomTableViewCell
    {
        public const string DefaultCellIdentifier = "AgendaItemCell";

        [View]
        [LabelSkin("AgendaItemTitleLabel")]
        public UILabel TitleLabel { get; private set; }

        [View]
        [LabelSkin("AgendaItemDescriptionLabel")]
        public UILabel DescriptionLabel { get; private set; }

        [View]
        [LabelSkin("AgendaItemTimeLabel")]
        public UILabel TimeLabel { get; private set; }

        public AgendaItemCell(string cellId = DefaultCellIdentifier) : base(UIKit.UITableViewCellStyle.Default, cellId)
        {
        }

        public void SetupCell(string title, string description, string time)
        {
            TitleLabel.Text = title;
            TimeLabel.Text = time;
            if (description != null)
            {
                NSMutableAttributedString attrDescription = new NSMutableAttributedString(description);
                NSMutableParagraphStyle style = new NSMutableParagraphStyle();
                style.LineHeightMultiple = 0.92f;
                attrDescription.AddAttribute(UIStringAttributeKey.ParagraphStyle, style, new NSRange(0, description.Length));
                DescriptionLabel.AttributedText = attrDescription;
            }
        }


        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            TitleLabel.SizeToFit();
            DescriptionLabel.SizeToFit();
            TimeLabel.SizeToFit();

            TitleLabel.Frame = this.LayoutBox()
                .Height(TitleLabel.Frame.Height)
                .Top(8)
                .Right(5)
                .Left(15);

            TimeLabel.Frame = this.LayoutBox()
                .Height(TimeLabel.Frame.Height)
                .Bottom(6)
                .Right(5)
                .Left(15);

            DescriptionLabel.Frame = this.LayoutBox()
                .Below(TitleLabel, 0)
                .Above(TimeLabel, 0)
                .Right(5)
                .Left(15);
        }

        public static float RowHeight
        {
            get { return 100.0f; }
        }
    }
}
