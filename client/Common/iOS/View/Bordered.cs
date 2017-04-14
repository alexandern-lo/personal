using System;
using UIKit;
using System.Diagnostics;
using SL4N;

namespace StudioMobile
{
	public class Bordered : UIView
	{
		static readonly ILogger LOG = LoggerFactory.GetLogger<Bordered>();

		UIView view;

		public Bordered(UIView view) : this()
		{
			this.view = view;
			view.BackgroundColor = UIColor.Clear;
			AddSubview (view);
		}

		public Bordered()
		{
			LayoutMargins = UIEdgeInsets.Zero;
			CornerRadius = 15;
			BorderWidth = 1;
			BorderColor = UIColor.Black;
			base.ClipsToBounds = true;
		}

		public override void LayoutSubviews ()
		{
			view.Frame = this.LayoutBox ()
				.Top (LayoutMargins.Top)
				.Left (Layer.CornerRadius + LayoutMargins.Left)
				.Bottom (LayoutMargins.Bottom)
				.Right (Layer.CornerRadius + LayoutMargins.Right);
		}

		public override bool ClipsToBounds {
			get {
				return base.ClipsToBounds;
			}
			set {
				LOG.Warn ("Warning: Attempt to change Bordered view ClipToBounds ignored. Value = {0}", value);
			}
		}

		public UIView View { get { return view; } }

		public float CornerRadius {
			get { return (float)Layer.CornerRadius; }
			set { Layer.CornerRadius = value; }
		}

		public float BorderWidth {
			get { return (float)Layer.BorderWidth; }
			set { Layer.BorderWidth = value; }
		}

		public UIColor BorderColor {
			get { return new UIColor(Layer.BorderColor); }
			set {
				Layer.BorderColor = value != null ? value.CGColor : null;
			}
		}
	}

	public class Bordered<T> : Bordered where T : UIView, new()
	{
		public Bordered () : base(new T())
		{
		}

		public Bordered(T view) : base(view) {}

		public new T View { get { return base.View as T; } }
	}

}

