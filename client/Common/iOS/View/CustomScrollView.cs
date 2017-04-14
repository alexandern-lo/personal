using System;
using UIKit;
using StudioMobile;
using System.Collections.Generic;

namespace StudioMobile
{
	public class CustomScrollView : UIScrollView
	{
		List<object> builtObjects;

		public CustomScrollView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		public CustomScrollView ()
		{
			Initialize ();
		}

		void Initialize ()
		{
			CreateView ();
		}

		protected virtual void CreateView ()
		{
			builtObjects = ViewBuilder.Build (this);
			BackgroundColor = UIColor.White;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				Cleanup.List (builtObjects);
			}
			base.Dispose (disposing);
		}
	}
	
}
