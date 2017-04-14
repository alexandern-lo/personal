using Color = Android.Graphics.Color;

namespace StudioMobile
{
	public partial struct RGB
	{
		public static implicit operator Color (RGB color)
		{
			return color.Native;
		}

		public Color Native  {
			get { return new Color (Red, Green, Blue, Alpha); }
		}

		public static void Dispose (RGB color)
		{			
		}
	}
}

