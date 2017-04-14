using UIKit;
using System.Threading.Tasks;
using System.IO;
using Foundation;
using System.Threading;
using System;
using CoreGraphics;
using System.Drawing;

namespace StudioMobile
{
	public struct Image : IDisposable
	{
		readonly UIImage impl;

		public Image (UIImage impl)
		{
			this.impl = impl;
		}
		public void Dispose ()
		{
			impl.Dispose ();
		}
		public static implicit operator UIImage(Image image)
		{
			return image.Native;
		}

		public UIImage Native { get { return impl; } } 

		public float Width { get { return (float)impl.Size.Width; } }
		public float Height { get { return (float)impl.Size.Height; } }

		public static async Task<Image> LoadFromStream(Stream stream, CancellationToken token)
		{
			return await Task.Run (() => {
				var data = NSData.FromStream (stream);
				token.ThrowIfCancellationRequested();
				return new Image(new UIImage (data));
			});
		}
		public static Task<Image> LoadFromStream(Stream stream)
		{
			return LoadFromStream (stream, CancellationToken.None);
		}
		public Task<Image> Resize(float sx, float sy)
		{
			var impl = this.impl;
			return Task.Run(() => new Image(impl.Scale(new CGSize(sx, sy))));
		}
        public Task<Image> AspectFitInSize(float sx, float sy)
        {
            var maxRatio = Math.Min(sx / Width, sy / Height);
            if (maxRatio >= 1) return Task.FromResult(this);
            return Resize(Width * maxRatio, Height * maxRatio);
        }
		public Task Write(Stream stream)
		{
			return Write (stream, CancellationToken.None);
		}
		public async Task Write(Stream stream, CancellationToken token)
		{
			var impl = this.impl;
			await Task.Run (async () => {
				await impl.AsPNG ().AsStream().CopyToAsync(stream);
				token.ThrowIfCancellationRequested();
			});
		}
	}

	public static class ImageExtensions 
	{
		public static NSTextAttachment ToTextAttachment(this UIImage image)
		{
			return image.ToTextAttachment (RectangleF.Empty);
		}
		//NOTE - bounds.Y = 0 corresponds to baseline, positivy value => above baseline, negative values - below baseline
		public static NSTextAttachment ToTextAttachment(this UIImage image, RectangleF bounds)
		{
			var rect = bounds.IsEmpty ? new CGRect (CGPoint.Empty, new CGSize (image.CGImage.Width, image.CGImage.Height)) : (CGRect)bounds;
			return new NSTextAttachment { 
				Bounds = rect,
				Image = image
			};
		}
	}
}

