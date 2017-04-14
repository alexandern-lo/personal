using Android.Content;
using Android.Content.Res;
using Android.Util;
using Android.Widget;
using Java.Lang;
using LiveOakApp.Models;
using LiveOakApp.Resources;

namespace LiveOakApp.Droid.CustomViews
{
	public class CustomEditText : EditText
	{
		public CustomEditText(System.IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		public CustomEditText(Context context) :
			base(context)
		{
		}

		public CustomEditText(Context context, IAttributeSet attrs) :
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
					Hint = L10n.Localize(string_id, comment);

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
					var fontService = ServiceLocator.Instance.FontService;

					var typeface = fontService.GetFont(Context, typefaceName);

					SetTypeface(typeface, Android.Graphics.TypefaceStyle.Normal);
				}

			}
			finally
			{
				fontableAttrs.Recycle();
			}
		}

		public CustomEditText(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
		}

	}
}

