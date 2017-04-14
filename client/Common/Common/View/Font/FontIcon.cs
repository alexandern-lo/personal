using System;

namespace StudioMobile
{
	/// <summary>
	/// Font with well known name which can be loaded by just supplying font name. 
	/// Usually such fonts are bundled with app or provide by system (example - Arial).
	/// </summary>
	public class WellKnownFont 
	{
		public WellKnownFont(string name)
		{
			if (name == null)
				throw new ArgumentNullException ();
			Name = name;
		}

		public string Name { get; private set; }

		public Font FontWithSize(float size)
		{
			return new Font (Name, size);
		}

		public FontIcon Icon(char icon, float size)
		{
			return new FontIcon { 
				IconIndex = icon,
				Font = FontWithSize(size)
			};
		}
	}

	/// <summary>
	/// Template for an icon image. Since icons produced from font glyph can be of different size there is 
	/// one to many relationships between glyp in a font and created images.
	/// FontIconTemplate can be used to generate as many icon images from a given font and glyph as neccecary.
	/// </summary>
	public partial struct FontIconTemplate 
	{
		public FontIconTemplate(WellKnownFont font, char icon) : this()
		{
			Font = font;
			IconIndex = icon;
		}

		public WellKnownFont Font;
		public char IconIndex;

		public FontIcon Icon(float size)
		{
			return new FontIcon {
				IconIndex = IconIndex,
				Font = Font.FontWithSize (size)
			};
		}
	}

	/// <summary>
	/// Font icons produced from FontIconTemplate's and hold font of specific size and glyph.
	/// </summary>
	public struct FontIcon
	{
		public Font Font;
		public char IconIndex;
	}
}

