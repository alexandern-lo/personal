using System;

namespace StudioMobile
{
	public partial struct RGB
	{
		public RGB(UIKit.UIColor color) : this()
		{
			System.nfloat r, g, b, a;
			color.GetRGBA (out r, out g, out b, out a);
			Red = (byte)(r * 255);
			Green = (byte)(g * 255);
			Blue = (byte)(b * 255);
			Alpha = (byte)(a * 255);
		}
		UIKit.UIColor nativeColor;
		public static implicit operator UIKit.UIColor (RGB color)
		{
			return color.Native;
		}
		public CoreGraphics.CGColor CGColor { get { return this.Native.CGColor; } }
		public UIKit.UIColor Native { 
			get  { 
				if (nativeColor == null)
					nativeColor = new UIKit.UIColor (Red / 255f, Green / 255f, Blue / 255f, Alpha / 255f);
				return nativeColor;
			} 
		}

		public static void Dispose (RGB color)
		{
			if (color.nativeColor != null)
				color.nativeColor.Dispose ();
		}
	}
}

