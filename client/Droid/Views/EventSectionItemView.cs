using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace LiveOakApp.Droid.Views
{
	public class EventSectionItemView : FrameLayout
	{
		public EventSectionItemView(Context context) :
			base(context)
		{
			Initialize();
		}

		public EventSectionItemView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public EventSectionItemView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

		public TextView HeaderText
		{
			get; private set;
		}

		public View Divider { get; private set; }

		void Initialize()
		{
			Inflate(Context, Resource.Layout.SectionHeader, this);
			HeaderText = FindViewById<TextView>(Resource.Id.section_header);
			Divider = FindViewById(Resource.Id.bot_divider);
		}
	}
}

