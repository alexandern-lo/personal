using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using LiveOakApp.Models;
using LiveOakApp.Resources;

namespace LiveOakApp.Droid.CustomViews
{
    public class CustomTextView : TextView
    {
        public CustomTextView(System.IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer){}
        public CustomTextView(Context context) :
            base(context){}

        Rect bounds;

        bool cropFontPaddings;
        bool CropFontPaddings 
        { 
            get
            {
                return cropFontPaddings;
            }
            set
            {
                cropFontPaddings = value;
                RequestLayout();
            }
        }

        public CustomTextView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            TypedArray localizableAttrs = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.Localizable, 0, 0);
            try
            {
                var string_id = localizableAttrs.GetString(Resource.Styleable.Localizable_localized_string_id);
                var comment = localizableAttrs.GetString(Resource.Styleable.Localizable_comment);
                if (string_id != null && comment == null)
                {
                    throw new IllegalArgumentException("Missing xml arg \"comment\"");
                }
                if (string_id != null)
                    Text = L10n.Localize(string_id, comment);
            }
            finally
            {
                localizableAttrs.Recycle();
            }

            TypedArray fontableAttrs = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.Fontable, 0, 0);
            try
            {
                var typefaceName = fontableAttrs.GetString(Resource.Styleable.Fontable_custom_font);
                if (typefaceName != null)
                {
                    SetFont(typefaceName);
                }
                var underline = fontableAttrs.GetBoolean(Resource.Styleable.Fontable_underline, false);
                if (underline)
                    PaintFlags = PaintFlags | PaintFlags.UnderlineText;
                CropFontPaddings = fontableAttrs.GetBoolean(Resource.Styleable.Fontable_cropFontPaddings, false);
                if (CropFontPaddings)
                    bounds = new Rect();
            }
            finally
            {
                fontableAttrs.Recycle();
            }
        }

        public CustomTextView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {}

        // For correct width measurement of multiline text view. (for wrap_content case)
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            if(CropFontPaddings)
            {
                Paint.GetTextBounds(Text.ToCharArray(), 0, Text.Length, bounds);
                SetMeasuredDimension(
                    MeasureSpec.MakeMeasureSpec(bounds.Width() + CompoundPaddingLeft + CompoundPaddingRight + 1, MeasureSpec.GetMode(widthMeasureSpec)),
                    MeasureSpec.MakeMeasureSpec(bounds.Height() + 1, MeasureSpec.GetMode(heightMeasureSpec))
                );
                return;
            }
            if (MeasureSpec.GetMode(widthMeasureSpec).Equals(MeasureSpecMode.AtMost) && Layout != null)
            {
                int width = (int)Math.Ceil(GetMaxLineWidth(Layout))
                                     + CompoundPaddingLeft + CompoundPaddingRight;
                int height = MeasuredHeight;
                SetMeasuredDimension(width, height);
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            if(CropFontPaddings)
            {
                Paint.AntiAlias = true;
                Paint.Color = new Color(CurrentTextColor);
                canvas.DrawText(Text.ToCharArray(), 0, Text.Length, CompoundPaddingLeft, bounds.Height(), Paint);
                return;
            }
            base.OnDraw(canvas);
        }

        float GetMaxLineWidth(Layout layout)
        {
            float max_width = 0.0f;
            int lines = layout.LineCount;
            for (int i = 0; i < lines; i++)
            {
                if (layout.GetLineWidth(i) > max_width)
                {
                    max_width = layout.GetLineWidth(i);
                }
            }
            return max_width;
        }

        public void SetFont(string typefaceName) 
        {
            var fontService = ServiceLocator.Instance.FontService;
            var typeface = fontService.GetFont(Context, typefaceName);
            SetTypeface(typeface, TypefaceStyle.Normal);
        }

    }
}

