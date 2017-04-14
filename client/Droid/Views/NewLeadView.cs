using Android.Content;
using Android.Support.V4.App;
using Android.Util;
using Android.Widget;

namespace LiveOakApp.Droid.Views
{
	public class NewLeadView : FrameLayout
	{
		public NewLeadView(Context context) :
			base(context)
		{
			Initialize();
		}

		public NewLeadView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public NewLeadView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

		public FragmentTabHost TabHost { get; private set; }
		public TabWidget TabWidget { get; private set; }
		public FrameLayout TabContent { get; private set; }

		void Initialize()
		{
			Inflate(Context, Resource.Layout.LeadTabsHostLayout, this);

			TabHost = FindViewById<FragmentTabHost>(Android.Resource.Id.TabHost);
			TabWidget = FindViewById<TabWidget>(Android.Resource.Id.Tabs);
			TabContent = FindViewById<FrameLayout>(Android.Resource.Id.TabContent);
		}
	}
}

