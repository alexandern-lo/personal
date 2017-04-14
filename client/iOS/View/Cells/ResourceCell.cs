using System;
using LiveOakApp.iOS.View.Skin;
using StudioMobile.FontAwesome;
using StudioMobile;
using UIKit;
using Foundation;
using LiveOakApp.Models.Data;
using LiveOakApp.Models.ViewModels;

namespace LiveOakApp.iOS.View.Cells
{
    public class ResourceCell : CustomTableViewCell
    {
        public const string DefaultCellIdentifier = "ResourceCell";

        [View(1)]
        public UIImageView TypeImageView { get; private set; }

        [View(2)]
        public UIButton CheckBoxButton { get; private set; }

        [View(3)]
        [LabelSkin("xSmallRegularBlackLabel")]
        public UILabel TitleLabel { get; private set; }

        [View(4)]
        [LabelSkin("xSmallRegularGrayLabel")]
        public UILabel DescriptionLabel { get; private set; }

        public ResourceViewModel Resource { get; private set; }

        public ResourceCell(UITableView tableView, Action<ResourceCell> checkBoxClicked, String cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = UIColor.Clear;
            SelectionStyle = UITableViewCellSelectionStyle.None;
            CheckBoxButton.ImageEdgeInsets = new UIEdgeInsets(2.5f, 30, 2.5f, 9);
            CheckBoxButton.SetImage(UIImage.FromBundle("resorces_checkbox"), UIControlState.Normal);
            CheckBoxButton.TouchUpInside += (sender, e) => checkBoxClicked(this);
        }

        public void SetupCell(ResourceViewModel resource)
        {
            Resource = resource;
            TitleLabel.Text = resource.Title;
            DescriptionLabel.Text = resource.Description;
            TypeImageView.Image = UIImage.FromBundle(resource.ResourceTypeImageName);
        }

        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);
            if (selected)
            {
                CheckBoxButton.SetImage(UIImage.FromBundle("resorces_checkbox_active"), UIControlState.Normal);
            }
            else {
                CheckBoxButton.SetImage(UIImage.FromBundle("resorces_checkbox"), UIControlState.Normal);
            }
        }


        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            TitleLabel.SizeToFit();
            DescriptionLabel.SizeToFit();
            CheckBoxButton.SizeToFit();
            TypeImageView.SizeToFit();

            TitleLabel.Frame = this.LayoutBox()
                .Height(TitleLabel.Frame.Height)
                .Top(8)
                .Right(55)
                .Left(55);

            DescriptionLabel.Frame = this.LayoutBox()
                .Height(TitleLabel.Frame.Height)
                .Below(TitleLabel, -3)
                .Right(55)
                .Left(55);

            TypeImageView.Frame = this.LayoutBox()
                .Height(30)
                .Width(24)
                .CenterVertically()
                .Left(18);

            CheckBoxButton.Frame = this.LayoutBox()
                .Height(49)
                .Width(83)
                .CenterVertically()
                .Right(0);
        }

        public static float RowHeight
        {
            get { return 50.0f; }
        }
    }
}

