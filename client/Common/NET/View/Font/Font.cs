using System;

namespace StudioMobile
{
	public partial struct Font : IDisposable
	{
		public System.Drawing.Font Native;

		public Font(string name, float size)
		{
			Native = new System.Drawing.Font (name, size);
		}

		public static implicit operator System.Drawing.Font(Font f)
		{
			return f.Native;
		}

		public void Dispose()
		{
			Native.Dispose ();
		}
	}
}

