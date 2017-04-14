using System;
using UIKit;
using StudioMobile;
using CoreGraphics;
using Foundation;

namespace StudioMobile
{
	public class SMUITextView : UITextView 
	{
		public UILabel Placeholder { get; private set; }

		public SMUITextView ()
		{
			Initialize ();
		}

		public SMUITextView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		public SMUITextView (NSObjectFlag t) : base (t)
		{
			Initialize ();
		}

		public SMUITextView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		public SMUITextView (CGRect frame) : base (frame)
		{
			Initialize ();
		}
		
		public SMUITextView (CGRect frame, NSTextContainer textContainer) : base (frame, textContainer)
		{
			Initialize ();
		}

		protected void Initialize()
		{
			Placeholder = new UILabel ();
			AddSubview (Placeholder);

			ShouldEndEditing = t => {
				Placeholder.Hidden = !String.IsNullOrWhiteSpace (Text);
				return true;
			};

			Changed += (sender, e) => {
				var showPlaceholder = String.IsNullOrWhiteSpace (Text);
				if (showPlaceholder && Placeholder.Hidden) {
					SetNeedsLayout();
				}
				Placeholder.Hidden = !showPlaceholder;
			};
			Placeholder.Hidden = true;
		}

		public override string Text {
			get {
				return base.Text;
			}
			set {
				base.Text = value;
				Placeholder.Hidden = !String.IsNullOrWhiteSpace (value);
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
			}
			base.Dispose (disposing);
		}
			
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			if (!Placeholder.Hidden) {
				Placeholder.SizeToFit ();
				var pos = GetCaretRectForPosition (SelectedTextRange.Start);
				Placeholder.Frame = this.LayoutBox ()
				.Top (TextContainerInset.Top)
				.Left (TextContainerInset.Left + pos.Left)
				.Width (Placeholder)
				.Height (Placeholder);
			}
		}
	}
	
}
