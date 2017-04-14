using System;
using CoreGraphics;
using CoreText;
using System.Runtime.InteropServices;
using ObjCRuntime;
using UIKit;

namespace StudioMobile
{
	public partial class IconGenerator
	{
		public Image CreateIcon (FontIcon icon)
		{			
			using (var path = CreatePath (icon.Font, icon.IconIndex)) {
				return new Image(CreateIconFromPath (path, Appearance));
			}
		}

		public CGPath CreatePath (Font ctfont, char icon)
		{						
			var transform = CGAffineTransform.MakeIdentity ();
			return ctfont.CTFont.GetPathForGlyph (GlyphForIcon (ctfont, icon), ref transform);
		}

		readonly char[] getGlyphCharBuffer = new char[1];
		readonly ushort[] getGlyphGlyphBuffer = new ushort[1];

		public ushort GlyphForIcon (CTFont ctfont, char icon)
		{
			getGlyphCharBuffer [0] = icon;
			getGlyphGlyphBuffer [0] = 0;
			CTFontGetGlyphsForCharacters (ctfont.Handle, getGlyphCharBuffer, getGlyphGlyphBuffer, 1);
			return getGlyphGlyphBuffer [0];
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern bool CTFontGetGlyphsForCharacters (
			IntPtr font, 
			[In, MarshalAs (UnmanagedType.LPWStr)] char[] characters, 
			[Out] ushort[] glyphs,
			nint count);

		static public UIImage CreateIconFromPath (CGPath path, FontIconAppearance appearance)
		{
			//NOTE: glyph bounds are has negative Y origin which absolute value is equal to baseline.
			//this is why traslate and scale is used to convert context coordinates into glyph coordinates.
			var bounds = path.BoundingBox;
			var baseLineY = bounds.Y;
			var imageSize = new CGSize (bounds.Width + appearance.StrokeWidth*2, bounds.Height + appearance.StrokeWidth*2);
			UIGraphics.BeginImageContextWithOptions (imageSize, false, 0f);
			try {
				using (var context = UIGraphics.GetCurrentContext ()) {
					context.TranslateCTM (0, imageSize.Height + baseLineY);
					context.ScaleCTM (1, -1);
					path.RenderInContext (context, appearance);
					var image = UIGraphics.GetImageFromCurrentImageContext ();
					UIGraphics.EndImageContext ();
					if (image.RenderingMode != appearance.RenderingMode) {
						image = image.ImageWithRenderingMode (appearance.RenderingMode);
					}
					return image;
				}
			} finally {
				UIGraphics.EndImageContext ();
			}
		}
	}


	public static class UILabelFontIconExtensions
	{
		public static void SetIcon(this UILabel label, FontIcon icon)
		{
			label.Font = icon.Font;
			label.Text = icon.IconIndex.ToString();
		}

		public static void SetIcon(this UILabel label, FontIconTemplate template)
		{
			label.SetIcon(template.Icon(label.Font.PointSize));
		}
	}

	public static class UIButtonFontIconExtensions
	{
		public static void SetIcon(this UIButton btn, FontIcon icon, UIControlState state = UIControlState.Normal)
		{
			btn.Font = icon.Font.UIFont;
			var f = btn.Font;
			var b = f.FamilyName;
			var c = f.PointSize;
			btn.SetTitle(icon.IconIndex.ToString(), state);
		}

		public static void SetIcon(this UIButton btn, FontIconTemplate template, UIControlState state = UIControlState.Normal)
		{
			btn.SetIcon (template.Icon (btn.Font.PointSize), state);
		}
	}

	public static class UIBarButtonItemFontIconExtensions
	{
		public static void SetIcon(this UIBarButtonItem btn, FontIcon icon, UIColor color, UIControlState state = UIControlState.Normal)
		{
			var attrs = new UITextAttributes {
				Font = icon.Font,
				TextColor = color
			};
			btn.Title = icon.IconIndex.ToString();
			btn.SetTitleTextAttributes (attrs, state);
		}

		public static void SetIcon(this UIBarButtonItem btn, FontIconTemplate template, UIColor color, UIControlState state = UIControlState.Normal)
		{
			var attrs = btn.GetTitleTextAttributes (state);
			var size = attrs.Font != null ? attrs.Font.PointSize : 12;
			btn.SetIcon (template.Icon (size), color, state);
		}
	}

	public static class UITabBarItemFontIconExtensions 
	{
		static readonly IconGenerator generator = new IconGenerator();
		public static void SetIcon(this UITabBarItem tab, FontIcon icon)
		{			
			tab.Image = generator.CreateIcon (icon);
		}

		public static void SetIcon(this UITabBarItem tab, FontIconTemplate template)
		{
			var attrs = tab.GetTitleTextAttributes (UIControlState.Normal);
			var size = attrs.Font.PointSize;
			tab.SetIcon (template.Icon (size));
		}
	}
}

