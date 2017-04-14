
using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Widget;

namespace LiveOakApp.Droid.Views
{
	public class CircleImageView : ImageView
	{

		public CircleImageView(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
		{
			Initialize();
		}

		public CircleImageView(Context context) :
			base(context)
		{
			Initialize();
		}

		public CircleImageView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public CircleImageView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

		void Initialize()
		{ }

		protected override void OnDraw(Canvas canvas)
		{

			if (Drawable == null)
			{
				return;
			}

			if (Width == 0 || Height == 0)
			{
				return;
			}

			Bitmap roundBitmap = null;
			Bitmap bitmap = ((BitmapDrawable)Drawable).Bitmap;

			if (bitmap != null)
			{
				roundBitmap = GetCroppedBitmap(bitmap, Width);
			}


			if (roundBitmap != null)
			{
				canvas.DrawBitmap(roundBitmap, 0, 0, null);
			}
		}

		public static Bitmap GetCroppedBitmap(Bitmap bmp, int radius)
		{

			Bitmap sbmp;

			bmp = GetSquareBitmap(bmp);


			if (bmp.Width != radius || bmp.Height != radius)
			{
				sbmp = Bitmap.CreateScaledBitmap(bmp, radius, radius, false);
			}
			else {
				sbmp = bmp;
			}

			Bitmap output = Bitmap.CreateBitmap(sbmp.Width,
												sbmp.Height, Bitmap.Config.Argb8888);
			Canvas canvas = new Canvas(output);

			Paint paint = new Paint();
			Rect rect = new Rect(0, 0, sbmp.Width, sbmp.Height);

			paint.AntiAlias = true;

			canvas.DrawCircle(sbmp.Width / 2, sbmp.Height / 2,
					sbmp.Width / 2, paint);
			paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
			canvas.DrawBitmap(sbmp, rect, rect, paint);

			return output;
		}

		private static Bitmap GetSquareBitmap(Bitmap bmp)
		{
			if (bmp == null)
			{
				return null;
			}

			int size = Math.Min(bmp.Height, bmp.Width);
			int left = (bmp.Width - size) / 2;
			int top = (bmp.Height - size) / 2;
			return Bitmap.CreateBitmap(bmp, left, top, size, size);
		}
	}
}

