using System;
using CoreGraphics;
using ObjCRuntime;
using System.Runtime.InteropServices;
using UIKit;

namespace StudioMobile
{
	public static class GraphicsExtensions
	{
		public unsafe static CGPath CopyByTransformingPath(this CGPath path, CGAffineTransform transform)
		{
			var handle = CGPathCreateCopyByTransformingPath (path.Handle, &transform);
			return new CGPath (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe static extern IntPtr CGPathCreateCopyByTransformingPath (
			IntPtr path,
			CGAffineTransform *transform
		); 


		// Prevent +1 on values that are slightly too big (e.g. 24.000001).
		const float EPSILON = 0.01f;

		static CGSize RoundImageSize (CGSize size)
		{
			return new CGSize (
				(float)Math.Ceiling (size.Width - EPSILON),
				(float)Math.Ceiling (size.Height - EPSILON));
		}

		public static void RenderInContext (this CGPath path, CGContext context, FontIconAppearance appearance)
		{			
			context.AddPath (path);
			var cgColors = appearance.CGColors;
			if (appearance.Colors.Length > 1) {
				context.SaveState ();
				context.Clip ();
				var bounds = path.BoundingBox;
				context.RenderGradientInRect (bounds, appearance.CGColors);
				context.RestoreState ();
			} else {
				context.SetFillColor (cgColors [0]);
				context.FillPath ();
			}
			context.AddPath (path);

			if (appearance.StrokeColor.Native != null && appearance.StrokeWidth > 0.0f) {
				context.SetStrokeColor (appearance.StrokeColor.CGColor);
				context.SetLineWidth (appearance.StrokeWidth);
				context.StrokePath ();
			}
		}

		public static void RenderGradientInRect (this CGContext context, CGRect bounds, CGColor[] colors)
		{
			var n = colors.Length;
			nfloat[] locations = new nfloat[n];
			for (var i = 0; i < n; i++) {
				locations [i] = (nfloat)i / (n - 1);
			}
			using (var gradient = new CGGradient (null, colors, locations)) {
				var topLeft = new CGPoint (bounds.GetMinX (), bounds.GetMinY ());
				var bottomLeft = new CGPoint (bounds.GetMinX (), bounds.GetMaxY ());
				context.DrawLinearGradient (gradient, topLeft, bottomLeft, 0);
			}
		}

	}
}

