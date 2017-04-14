using System;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.ViewModels;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Cells
{
    public class DashboardResourceCell : CustomBindingsTableViewCell
    {
        public const string DefaultCellIdentifier = "DashboardResourceCell";

        [View(0)]
        public UIImageView ResourceTypeImageView { get; private set; }

        [View(1)]
        [LabelSkin("xSmallRegularBlackLabel")]
        public UILabel ResourceNameLabel { get; private set; }

        [View(2)]
        public UIImageView SentImageView { get; private set; }

        [View(3)]
        public UILabel SentLabel { get; private set; }

        [View(4)]
        public UIImageView OpenedImageView { get; private set; }

        [View(5)]
        [LabelSkin("LargeBoldBlackLabel")]
        public UILabel OpenedLabel { get; private set; }


        public DashboardResourceCell(string cellId = DashboardResourceCell.DefaultCellIdentifier) : base(UIKit.UITableViewCellStyle.Default, cellId)
        {
            SentImageView.Image = UIImage.FromBundle("icon_paperplane.png");
            OpenedImageView.Image = UIImage.FromBundle("icon_folder.png");
        }

        public void SetupCell(DashboardResourceViewModel dashboardResource)
        {
            ResourceNameLabel.Text = dashboardResource.Name;
            SentLabel.Text = dashboardResource.SentCount.ToString();
            OpenedLabel.Text = dashboardResource.OpenedCount.ToString();
            ResourceTypeImageView.Image = UIImage.FromBundle(dashboardResource.ResourceTypeImageName);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            SentImageView.SizeToFit();
            OpenedImageView.SizeToFit();
            ResourceNameLabel.SizeToFit();
            ResourceTypeImageView.SizeToFit();
            SentLabel.SizeToFit();
            OpenedLabel.SizeToFit();

            ResourceTypeImageView.Frame = this.LayoutBox()
                .Left(20)
                .CenterVertically()
                .Height(30)
                .Width(24);

            OpenedLabel.Frame = this.LayoutBox()
                .Right(15)
                .CenterVertically()
                .Width(OpenedLabel.Bounds.Width)
                .Height(OpenedLabel.Bounds.Height);

            OpenedImageView.Frame = this.LayoutBox()
                .Before(OpenedLabel, 6)
                .CenterVertically()
                .Width(OpenedImageView.Bounds.Width)
                .Height(OpenedImageView.Bounds.Height);

            SentLabel.Frame = this.LayoutBox()
                .Before(OpenedImageView, 15)
                .CenterVertically()
                .Width(SentLabel.Bounds.Width)
                .Height(SentLabel.Bounds.Height);

            SentImageView.Frame = this.LayoutBox()
                .Before(SentLabel, 6)
                .CenterVertically()
                .Width(SentImageView.Bounds.Width)
                .Height(SentImageView.Bounds.Height);

            ResourceNameLabel.Frame = this.LayoutBox()
                .After(ResourceTypeImageView, 8)
                .Before(SentImageView, 2)
                .CenterVertically()
                .Height(ResourceNameLabel.Bounds.Height);
        }
    }
}
