using Android.Content;
using Android.Util;
using Android.Widget;
using LiveOakApp.Droid.CustomViews;

namespace LiveOakApp.Droid.Views
{
    public class QualifyStateView : FrameLayout
    {
        public QualifyStateView(Context context) :
            base(context)
        {
            Initialize();
        }

        public QualifyStateView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public QualifyStateView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        public ImageView StateIcon { get; private set; }
        public CustomTextView StateTitle { get; private set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.QualifyStateLayout, this);

            StateIcon = FindViewById<ImageView>(Resource.Id.state_icon);
            StateTitle = FindViewById<CustomTextView>(Resource.Id.state_title);
        }
    }
}

