using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using StudioMobile;

namespace LiveOakApp.Droid.Views
{
    public class ErrorMessageView : LinearLayout
    {
        public ErrorMessageView(Context context) :
            base(context)
        {
            Initialize();
        }

        public ErrorMessageView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public ErrorMessageView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        TextView MessageView { get; set; }
        Button ActionButton { get; set; }

        public string Message 
        { 
            get { return MessageView.Text; }
            set { MessageView.Text = value; }
        }

        void Initialize()
        {
            Orientation = Orientation.Vertical;

            SetGravity(GravityFlags.Center);

            var pad = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 4f, Context.Resources.DisplayMetrics);

            SetPadding(pad,pad,pad,pad);

            Inflate(Context, Resource.Layout.ErrorMessageViewLayout, this);

            MessageView = FindViewById<TextView>(Resource.Id.error_message);
            ActionButton = FindViewById<Button>(Resource.Id.action_button);
        }

        public void SetShowsButton(bool shows)
        {
            ActionButton.Visibility = shows ? ViewStates.Visible : ViewStates.Gone;
        }

        public EventHandlerSource<Button> ActionButtonClickTarget
        {
            get { return ActionButton.ClickTarget(); }
        }

        public IProperty<string> MessageProperty
        {
            get { return MessageView.TextProperty(); }
        }
    }
}
