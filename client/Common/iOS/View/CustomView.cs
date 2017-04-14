using System;
using UIKit;
using System.Collections.Generic;

namespace StudioMobile
{
	//TODO clients of this class has to maintain List<object> of all objects built by ViewBuilder
	//to be able to dispose them later on. Maybe it is better to host ViewBuilder objects which control
	//created views instead of having List<object> in each client view?

	public class CustomView : UIView
	{
		List<object> builtObjects;

		public CustomView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		public CustomView ()
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
