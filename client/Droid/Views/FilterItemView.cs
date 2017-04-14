using Android.Content;
using Android.Util;
using Android.Widget;
using Com.Kyleduo.Switchbutton;

namespace LiveOakApp.Droid.Views
{
	public class FilterItemView : FrameLayout
	{
		public FilterItemView(Context context) :
			base(context)
		{
			Initialize();
		}

		public FilterItemView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public FilterItemView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

		public SwitchButton Switch
		{
			get; private set;
		}

		public TextView OptionName
		{
			get; private set;
		}

		void Initialize()
		{
			Inflate(Context, Resource.Layout.SwitchLayout, this);

			OptionName = FindViewById<TextView>(Resource.Id.OptionTitle);
			Switch = FindViewById<SwitchButton>(Resource.Id.OptionSwitch);
		}
	}
}

