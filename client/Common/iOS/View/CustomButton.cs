using System;
using System.Collections.Generic;
using UIKit;

namespace StudioMobile
{
	public class CustomButton : UIButton
	{
		List<object> builtObjects;

		public CustomButton(IntPtr handle) : base (handle)
		{
			Initialize();
		}

		public CustomButton()
		{
			Initialize();
		}

		void Initialize()
		{
			CreateView();
		}

		protected virtual void CreateView()
		{
			builtObjects = ViewBuilder.Build(this);
			BackgroundColor = UIColor.White;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Cleanup.List(builtObjects);
			}
			base.Dispose(disposing);
		}
	}
}

