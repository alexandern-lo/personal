using Android.Content;
using Android.Util;
using Android.Widget;
using LiveOakApp.Droid.CustomViews;

namespace LiveOakApp.Droid.Views
{
    public class QualifyAnswerView : FrameLayout
    {
        public QualifyAnswerView(Context context) :
            base(context)
        {
            Initialize();
        }

        public QualifyAnswerView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public QualifyAnswerView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        public CustomTextView AnswerTitle { get; private set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.AnswerItemLayout, this);

            AnswerTitle = FindViewById<CustomTextView>(Resource.Id.answer_title);
        }

    }
}

