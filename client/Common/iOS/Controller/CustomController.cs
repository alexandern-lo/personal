using System;

using Foundation;
using SL4N;
using UIKit;

namespace StudioMobile
{
	public class CustomController<T> : UIViewController where T: UIView, new()
	{
        ILogger _log;
        protected ILogger LOG { get { return _log ?? (_log = LoggerFactory.GetLogger(GetType().Name)); } }

		public CustomController ()
		{
			Initialize ();
		}

		public CustomController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		public CustomController (NSObjectFlag t) : base (t)
		{
			Initialize ();
		}

		public CustomController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		public CustomController (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
			Initialize ();
		}

		void Initialize()
		{
			LOG.Info("Created");
			Bindings = new BindingList ();
		}
		
		public override void LoadView ()
		{
			base.View = new T ();
		}

		public new T View { 
			get { return base.View as T; } 
			set { base.View = value; }
		}

        public override void ViewDidLoad()
        {
            LOG.Info("View did load");
            base.ViewDidLoad();
        }

		public override void ViewWillAppear (bool animated)
		{
			LOG.Info("View will appear");
			base.ViewWillAppear (animated);
            Bindings.Bind ();
			Bindings.UpdateTarget ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			LOG.Info("View will disappear");
			base.ViewWillDisappear (animated);
			Bindings.Unbind ();
		}

		BindingList bindings;
		public BindingList Bindings { 
			get { 
				if (bindings == null) {
					bindings = new BindingList ();
				}
				return bindings;
			}
			set {
				Check.Argument(value, "value").NotNull();
				bindings = value;
			}
		}

		protected override void Dispose(bool disposing)
		{			
			base.Dispose(disposing);
			LOG.Info("Disposed");
		}
	}
}
