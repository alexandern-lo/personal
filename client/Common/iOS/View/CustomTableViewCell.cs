using System;
using UIKit;
using StudioMobile;
using Foundation;
using System.Collections.Generic;

namespace StudioMobile
{
	public class CustomTableViewCell : UITableViewCell
	{
		List<object> builtObjects;

		public CustomTableViewCell (UITableViewCellStyle style, string reuseIdentifier) : base (style, reuseIdentifier)
		{
			Initialize ();
		}

		public CustomTableViewCell ()
		{
			Initialize ();
		}

		public CustomTableViewCell (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		public CustomTableViewCell (NSObjectFlag t) : base (t)
		{
			Initialize ();
		}

		public CustomTableViewCell (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		public CustomTableViewCell (CoreGraphics.CGRect frame) : base (frame)
		{
			Initialize ();
		}

		public CustomTableViewCell (UITableViewCellStyle style, NSString reuseIdentifier) : base (style, reuseIdentifier)
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
	
	public class CustomTableViewCell<ContentT> : CustomTableViewCell
		where ContentT : UIView, new()
	{
		public CustomTableViewCell (UITableViewCellStyle style, string reuseIdentifier) : base (style, reuseIdentifier)
		{
		}

		public CustomTableViewCell (UITableViewCellStyle style, NSString reuseIdentifier) : base (style, reuseIdentifier)
		{
		}

		public CustomTableViewCell ()
		{
		}

		public CustomTableViewCell (NSCoder coder) : base (coder)
		{
		}

		public CustomTableViewCell (NSObjectFlag t) : base (t)
		{
		}

		public CustomTableViewCell (IntPtr handle) : base (handle)
		{
		}

		public CustomTableViewCell (CoreGraphics.CGRect frame) : base (frame)
		{
		}

		protected override void CreateView ()
		{
			base.CreateView ();
			Content = new ContentT ();
		}

		private ContentT content;

		public virtual ContentT Content {
			get { return content; }
			set {
				if (content != null) {
					content.RemoveFromSuperview ();
				}
				content = value;
				ContentView.AddSubview (content);
			}
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			content.Frame = ContentView.LayoutBox ()
				.Left (0).Right (0).Top (0).Bottom (0);
		}
	}
	
}
