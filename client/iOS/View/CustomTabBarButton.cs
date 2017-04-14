using System;
using CoreGraphics;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View
{
	public class CustomTabBarButton : CustomButton
	{
		[View(0)]
		public UIView SeparatorView { get; set; }

		public CGSize ImageViewSize { get; set; }
		public SeparatorGravityType SeparatorGravity { get; set; }
		public float VerticalOffset { get; set; }
		public UIEdgeInsets TitleLabelMargin { get; set; }
		public bool EnableSeparator {
			get { return !SeparatorView.Hidden; }
			set { SeparatorView.Hidden = !value; }
		}

		public CustomTabBarButton(IntPtr handle) : base (handle)
		{
			Initialize();
		}

		public CustomTabBarButton()
		{
			Initialize();
		}

		void Initialize()
		{
			// Default settings
			ImageViewSize = new CGSize(24f, 24f);
			EnableSeparator = false;
			SeparatorGravity = SeparatorGravityType.Right;
			SeparatorView.BackgroundColor = new UIColor(0.33f, 0.33f, 0.33f, 1.0f);
			TitleLabelMargin = new UIEdgeInsets(5, 5, 0, 5);
			VerticalOffset = 0.0f;
			BackgroundColor = UIColor.Clear;
			TitleLabel.TextAlignment = UITextAlignment.Center;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			var pW = this.Bounds.Width;
			var pH = this.Bounds.Height;

			TitleLabel.SizeToFit();

			if (EnableSeparator) {
				if (SeparatorGravity == SeparatorGravityType.Left)
				{
					SeparatorView.Frame = this.LayoutBox()
						.Width(1)
						.Left(0)
						.Top(0)
						.Bottom(0);
				}
				else {
					SeparatorView.Frame = this.LayoutBox()
						.Width(1)
						.Right(0)
						.Top(0)
						.Bottom(0);				
				}
			}

			var contentH = ImageViewSize.Height + TitleLabelMargin.Top + TitleLabelMargin.Bottom + TitleLabel.Bounds.Height;
			var topOffset = (pH - contentH) / 2.0f + VerticalOffset;

			ImageView.Frame = this.LayoutBox()
				.Width(ImageViewSize.Width)
				.Height(ImageViewSize.Height)
				.Top(topOffset)
				.CenterHorizontally();

			TitleLabel.Frame = this.LayoutBox()
				.Height(TitleLabel.Bounds.Height)
				.Below(ImageView, TitleLabelMargin.Top)
				.Left(TitleLabelMargin.Left)
				.Right(TitleLabelMargin.Right);
		}

		public enum SeparatorGravityType { 
			Left, Right
		}
	}
}

