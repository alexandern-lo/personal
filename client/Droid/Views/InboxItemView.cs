using Android.Content;
using Android.Util;
using StudioMobile;
using Android.Widget;
using LiveOakApp.Droid.CustomViews;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Models;
using System;

namespace LiveOakApp.Droid.Views
{
    public class InboxItemView : CustomBindingsView
    {
        public InboxItemView(Context context) :
            base(context)
        {
            Initialize();
        }

        public InboxItemView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public InboxItemView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        InboxItemViewModel _viewModel;
        public InboxItemViewModel ViewModel 
        {
            get 
            {
                return _viewModel;
            } 
            set 
            {
                Bindings.Clear();

                _viewModel = value;

                Bindings.Property(_viewModel, _ => _.Title)
                        .To(Title.TextProperty());
                Bindings.Property(_viewModel, _ => _.Message)
                        .To(Message.TextProperty());
                Bindings.Property(_viewModel, _ => _.ReceivedTime)
                        .Convert<string>((time) => time.Date == DateTime.Today ?
                                         ServiceLocator.Instance.DateTimeService.TimeToDisplayString(time, Context) :
                                         ServiceLocator.Instance.DateTimeService.DateTimeToDisplayString(time, Context))
                        .To(Time.TextProperty());
                Bindings.Property(_viewModel, _ => _.Type)
                        .UpdateTarget((notificationType) =>
                {
                    var imgResId = GetImageResourceIdForNotificationType(notificationType.Value);
                    Icon.SetImageResource(imgResId);
                });
                Bindings.Property(_viewModel, _ => _.IsRead)
                        .UpdateTarget((isRead) =>
                {
                    if(isRead.Value) 
                    {
                        Title.SetFont("OpenSans-Regular");
                        Message.SetFont("OpenSans-Regular");
                    } else
                    {
                        Title.SetFont("OpenSans-Bold");
                        Message.SetFont("OpenSans-Bold");
                    }
                });
            }
        }

        public ImageView Icon { get; private set; }
        public CustomTextView Title { get; private set; }
        public CustomTextView Message { get; private set; }
        public CustomTextView Time { get; private set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.InboxItemLayout, this);
            Icon = FindViewById<ImageView>(Resource.Id.message_icon);
            Title = FindViewById<CustomTextView>(Resource.Id.message_title);
            Message = FindViewById<CustomTextView>(Resource.Id.message_body);
            Time = FindViewById<CustomTextView>(Resource.Id.message_time);
        }

        int GetImageResourceIdForNotificationType(InboxItemViewModel.NotificationType type) 
        {
            switch (type)
            {
                case InboxItemViewModel.NotificationType.Announcement: return ViewModel.IsRead ? Resource.Drawable.announcement_on : Resource.Drawable.announcement_off;
                case InboxItemViewModel.NotificationType.News: return ViewModel.IsRead ? Resource.Drawable.news_on : Resource.Drawable.news_off;
                case InboxItemViewModel.NotificationType.Poll: return ViewModel.IsRead ? Resource.Drawable.poll_on : Resource.Drawable.poll_off;
                default: return 0;
            }
        }
    }
}
