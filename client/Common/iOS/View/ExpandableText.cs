using System;
using UIKit;
using Foundation;
using CoreText;
using CoreGraphics;
using StudioMobile;

namespace StudioMobile
{
	public class ExpandableTextEventArgs : EventArgs
	{
		public bool Expanding { get; internal set; }

		public float ToHeight { get; internal set; }
	}

	public class ExpandableText : UIView
	{
		public ExpandableText ()
		{
			CreateView ();
		}

		public ExpandableText (IntPtr handle) : base (handle)
		{
			CreateView ();
		}

		private void CreateView ()
		{
			BackgroundColor = UIColor.White;
			AttributedText = new NSAttributedString ();
			MoreAttributedText = new NSAttributedString ("… More");
			TapRecognizer = new UITapGestureRecognizer {
				NumberOfTapsRequired = 1,
				CancelsTouchesInView = true,
				DelaysTouchesBegan = true,
				DelaysTouchesEnded = true
			};
			TapRecognizer.AddTarget (TapHandler);
			AddGestureRecognizer (TapRecognizer);
		}

		public event EventHandler Clicked;

		private void TapHandler ()
		{
			if (Clicked != null) {
				Clicked (this, EventArgs.Empty);
			}
		}

		public UITapGestureRecognizer TapRecognizer {
			get;
			private set;
		}

		public string MoreText {
			get { return moreText.Value; }
			set { 
				MoreAttributedText = new NSAttributedString (value);
			}
		}

		private NSMutableAttributedString moreText;

		private NSAttributedString MoreAttributedText {
			get { return moreText; }
			set {
				if (value == null)
					throw new ArgumentNullException ();
				moreText = new NSMutableAttributedString (value);
				ResetCachedFramesetter ();
				SetNeedsLayout ();
			}
		}

		private UIFont moreFont;

		public UIFont MoreFont {
			get { return moreFont; }
			set {
				if (value == null)
					throw new ArgumentNullException ();
				moreFont = value;
				moreText.AddAttribute (UIStringAttributeKey.Font, moreFont, new NSRange (0, moreText.Length));
				SetNeedsLayout ();
			}
		}

		private UIColor moreTextColor;

		public UIColor MoreTextColor {
			get { return textColor; }
			set {
				if (value == null)
					throw new ArgumentNullException ();
				moreTextColor = value;
				moreText.AddAttribute (UIStringAttributeKey.ForegroundColor, moreTextColor, new NSRange (0, moreText.Length));
				SetNeedsDisplay ();
			}
		}

		public event EventHandler<ExpandableTextEventArgs> Expanding, Expanded;

		nfloat heightBeforeExpand = 0;
		bool expanded = false;

		public bool IsExpanded {
			get { return expanded; }
		}

		public void Expand(bool value)
		{
			if (value != expanded) {					
				nfloat targetHeight;
				if (value) {						
					var size = SizeThatFits (Bounds.Size, value);
					targetHeight = size.Height;
					//Don't expand if expand size is smaller than current size
					if (targetHeight <= Bounds.Size.Height) {
						return;
					}
					heightBeforeExpand = Bounds.Height;
				} else {
					targetHeight = heightBeforeExpand;
				}
				expanded = value;
				ResetCachedFramesetter ();
				var box = LayoutBox.FromCenterBounds (Center, Bounds)
					.Top (0).Left (0).Right (0).Height (targetHeight);
				if (Expanding != null) {
					Expanding (this, new ExpandableTextEventArgs {
						Expanding = expanded,
						ToHeight = box.LayoutHeight
					});
				}
				Center = (CGPoint)box.LayoutCenter;
				Bounds = new CGRect (Bounds.Location, (CGSize)box.LayoutSize);
				SetNeedsDisplay ();
				if (Expanded != null) {
					Expanded (this, new ExpandableTextEventArgs {
						Expanding = expanded,
						ToHeight = box.LayoutHeight
					});
				}
			}
		}

		NSMutableAttributedString text;

