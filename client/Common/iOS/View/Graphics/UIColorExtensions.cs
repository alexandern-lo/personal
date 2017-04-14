using System;
using UIKit;

namespace StudioMobile
{
	public static class UIColorExtensions
	{
		public static UIColor Lighter(this UIColor color, float value)
		{
			if (color == null)
				throw new ArgumentNullException ();
			nfloat h, s, b, a;
			color.GetHSBA (out h, out s, out b, out a);
			b = b * (1+value);
			return UIColor.FromHSBA (h, s, b, a);
		}
	}
}

