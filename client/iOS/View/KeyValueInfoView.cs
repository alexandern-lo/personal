using System;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS
{
	public class KeyValueInfoView : CustomView
	{
		[View]
		public UILabel KeyLabel { get; private set; }

		[View]
		public UILabel ValueLabel { get; private set; }

		[View]
		public UIView SeparatorView { get; private set; }

		public bool EnableSeparator {
			get { return !SeparatorView.Hidden; }
			set { SeparatorView.Hidden = !value; }
		}

		protected override void CreateView()
		{
			base.CreateView();
			KeyLabel.Font = Fonts.xSmallRegular;
			ValueLabel.Font = Fonts.SmallRegular;
			SeparatorView.BackgroundColor = Colors.GraySeparatorColor;
			KeyLabel.TextColor = new UIColor(0.604f, 0.604f, 0.604f, 1.0f);
			ValueLabel.TextColor = Colors.DarkGray;
		}

        public void MakeValid(bool valid)
        {
            if (valid)
                SeparatorView.BackgroundColor = Colors.GraySeparatorColor;
            else
                SeparatorView.BackgroundColor = Colors.Red;
        }

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			var pH = this.Bounds.Height;
			var pW = this.Bounds.Width;

			KeyLabel.SizeToFit();
			ValueLabel.SizeToFit();

			if (EnableSeparator) {
				SeparatorView.Frame = this.LayoutBox()
					.Height(1)
					.Left(0)
					.Right(0)
					.Bottom(0);
			}

			KeyLabel.Frame = this.LayoutBox()
				.Height(KeyLabel.Bounds.Height)
				.Top(pH * 0.11f)
				.Left(0)
				.Right(5.0f);
			ValueLabel.Frame = this.LayoutBox()
				.Height(ValueLabel.Bounds.Height)
				.Top(pH * 0.46f)
				.Left(0)
				.Right(5.0f);
		}
	}
}

