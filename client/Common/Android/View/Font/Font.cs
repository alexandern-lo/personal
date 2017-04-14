using Android.Graphics;
using Android.App;
using Android.Text;
using System;

namespace StudioMobile
{
	public partial struct Font : IDisposable
	{
		public TextPaint Native;

		public Font(string name, float size)
		{
			var ctx = Application.Context;
			var typeface = Typeface.CreateFromAsset(ctx.Assets, name);
			Native = new TextPaint ();
			Native.TextSize = size;
			Native.SetTypeface (typeface);
		}

		public static implicit operator TextPaint(Font f)
		{
			return f.Native;
		}

		public void Dispose()
		{
			if (Native != null) 
				Native.Dispose ();
		}
	}
}

