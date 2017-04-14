using System;
using UIKit;

namespace StudioMobile
{
	public class ModalViewController : UIViewController
	{
		UITapGestureRecognizer tapRecognizer;

		public ModalViewController()
		{
			Initialize();
		}

		public ModalViewController(Foundation.NSCoder coder) : base(coder)
		{
			Initialize();
		}

		public ModalViewController(Foundation.NSObjectFlag t) : base(t)
		{
			Initialize();
		}

		public ModalViewController(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		public ModalViewController(string nibName, Foundation.NSBundle bundle) : base(nibName, bundle)
		{
			Initialize();
		}

		void Initialize()
		{
			ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
			tapRecognizer = new UITapGestureRecognizer(OnTap);
		}

		public override void LoadView()
		{
			View = new UIView();
			View.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 127);
			ViewDidLoad();
		}

		public override UIView View
		{
			get
			{
				return base.View;
			}
			set
			{
				if (IsViewLoaded)
				{
					View.RemoveGestureRecognizer(tapRecognizer);
				}
				base.View = value;
				if (tapRecognizer.View != value)
				{
					View.AddGestureRecognizer(tapRecognizer);
				}
			}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			View.AddGestureRecognizer(tapRecognizer);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			if (WillOpen != null) WillOpen(this, EventArgs.Empty);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			if (WillClose != null) WillClose(this, EventArgs.Empty);
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			if (Opened != null) Opened(this, EventArgs.Empty);
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			if (Closed != null) Closed(this, EventArgs.Empty);
		}

		void OnTap()
		{
			var location = tapRecognizer.LocationInView(View);
			var view = View.HitTest(location, null);
			if (view == View)
			{
				Close(true);
			}
		}

		public event EventHandler Closed, Opened, WillClose, WillOpen;

		UIView contentView;
		public UIView ContentView
		{
			get { return contentView; }
			set
			{
				if (contentView != value)
				{
					if (contentView != null) contentView.RemoveFromSuperview();
					contentView = value;
					if (contentView != null)
					{
						contentView.SizeToFit();
						View.AddSubview(contentView);
					}
				}
			}
		}

		public void Open(UIViewController parent = null, bool animated = true)
		{
			if (parent == null)
				parent = UIApplication.SharedApplication.KeyWindow.RootViewController;
            parent.PresentViewController(this, animated, null);
		}

		public void Close(bool animated = true)
		{
			DismissModalViewController(animated);
		}
	}
	
}
