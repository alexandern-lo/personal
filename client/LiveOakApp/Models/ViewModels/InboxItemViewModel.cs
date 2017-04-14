using System;
using StudioMobile;

namespace LiveOakApp.Models.ViewModels
{
    public class InboxItemViewModel : DataContext
    {
        public enum NotificationType
        {
            Announcement,
            Poll,
            News
        };

        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime ReceivedTime { get; set; }
        public bool IsRead { get; set; }

        string typeImageName;
        public string TypeImageName
        {
            get
            {
                switch (Type)
                {
                    case NotificationType.Announcement:
                        {
                            if (IsRead) typeImageName = "announcement_off";
                            else typeImageName = "announcement_on";
                            break;
                        }
                    case NotificationType.News:
                        {
                            if (IsRead) typeImageName = "news_off";
                            else typeImageName = "news_on";
                            break;
                        }
                    case NotificationType.Poll:
                        {
                            if (IsRead) typeImageName = "poll_off";
                            else typeImageName = "poll_on";
                            break;
                        }
                }
                return typeImageName;
            }
        }
    }
}
