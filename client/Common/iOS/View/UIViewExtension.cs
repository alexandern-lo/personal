using System;
using UIKit;

namespace StudioMobile
{
	public static class UIViewExtension
	{
		public static UIImage BlurredSnapshot (this UIView view, UIColor tintColor, float blurRadius)
		{
			UIGraphics.BeginImageContextWithOptions (view.Bounds.Size, false, view.Window.Screen.Scale);
			try {
				view.DrawViewHierarchy (view.Frame, false);			
				var snapshotImage = UIGraphics.GetImageFromCurrentImageContext ();
				return snapshotImage.ApplyBlur (blurRadius:blurRadius, tintColor:tintColor, saturationDeltaFactor:1.8f, maskImage:null);

				//				return snapshotImage.ApplyLightEffect ();			
			} finally {
				UIGraphics.EndImageContext ();
			}
		}

	}
}

