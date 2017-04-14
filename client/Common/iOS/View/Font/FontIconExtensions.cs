using System;
using UIKit;
using CoreGraphics;
using System.Drawing;

namespace StudioMobile
{
	public static class FontIconExtensions
	{
		static readonly IconGenerator generator = new IconGenerator();

		public static NSTextAttachment ToTextAttachment(this FontIcon icon, UIColor textColor)
		{
			return icon.ToTextAttachment (textColor, RectangleF.Empty);
		}
		//NOTE - bounds.Y = 0 corresponds to baseline, positivy value => above baseline, negative values - below baseline
		public static NSTextAttachment ToTextAttachment(this FontIcon icon, UIColor textColor, RectangleF bounds)
		{
			generator.Appearance.Colors = new [] { new RGB(textColor) };
			generator.Appearance.StrokeColor = new RGB(textColor);
			var image = generator.CreateIcon (icon);

			var rect = bounds.IsEmpty ? new CGRect (CGPoint.Empty, new CGSize (image.Width, image.Height)) : (CGRect)bounds;
			return new NSTextAttachment { 
				Bounds = rect,
				Image = image
			};
		}
	}
}

