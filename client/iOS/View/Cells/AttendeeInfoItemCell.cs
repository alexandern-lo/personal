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
    public class AttendeeInfoItemCell : CustomTableViewCell
    {
        public const string DefaultCellIdentifier = "AttendeeInfoItemCell";

        [View(6)]
        public KeyValueInfoView ItemInfoView { get; private set; }


        public AttendeeInfoItemCell(String cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = UIColor.Clear;
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void SetupCell(AttendeeInfoItemViewModel infoItem)
        {
            ItemInfoView.KeyLabel.Text = infoItem.Key;
            ItemInfoView.ValueLabel.Text = infoItem.Value;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            ItemInfoView.Frame = this.LayoutBox()
                .Top(0)
                .Bottom(0)
                .Left(10)
                .Right(0);
        }

        public static float RowHeight
        {
            get { return 50.0f; }
        }
    }
}

