namespace StudioMobile
{
	public static class CharExtensions 
	{
		public static FontIcon FontIconWithFont(this char c, Font font)
		{
			return new FontIcon {
				IconIndex = c,
				Font = font
			};
		}
	}

}
