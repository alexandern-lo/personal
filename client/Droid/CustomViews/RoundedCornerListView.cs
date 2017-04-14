using Android.Content.Res;
using Android.Graphics;
using Android.Widget;

namespace LiveOakApp.Droid.CustomViews
{
    public class RoundedCornerListView : ListView
    {
        public RoundedCornerListView(System.IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public RoundedCornerListView(Android.Content.Context context) : base(context)
        {
        }

        public float CornerRadius { get; set; }

        public RoundedCornerListView(Android.Content.Context context, Android.Util.IAttributeSet attrs) : base(context, attrs)
        {
            TypedArray a = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.RoundedCornerListView, 0, 0);

            try
            {
                CornerRadius = a.GetDimensionPixelSize(Resource.Styleable.RoundedCornerListView_clipCornerRadius, 0);
                SetClipChildren(true);
            }
            finally
            {
                a.Recycle();
            }
        }

        public RoundedCornerListView(Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public RoundedCornerListView(Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        private Bitmap bitmap;
        private Shader shader;
        private Paint paint;
        private Canvas tempCanvas;

        private bool willBeDrawn;

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);

            willBeDrawn = !(MeasuredHeight == 0 || MeasuredWidth == 0);

            if (!willBeDrawn)
                return;

            if (bitmap == null || bitmap.Height != MeasuredHeight || bitmap.Width != MeasuredWidth || shader == null || paint == null || tempCanvas == null)
            {
                bitmap = Bitmap.CreateBitmap(MeasuredWidth, MeasuredHeight, Bitmap.Config.Argb8888);
                shader = new BitmapShader(bitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);
                paint = new Paint(PaintFlags.AntiAlias);
                tempCanvas = new Canvas(bitmap);
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            //var clipPath = new Path();

            //clipPath.AddRoundRect(new RectF(canvas.ClipBounds), CornerRadius, CornerRadius, Path.Direction.Cw);

            //canvas.ClipPath(clipPath); 
            if (!willBeDrawn)
                return;
            
            base.OnDraw(tempCanvas);

            paint.SetShader(shader);

            canvas.DrawRoundRect(new RectF(canvas.ClipBounds), CornerRadius, CornerRadius, paint);
        }

        protected override void DispatchDraw(Canvas canvas)
        {
            if(willBeDrawn)
                base.DispatchDraw(tempCanvas);
        }
    }
}