		NSAttributedString AttributedText {
			get { return text; }
			set { 
				if (value == null)
					throw new ArgumentNullException ();
				text = new NSMutableAttributedString (value);

				if (font != null)
					text.AddAttribute (UIStringAttributeKey.Font, font, new NSRange (0, text.Length));
				if (textColor != null)
					text.AddAttribute (UIStringAttributeKey.ForegroundColor, textColor, new NSRange (0, text.Length));
				ResetCachedFramesetter ();
				SetNeedsLayout ();
			}
		}

		public string Text {
			get { return text.ToString (); }
			set { 
				AttributedText = new NSAttributedString (value ?? "");
			}
		}

		UIFont font;

		public UIFont Font {
			get { return font; }
			set {
				if (value == null)
					throw new ArgumentNullException ();
				font = value;
				text.AddAttribute (UIStringAttributeKey.Font, font, new NSRange (0, text.Length));
				ResetCachedFramesetter ();
				SetNeedsLayout ();
			}
		}

		UIColor textColor;

		public UIColor TextColor {
			get { return textColor; }
			set {
				if (value == null)
					throw new ArgumentNullException ();
				textColor = value;
				text.AddAttribute (UIStringAttributeKey.ForegroundColor, textColor, new NSRange (0, text.Length));
				SetNeedsDisplay ();
			}
		}

		UITextAlignment textAlignment;

		public UITextAlignment TextAlignment {
			get { return textAlignment; }
			set { 
				textAlignment = value;
				using (var style = new NSMutableParagraphStyle {
					Alignment = textAlignment
				}) {
					text.AddAttribute (UIStringAttributeKey.ParagraphStyle, style, new NSRange (0, text.Length));
				}
				ResetCachedFramesetter ();
				SetNeedsDisplay ();
			}
		}

		public CGSize SizeThatFits (CGSize size, bool expanded)
		{
			var height = expanded ? nfloat.MaxValue : size.Height;
			return AttributedText.GetBoundingRect (
				new CGSize (size.Width, height), 
				NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin, 
				null).Size;
		}

		public override CGSize SizeThatFits (CGSize size)
		{
			return SizeThatFits (size, IsExpanded);
		}

		void ResetCachedFramesetter ()
		{
			Cleanup.All (framesetter);
			framesetter = null;
			fitRange = new NSRange ();
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			ResetCachedFramesetter ();
		}

		CTFramesetter framesetter;
		NSRange fitRange;

		public override void Draw (CGRect rect)
		{
			NSRange range = new NSRange (0, text.Length);
			if (framesetter == null) {
				framesetter = new CTFramesetter (text);
				var size = framesetter.SuggestFrameSize (range, null, Bounds.Size, out fitRange);
				if (!size.IsEmpty) {
					var drawMore = range.Length > fitRange.Length;
					//TODO fix bug - if text has line break then "...More" added in incorrect position
					//Example: text 
					//line 1
					//line 2
					//Converted to
					//line 1
					//l..More
					//But should
					//line 1
					//line 2            ...More
					if (drawMore) {
						framesetter.Dispose ();
						framesetter = null;
						var start = fitRange.Location;
						var len = fitRange.Length - MoreText.Length;
						using (var visibleString = new NSMutableAttributedString ()) {
							if (start + len < text.Length && start + len > 0) {
								visibleString.Append (text.Substring (start, len));
								visibleString.Append (MoreAttributedText);
							}
							framesetter = new CTFramesetter (visibleString);
						}
					}
				} else {
					fitRange = range;
				}
			}

			using (var context = UIGraphics.GetCurrentContext ())
			using (var path = CGPath.FromRect (Bounds))
			using (var frame = framesetter.GetFrame (fitRange, path, null)) {
				context.SetFillColor (BackgroundColor.CGColor);
				context.FillRect (Bounds);
				context.SaveState ();
				context.TranslateCTM (0, Bounds.Height);
				context.ScaleCTM (1, -1);
				frame.Draw (context);
			}
		}
	}
}

