using System;
using UIKit;

namespace StudioMobile
{
	public class ClickableView : CustomView 
	{
		public UITapGestureRecognizer TapRecognizer { get; private set; }
		protected override void CreateView ()
		{
			base.CreateView ();

			TapRecognizer = new UITapGestureRecognizer () {
				NumberOfTapsRequired = 1,
				CancelsTouchesInView = true,
				DelaysTouchesBegan = true,
				DelaysTouchesEnded = true
			};
			TapRecognizer.AddTarget (TapHandler);
			AddGestureRecognizer (TapRecognizer);
		}

		void TapHandler ()
		{
			OnClick ();
			if (Click != null) {
				Click (this, EventArgs.Empty);
			}
		}

		protected virtual void OnClick()
		{
		}

		public event EventHandler Click;

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				RemoveGestureRecognizer (TapRecognizer);
			}
			base.Dispose (disposing);
		}
	}
}
