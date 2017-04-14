using System;
using Foundation;
using UIKit;
using System.Drawing;
using CoreGraphics;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;

namespace LiveOakApp.iOS.View.TableHeaders
{
	[Register("SectionHeaderView")]
	public class SectionHeaderView : UIView
	{
		public SectionHeaderView(string title)
		{
			PerformInit(title);
		}
		public SectionHeaderView(IntPtr handle) : base(handle)
		{
			PerformInit(null);
		}

        private void PerformInit(string title)
        {
            TextLabel = new UILabel()
            {
                Font = Fonts.SmallSemibold,
                BackgroundColor = UIColor.Clear,
                TextColor = new UIColor(0.6f, 0.6f, 0.6f, 1f),
				Text = title
			};
            SeparatorView = new UIView()
            {
                BackgroundColor = new UIColor(0.86f, 0.86f, 0.86f, 1f)
            };
            SeparatorView.Hidden = true;
			LayoutMargins = new UIEdgeInsets(1, 15, 1, 5);
			BackgroundColor = new UIColor(0.957f, 0.957f, 0.957f, 1f);
			Bounds = new RectangleF(0, 0, 320, 25);
			AddSubview(TextLabel);
            AddSubview(SeparatorView);
		}

		public UILabel TextLabel { get; set; }
        public UIView SeparatorView { get; set; }

		public override void LayoutSubviews()
		{
            TextLabel.Frame = this.LayoutBox()
                .Left(15)
                .Right(0)
                .CenterVertically()
                .Height(HeaderHeight);

            SeparatorView.Frame = new CGRect(
                0,
                Bounds.Height,
                Bounds.Width,
                1);
		}

		public static float HeaderHeight
		{
			get
			{
				return 25f;
			}
		}
	}
}

