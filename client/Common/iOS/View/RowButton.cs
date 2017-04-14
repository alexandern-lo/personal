using System;
using StudioMobile;
using UIKit;

namespace StudioMobile
{
	public class RowButton : AccessoryButton
	{
		[View]
		UIView TopLine { get; set; }

		[View]
		UIView BottomLine { get; set; }

		protected override void CreateView ()
		{
			base.CreateView ();
			TopLine.BackgroundColor = BottomLine.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			TopLine.Frame = this.LayoutBox ()
				.Top (0).Left (-10).Right (-10).Height (1);
			BottomLine.Frame = this.LayoutBox ()
				.Bottom (0).Left (-10).Right (-10).Height (1);
		}

		public UIColor SeparatorColor {
			get { return TopLine.BackgroundColor; }
			set { 
				TopLine.BackgroundColor = BottomLine.BackgroundColor = value;
			}
		}
	}
}

