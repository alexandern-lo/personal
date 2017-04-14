using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace LiveOakApp.Droid.Views
{
    public class TabView : FrameLayout
    {
        public TabView(Context context) :
            base(context)
        {
            Initialize();
        }

        public TabView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public TabView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        public TextView Title { get; private set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.TabLayout, this);

            Title = FindViewById<TextView>(Resource.Id.tab_title);
        }
    }
}

