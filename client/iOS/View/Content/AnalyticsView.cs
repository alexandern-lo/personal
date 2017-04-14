using System;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Content
{
	public class AnalyticsView : CustomView
	{
		[View]
		[LabelSkin("LargeLightWhiteLabel")]
		public UILabel ControllerTitleLabel { get; private set; }

		protected override void CreateView()
		{
			base.CreateView();
			ControllerTitleLabel.Text = "Analytics";
			BackgroundColor = new UIColor(0.6f, 0.6f, 0.6f, 1.0f);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			ControllerTitleLabel.SizeToFit();
			ControllerTitleLabel.Frame = this.LayoutBox()
				.Width(ControllerTitleLabel.Frame.Width)
				.Height(ControllerTitleLabel.Frame.Height)
				.CenterHorizontally()
				.CenterVertically();
		}
	}
}

