using Android.Content;
using Android.Util;
using Android.Widget;

namespace LiveOakApp.Droid.Views
{
    public class AttendeeInfoItemView : FrameLayout
    {
        public AttendeeInfoItemView(Context context) :
            base(context)
        {
            Initialize();
        }

        public AttendeeInfoItemView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public AttendeeInfoItemView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        public TextView InfoTitle { get; private set; }
        public TextView InfoValue { get; private set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.AttendeeInfoItemLayout, this);

            InfoTitle = FindViewById<TextView>(Resource.Id.info_title);
            InfoValue = FindViewById<TextView>(Resource.Id.info_value);
        }
    }
}

