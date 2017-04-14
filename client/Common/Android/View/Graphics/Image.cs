using System;
using Android.Graphics;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace StudioMobile
{
	//NOT TESTED!!! be ready to fix below code
	public struct Image : IDisposable
	{
		readonly Bitmap impl;

		public Image (Bitmap impl)
		{
			this.impl = impl;
		}
		public void Dispose ()
		{
            if(impl != null)
            {
                impl.Recycle();
                impl.Dispose();
            }
        }

		public static implicit operator Bitmap(Image image)
		{
			return image.Native;
		}

		public Bitmap Native { get { return impl; } } 

		public float Width { get { return (float)impl.Width; } }
		public float Height { get { return (float)impl.Height; } }

		public static async Task<Image> LoadFromStream(Stream stream, CancellationToken token)
		{
			return await Task.Run (() => {
				var data = BitmapFactory.DecodeStream(stream);
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
			int isx = (int)Math.Round (sx), isy = (int)Math.Round(sy);
			return Task.Run(() => new Image(Bitmap.CreateScaledBitmap(impl, isx, isy, true)));
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
            await Task.Run(() => impl.Compress(Bitmap.CompressFormat.Jpeg, 80, stream));
		}
	}
}

