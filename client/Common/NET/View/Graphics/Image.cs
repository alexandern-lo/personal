using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace StudioMobile
{
	//WARNING not tested!
	public struct Image : IDisposable
	{
		readonly System.Drawing.Image impl;

		public Image (System.Drawing.Image impl)
		{
			this.impl = impl;
		}
		public void Dispose ()
		{
			impl.Dispose ();
		}
		public static implicit operator System.Drawing.Image(Image image)
		{
			return image.Native;
		}

		public System.Drawing.Image Native { get { return impl; } } 

		public float Width { get { return (float)impl.Size.Width; } }
		public float Height { get { return (float)impl.Size.Height; } }

		public static async Task<Image> LoadFromStream(Stream stream, CancellationToken token)
		{
			return await Task.Run (() => {
				var data = System.Drawing.Image.FromStream (stream);
				token.ThrowIfCancellationRequested();
				return new Image(data);
			});
		}
		public static Task<Image> LoadFromStream(Stream stream)
		{
			return LoadFromStream (stream, CancellationToken.None);
		}
		public Task<Image> Resize(float sx, float sy)
		{
			var impl = this.impl;
			return Task.Run(() => {
				int isx = (int)Math.Round (sx), isy = (int)Math.Round(sy);
				var newImage = new Bitmap(isx, isy);
				using (var graphics = Graphics.FromImage(newImage))
				{
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					graphics.DrawImage(impl, 0, 0, isx, isy);
				}
				return new Image(newImage);
			});
		}
		public Task Write(Stream stream)
		{
			return Write (stream, CancellationToken.None);
		}
		public async Task Write(Stream stream, CancellationToken token)
		{
			var impl = this.impl;
			await Task.Run (() => {
				impl.Save(stream, ImageFormat.Png);
				token.ThrowIfCancellationRequested();
			});
		}
	}
}

