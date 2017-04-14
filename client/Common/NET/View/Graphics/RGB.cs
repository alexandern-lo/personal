using System;

namespace StudioMobile
{
	public partial struct RGB
	{
		public static implicit operator System.Drawing.Color (RGB color)
		{
			return color.Native;
		}

		public System.Drawing.Color Native  {
			get { 
				return System.Drawing.Color.FromArgb (Alpha, Red, Green, Blue); 
			}
		}

		public static void Dispose (RGB color)
		{		
		}
	}
}

