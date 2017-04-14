using System;
using System.Globalization;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models;
using LiveOakApp.Models.ViewModels;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Cells
{
    public class InboxItemCell : CustomTableViewCell
    {
        public const string DefaultCellIdentifier = "InboxItemCell";
        [View(0)]
        public UIImageView TypeImageView { get; private set; }

        [View(1)]
        [LabelSkin("NormalSemiboldBlackLabel")]
        public UILabel TitleLabel { get; private set; }

        [View(2)]
        [LabelSkin("NormalSemiboldGrayLabel")]
        public UILabel MessageLabel { get; private set; }

        [View(3)]
        [LabelSkin("xSmallRegularBlackLabel")]
        public UILabel ResivedTimeLabel { get; private set; }


        public InboxItemCell(string cellId) : base(UITableViewCellStyle.Default, cellId)
        {
        }

        public void SetupCell(InboxItemViewModel inboxItem)
        {
            TypeImageView.Image = UIImage.FromBundle(inboxItem.TypeImageName);
            if (inboxItem.IsRead)
            {
                LabelSkin.NormalRegularBlackLabel(TitleLabel);
                LabelSkin.NormalRegularGrayLabel(MessageLabel);
            }
            else {
                LabelSkin.NormalSemiboldBlackLabel(TitleLabel);
                LabelSkin.NormalSemiboldGrayLabel(MessageLabel);
            }
            TitleLabel.Text = inboxItem.Title;
            MessageLabel.Text = inboxItem.Message;
            if (inboxItem.ReceivedTime.Date == DateTime.Today.Date)
                ResivedTimeLabel.Text = ServiceLocator.Instance.DateTimeService.TimeToDisplayString(inboxItem.ReceivedTime);            
            else 
                ResivedTimeLabel.Text = ServiceLocator.Instance.DateTimeService.DateTimeToDisplayString(inboxItem.ReceivedTime);
            
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            TypeImageView.SizeToFit();
            TitleLabel.SizeToFit();
            MessageLabel.SizeToFit();
            ResivedTimeLabel.SizeToFit();

            TypeImageView.Frame = this.LayoutBox()
                .Left(15)
                .Top(11)
                .Width(TypeImageView.Bounds.Width)
                .Height(TypeImageView.Bounds.Height);

            ResivedTimeLabel.Frame = this.LayoutBox()
                .Right(15)
                .CenterVertically()
                .Width(ResivedTimeLabel.Bounds.Width)
                .Height(ResivedTimeLabel.Bounds.Height);

            TitleLabel.Frame = this.LayoutBox()
                .After(TypeImageView, 10)
                .CenterVertically(TypeImageView)
                .Before(ResivedTimeLabel, 7)
                .Height(TitleLabel.Bounds.Height);

            MessageLabel.Frame = this.LayoutBox()
                .After(TypeImageView, 10)
                .Below(TitleLabel, 0)
                .Before(ResivedTimeLabel, 0)
                .Height(MessageLabel.Bounds.Height);
        }

        public static float CellHeight() { return 60.0f; }
    }
}
