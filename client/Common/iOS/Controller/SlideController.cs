using System;
using UIKit;

namespace StudioMobile
{

	public static class UIViewController_SlideMenu 
	{
		public static SlideController GetSlideController(this UIViewController controller)
		{
			UIViewController parent;
			do {
				parent = controller.ParentViewController;
				var slideController = parent as SlideController;
				if (slideController != null) {
					return slideController;
				}
				controller = parent;
			} while (parent != null);
			return null;
		}
	}

	public class SlideController : UIViewController
	{

		public SlideController ()
		{
		}

		public SlideController (IntPtr handle) : base (handle)
		{
		}
			
		public override void LoadView ()
		{
			var view = new SlideLayoutView ();
			view.WillPresentView += WillPresentViewHandler;
			view.DidPresentView += DidPresentViewHandler;
			view.WillDismissView += WillDismissViewHandler;
			view.DidDismissView += DidDismissViewHandler;
			View = view;
		}

		UIViewController ViewController(UIView view)
		{
			if (leftController != null && leftController.View == view)
				return leftController;
			if (rightController != null && rightController.View == view)
				return rightController;
			if (contentController != null && contentController.View == view)
				return contentController;
			return null;
		}

		void WillPresentViewHandler (object sender, DisplaySlideViewEventArgs e)
		{
			var controller = ViewController (e.TargetView);
			if (controller != null) {
				controller.ViewWillAppear (e.Animated);
			}
		}

		void DidPresentViewHandler (object sender, DisplaySlideViewEventArgs e)
		{
			var controller = ViewController (e.TargetView);
			if (controller != null) {
				controller.ViewDidAppear (e.Animated);
			}
		}

		void WillDismissViewHandler (object sender, DisplaySlideViewEventArgs e)
		{
			var controller = ViewController (e.TargetView);
			if (controller != null) {
				controller.ViewWillDisappear (e.Animated);
			}
		}

		void DidDismissViewHandler (object sender, DisplaySlideViewEventArgs e)
		{
			var controller = ViewController (e.TargetView);
			if (controller != null) {
				controller.ViewDidDisappear (e.Animated);
			}
		}

		public float SlideSpeed { 
			get { return LayoutView.SlideSpeed; }
			set { LayoutView.SlideSpeed = value; }
		}

		public float VelocityThreshold { 
			get { return LayoutView.VelocityThreshold; }
			set { LayoutView.VelocityThreshold = value; }
		}

		public float TranslateThreshold { 
			get { return LayoutView.TranslateThreshold; }
			set { LayoutView.TranslateThreshold = value; }
		}

		public SlideLayoutView LayoutView { get { return View as SlideLayoutView; } }

		public float LeftWidth {
			get { return LayoutView.LeftWidth; }
			set { LayoutView.LeftWidth = value; }
		}

		public float RightWidth {
			get { return LayoutView.RightWidth; }
			set { LayoutView.RightWidth = value; }
		}

		UIViewController leftController;

		public UIViewController LeftController { 
			get { return leftController; }
			set {
				leftController = SetController (value, SlideLayoutLocation.Left);
			}
		}

		UIViewController rightController;

		public UIViewController RightController { 
			get { return rightController; }
			set { 
				rightController = SetController (value, SlideLayoutLocation.Right);
			}
		}

		UIViewController contentController;

		public UIViewController ContentController {
			get { return contentController; }
			set { 
				contentController = SetController (value, SlideLayoutLocation.Center);
			}
		}

		UIViewController SetController (UIViewController controller, SlideLayoutLocation location)
		{
			var view = LayoutView.GetViewForLocation(location);
			var prevController = ViewController(view);
			if (prevController != null) {
				prevController.RemoveFromParentViewController ();
			}
			if (controller != null) {
				AddChildViewController (controller);
				LayoutView.SetView (controller.View, location);
			} else {
				LayoutView.SetView (null, location);
			}
			return controller;
		}
			
		public void PresentMenu (SlideLayoutLocation location, bool animated = true)
		{
			LayoutView.PresentView (location, animated);
		}

		public void DismissMenu (bool animated = true)
		{
			LayoutView.DismissPresentedView (animated);
		}

		public bool GesturesEnabled {
			get { 
				return LayoutView.PanGestureRecognizer.Enabled && LayoutView.TapGestureRecognizer.Enabled; 
			}
			set { 
				LayoutView.PanGestureRecognizer.Enabled = value;
				LayoutView.TapGestureRecognizer.Enabled = value;
			}
		}
	}
}

