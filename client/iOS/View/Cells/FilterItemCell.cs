using System;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Cells
{
	public class FilterItemCell : CustomTableViewCell
	{
		public const string DefaultCellIdentifier = "FilterItemCell";

		[View]
		public UILabel TitleLabel { get; private set; }

		[View]
        [CommonSkin("DefaultSwitch")]
		public UISwitch Switch { get; private set; }

		public FilterItemCell(string cellId = DefaultCellIdentifier) : base(UIKit.UITableViewCellStyle.Default, cellId)
		{
			TitleLabel.Font = Fonts.NormalRegular;
			TitleLabel.TextColor = Colors.DarkGray;
            SelectionStyle = UITableViewCellSelectionStyle.None;
		}

		public void SetupCell(string title, bool state)
		{
			TitleLabel.Text = title;
			Switch.On = state;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			var pH = Bounds.Height;
			var pW = Bounds.Width;

			TitleLabel.SizeToFit();
			Switch.SizeToFit();

			var leftAndRightMargin = pW * 0.03862f;

			Switch.Frame = this.LayoutBox()
				.Height(Switch.Bounds.Height)
				.Width(Switch.Bounds.Width)
				.Right(leftAndRightMargin)
				.CenterVertically();

			TitleLabel.Frame = this.LayoutBox()
				.Height(TitleLabel.Bounds.Height)
				.Before(Switch, 5.0f)
				.Left(leftAndRightMargin)
				.CenterVertically();
		}

        public static nfloat RowHeight
        { 
            get { return  50f; }
        }
	}
}

