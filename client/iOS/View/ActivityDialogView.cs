using System;
using LiveOakApp.iOS.View;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View
{
	public class ActivityDialogView : CustomView
	{
		[View(0)]
		public UIView DialogView { get; private set; }

		[View(1)]
		public UIActivityIndicatorView ActivityIndicator { get; private set; }

		[View(2)]
		[LabelSkin("SmallLabel")]
		public UILabel TitleLabel { get; private set; }

		protected override void CreateView()
		{
			base.CreateView();
			BackgroundColor = UIColor.Black.ColorWithAlpha(0.2f);
			DialogView.Layer.CornerRadius = 20;
			DialogView.Layer.MasksToBounds = true;
			DialogView.BackgroundColor = new UIColor(0.5f, 0.5f, 0.5f, 1.0f);

			ActivityIndicator.StartAnimating();
			TitleLabel.TextAlignment = UITextAlignment.Center;
			TitleLabel.TextColor = UIColor.White;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			var size = this.Bounds.Height > this.Bounds.Width ? this.Bounds.Width / 3 : this.Bounds.Height / 3;

			DialogView.Frame = this.LayoutBox()
				.Height(size)
				.Width(size)
				.CenterVertically()
				.CenterHorizontally();

			ActivityIndicator.SizeToFit();
			ActivityIndicator.Frame = this.LayoutBox()
				.Height(ActivityIndicator.Frame.Height)
				.Width(ActivityIndicator.Frame.Width)
				.CenterVertically()
				.CenterHorizontally();

			TitleLabel.SizeToFit();
			TitleLabel.Frame = this.LayoutBox()
				.Height(TitleLabel.Frame.Height)
				.Width(size - 6)
				.CenterHorizontally()
				.Below(ActivityIndicator, 5);
		}
	}
}

