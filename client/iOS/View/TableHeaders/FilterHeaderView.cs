using System;
using UIKit;
using StudioMobile;
using LiveOakApp.iOS.View.Skin;

namespace LiveOakApp.iOS.View.TableHeaders
{
	public class FilterHeaderView : CustomView
	{
		[View]
		public UILabel TitleLabel { get; private set; }

		protected override void CreateView()
		{
			base.CreateView();
			TitleLabel.Font = Fonts.xLargeSemibold;
			TitleLabel.TextColor = Colors.DarkGray;
            BackgroundColor = Colors.DefaultTableViewBackgroundColor;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			TitleLabel.SizeToFit();
			TitleLabel.Frame = this.LayoutBox()
				.Height(TitleLabel.Bounds.Height)
				.Left(Bounds.Width * 0.03862f)
				.Right(5.0f)
				.Top(Bounds.Height * 0.3636f);
		}
	}
}
