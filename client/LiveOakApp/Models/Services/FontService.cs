using System;
using System.Collections.Generic;

#if __ANDROID__
using Android.Graphics;
using Android.Content;
#endif

namespace LiveOakApp.Models.Services
{
	public class FontService
	{

#if __ANDROID__

        public const string OpenSansRegular = "OpenSans-Regular";
        public const string OpenSansBold = "OpenSans-Bold";
        public const string OpenSansBoldItalic = "OpenSans-BoldItalic";
        public const string OpenSansExtraBold = "OpenSans-ExtraBold";
        public const string OpenSansExtraBoldItalic = "OpenSans-ExtraBoldItalic";
        public const string OpenSansItalic = "OpenSans-Italic";
        public const string OpenSansLightItalic = "OpenSans-LightItalic";
        public const string OpenSansSemibold = "OpenSans-Semibold";
        public const string OpenSansSemiboldItalic = "OpenSans-SemiboldItalic";
        public const string OpenSansLight = "OpenSans-Light";


        Dictionary<string, Typeface> typefaces;

		public FontService()
		{
			typefaces = new Dictionary<string, Typeface>();
		}

		public Typeface GetFont(Context context, string typefaceName)
		{
			Typeface tf;

			if (typefaces.ContainsKey(typefaceName))
			{
				tf = typefaces[typefaceName];
			}
			else
			{
				var path = ("fonts/" + typefaceName + ".ttf");
				tf = Typeface.CreateFromAsset(context.Assets, path);

				typefaces.Add(typefaceName, tf);
			}



			return tf;
		}

		#endif
	}
}

