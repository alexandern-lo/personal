using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace LiveOakApp.Droid.Views
{
    public class StartupView : FrameLayout
    {
        public StartupView(Context context) : base(context)
        {
            Initialize();
        }

        public StartupView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public StartupView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        public TextView ProgressTextView { get; private set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.StartupView, this);
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            SetBackgroundResource(Resource.Drawable.login_bg);

            ProgressTextView = FindViewById<TextView>(Resource.Id.startup_progress_textview);
        }
    }
}
