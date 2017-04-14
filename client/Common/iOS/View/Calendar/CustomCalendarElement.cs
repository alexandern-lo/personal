using System;
using UIKit;
using StudioMobile;
using System.Reflection;
using System.Linq;
using Foundation;
using System.Collections.Generic;

namespace StudioMobile
{

	//TODO clients of this class has to maintain List<object> of all objects built by ViewBuilder
	//to be able to dispose them later on. Maybe it is better to host ViewBuilder objects which control
	//created views instead of having List<object> in each client view?

	public class CustomCalendarElement : CalendarViewElement
	{
		List<object> builtObjects;

		public CustomCalendarElement ()
		{
			Initalize ();
		}

		public CustomCalendarElement (NSCoder coder) : base (coder)
		{
			Initalize ();
		}

		public CustomCalendarElement (NSObjectFlag t) : base (t)
		{
			Initalize ();
		}

		public CustomCalendarElement (IntPtr handle) : base (handle)
		{
			Initalize ();
		}

		public CustomCalendarElement (CoreGraphics.CGRect frame) : base (frame)
		{
			Initalize ();
		}

		void Initalize ()
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
