using System;
using UIKit;
using System.Drawing;
using Foundation;
using StudioMobile;

namespace StudioMobile
{
	public class UIViewHighlighter
	{
		public UIViewHighlighter()
		{
			//LongPress recognizer with very small press duration parameter
			//works just like touch up/touch down detector.
			var r = new UILongPressGestureRecognizer {
				CancelsTouchesInView = false,
				MinimumPressDuration = 0.001,
				DelaysTouchesBegan = false,
				DelaysTouchesEnded = false
			};
			r.AddTarget (PanGestureStateChanged);
			Gesture = r;
		}

		void PanGestureStateChanged()
		{
			switch (Gesture.State) {
			case UIGestureRecognizerState.Began:
				Highlight ();
				break;
			case UIGestureRecognizerState.Ended:
			case UIGestureRecognizerState.Cancelled:
				Unhighlight ();
				break;
			default:
				break;
			}		
		}

		public UIGestureRecognizer Gesture { get; private set; }

		public UIView View { get; private set; } 

		public UIColor HighlightColor { get; set; }
		UIColor oldColor;

		public void BeginHighlighting(UIView view)
		{
			if (View != null)
				throw new InvalidOperationException ();
			View = view;
			View.AddGestureRecognizer (Gesture);
		}

		public void EndHighlighting()
		{
			if (View != null) {
				View.RemoveGestureRecognizer (Gesture);
				if (oldColor != null)
					View.BackgroundColor = oldColor;
				View = null;
			}
		}

		void Highlight ()
		{
			oldColor = View.BackgroundColor;
			View.BackgroundColor = HighlightColor;
		}

		void Unhighlight ()
		{
			View.BackgroundColor = oldColor;
		}
	}
	
}
