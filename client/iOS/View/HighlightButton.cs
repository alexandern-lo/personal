using System;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View
{
	public class HighlightButton : CustomButton
	{
		public HighlightButton(IntPtr handle) : base (handle)
		{
		}

		public HighlightButton()
		{
		}

		private UIColor normalColor;
		public UIColor NormalColor
		{
			get
			{
				return normalColor;
			}
			set
			{
				normalColor = value;
				BackgroundColor = value;
			}
		}

		public UIColor PressedColor { get; set; }

		public override bool Highlighted
		{
			get
			{
				return base.Highlighted;
			}
			set
			{
				if (value)
				{
					BackgroundColor = PressedColor;
				}
				else {
					BackgroundColor = NormalColor;
				}
				base.Highlighted = value;
			}
		}
	}
}
