using System;

namespace StudioMobile
{
	public partial struct Font
	{
		public FontIcon Icon(char icon)
		{
			return new FontIcon { 
				Font = this,
				IconIndex = icon
			};
		}
	}
}

