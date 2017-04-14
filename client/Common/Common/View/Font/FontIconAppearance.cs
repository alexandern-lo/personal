using System;
using System.Drawing;

namespace StudioMobile
{
	public class FontIconAppearance : IDisposable
	{
		public FontIconAppearance ()
		{
			Colors = new [] { RGB.DarkGray };
			StrokeColor = RGB.Black;
			StrokeWidth = 0;
		}

		#if __IOS__
		UIKit.UIImageRenderingMode renderingMode = UIKit.UIImageRenderingMode.Automatic;
		public UIKit.UIImageRenderingMode RenderingMode { 
			get { return renderingMode; } 
			set { renderingMode = value;} 
		}
		#endif

		RGB[] colors;
		public RGB[] Colors { 
			get { return colors; } 
			set {
				colors = value;
				#if __IOS__
				UpdateCGColors();
				#endif
			}
		}

		#if __IOS__
		void UpdateCGColors()
		{
			Cleanup.All (CGColors);
			CGColors = new CoreGraphics.CGColor[colors.Length];
			for (var i = 0; i < colors.Length; ++i) {
				CGColors [i] = colors [i].CGColor;
			}
		}
		public CoreGraphics.CGColor[] CGColors {  get; private set; }
		#endif

		public RGB StrokeColor { get; set; }
		public float StrokeWidth { get; set; }

		protected virtual void Dispose (bool disposing)
		{
			if (disposing) {
				RGB.Dispose (StrokeColor);
				foreach (var color in colors) {
					RGB.Dispose (color);
				}
				#if __IOS__
				Cleanup.All (CGColors);
				#endif
			}
		}

		public void Dispose ()
		{
			Dispose (true);
		}

		~FontIconAppearance ()
		{
			Dispose (false);
		}
	}
}

