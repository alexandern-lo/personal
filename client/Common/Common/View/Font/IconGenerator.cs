using System;

namespace StudioMobile
{
	public partial class IconGenerator
	{
		public IconGenerator ()
		{
			Appearance = new FontIconAppearance ();
		}

		public FontIconAppearance Appearance { get; set; }

		protected virtual void Dispose (bool disposing)
		{
			if (disposing) {
				if (Appearance != null) {
					Appearance.Dispose ();
				}
			}
		}

		public void Dispose ()
		{
			Dispose (true);
		}

		~IconGenerator ()
		{
			Dispose (false);
		}
	}
}

