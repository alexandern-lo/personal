using CoreText;
using System;
using UIKit;
using CoreGraphics;

namespace StudioMobile
{
	public partial struct Font : IDisposable
	{
		public Font(string name, float size) : this()
		{
			ctFont = new CTFont (name, size);
		}

		public void Dispose()
		{
			Clear ();
		}

		public static implicit operator UIFont(Font f)
		{
			return f.UIFont;
		}

		UIFont uiFont;
		public UIFont UIFont {
			get {
				if (uiFont == null && ctFont != null) {
					uiFont = UIFont.FromName (ctFont.FamilyName, ctFont.Size);
				}
				return uiFont;
			}
			set {
				Clear ();
				uiFont = value;
			}
		}

		public static implicit operator CTFont(Font f)
		{
			return f.CTFont;
		}

		CTFont ctFont;
		public CTFont CTFont {
			get {
				if (ctFont == null && uiFont != null) {
					using (var cgFont = CGFont.CreateWithFontName (uiFont.FamilyName)) {
						ctFont = new CTFont (cgFont, uiFont.PointSize, CGAffineTransform.MakeIdentity());
					}
				}
				return ctFont;
			}
			set { 
				Clear ();
				ctFont = value;
			}
		}

		void Clear()
		{
			if (uiFont != null)
				uiFont.Dispose ();
			if (ctFont != null)
				ctFont.Dispose ();
		}
	}

	public static class UIFontExtensions 
	{
		public static FontIcon Icon(this UIFont f, char c)
		{
			return new FontIcon { 
				Font = new Font { UIFont = f },
				IconIndex = c
			};
		}

		public static FontIcon Icon(this UIFont f, FontIconTemplate template)
		{
			return template.Icon (f.PointSize);
		}
	}
}

