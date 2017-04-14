using System;

namespace StudioMobile
{
	public static class FontIconTemplateExtensions
	{
		public static FontIcon Icon(this FontIconTemplate template, nfloat size)
		{
			return template.Icon((float)size);
		}
	}
}

