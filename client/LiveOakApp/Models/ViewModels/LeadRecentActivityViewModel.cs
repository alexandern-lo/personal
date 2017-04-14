using System;
#if __ANDROID__
using Android.Content;
#endif
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Resources;

namespace LiveOakApp.Models.ViewModels
{

    public enum LeadActionType
    {
        Unknown,
        Created,
        Updated,
        Deleted
    }
    public class LeadRecentActivityViewModel
    {
        public LeadRecentActivityDTO LeadRecentActivityDTO { get; private set; }
        public Guid? LeadUid { get { return LeadRecentActivityDTO.LeadUid; } }
        public string FirstName { get { return LeadRecentActivityDTO.FirstName; } }
        public string LastName { get { return LeadRecentActivityDTO.LastName; } }
        public string PhotoUrl { get { return LeadRecentActivityDTO.PhotoUrl; } }
        public Guid? EventUid { get { return LeadRecentActivityDTO.EventUid; } }
        public string EventName { get { return LeadRecentActivityDTO.EventName; } }
        public LeadActionType? PerformedAction { get { return LeadPerfomedActionToLeadActionType(LeadRecentActivityDTO.PerformedAction); } }
        public DateTime? PerformedAt { get { return LeadRecentActivityDTO.PerformedAt; } }
        public FileResource PhotoResource { get { return new FileResource(null, LeadRecentActivityDTO.PhotoUrl); } }

        public LeadRecentActivityViewModel(LeadRecentActivityDTO leadRecentActivityDTO)
        {
            LeadRecentActivityDTO = leadRecentActivityDTO;
        }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public string GetPerformedAtFormatted()
        {
            if (LeadRecentActivityDTO.PerformedAt == null) return "";
            var dateTime = LeadRecentActivityDTO.PerformedAt.GetValueOrDefault();
            if (dateTime.Date == DateTime.Today.Date && DateTime.Now.TimeOfDay.TotalSeconds - dateTime.TimeOfDay.TotalSeconds <= 60)
                return L10n.Localize("JustNow", "Just now");
            var minutesPast = DateTime.Now.TimeOfDay.TotalMinutes - dateTime.TimeOfDay.TotalMinutes;
            if (dateTime.Date == DateTime.Today.Date && minutesPast < 2)
                return L10n.Localize("MinuteAgo", "A minute ago");
            if (dateTime.Date == DateTime.Today.Date && minutesPast < 60)
                return string.Format(L10n.Localize("XMinutesAgo", "{0} minutes ago"), Math.Round(minutesPast, MidpointRounding.AwayFromZero));
            if (LeadRecentActivityDTO.PerformedAt.GetValueOrDefault().Date == DateTime.Today.Date)
                return L10n.Localize("TodayAtDateTitle", "Today at ") + ServiceLocator.Instance.DateTimeService.TimeToDisplayString(LeadRecentActivityDTO.PerformedAt);
            if (LeadRecentActivityDTO.PerformedAt.GetValueOrDefault().Date == DateTime.Today.AddDays(-1))
                return L10n.Localize("YesterdayAtDateTitle", "Yesterday at ") + ServiceLocator.Instance.DateTimeService.TimeToDisplayString(LeadRecentActivityDTO.PerformedAt);
            return ServiceLocator.Instance.DateTimeService.DateTimeToDisplayString(LeadRecentActivityDTO.PerformedAt);
        }
#if __ANDROID__
        public string GetPerformedAtFormatted(Context context)
        {
            if (LeadRecentActivityDTO.PerformedAt == null) return "";
            var dateTime = LeadRecentActivityDTO.PerformedAt.GetValueOrDefault();
            if (dateTime.Date == DateTime.Today.Date && DateTime.Now.TimeOfDay.TotalSeconds - dateTime.TimeOfDay.TotalSeconds <= 60)
                return L10n.Localize("JustNow", "Just now");
            var minutesPast = DateTime.Now.TimeOfDay.TotalMinutes - dateTime.TimeOfDay.TotalMinutes;
            if (dateTime.Date == DateTime.Today.Date && minutesPast < 2)
                return L10n.Localize("MinuteAgo", "A minute ago");
            if (dateTime.Date == DateTime.Today.Date && minutesPast < 60)
                return string.Format(L10n.Localize("XMinutesAgo", "{0} minutes ago"), Math.Round(minutesPast, MidpointRounding.AwayFromZero));
            if (LeadRecentActivityDTO.PerformedAt.GetValueOrDefault().Date == DateTime.Today.Date)
                return L10n.Localize("TodayAtDateTitle", "Today at ") + ServiceLocator.Instance.DateTimeService.TimeToDisplayString(LeadRecentActivityDTO.PerformedAt, context);
            if (LeadRecentActivityDTO.PerformedAt.GetValueOrDefault().Date == DateTime.Today.AddDays(-1))
                return L10n.Localize("YesterdayAtDateTitle", "Yesterday at ") + ServiceLocator.Instance.DateTimeService.TimeToDisplayString(LeadRecentActivityDTO.PerformedAt, context);
            return ServiceLocator.Instance.DateTimeService.DateTimeToDisplayString(LeadRecentActivityDTO.PerformedAt, context);
        }
#endif
        public LeadActionType LeadPerfomedActionToLeadActionType(LeadPerformedAction? leadPerformedAction)
        {
            switch (leadPerformedAction)
            {
                case LeadPerformedAction.Unknown: return LeadActionType.Unknown;
                case LeadPerformedAction.Created: return LeadActionType.Created;
                case LeadPerformedAction.Updated: return LeadActionType.Updated;
                case LeadPerformedAction.Deleted: return LeadActionType.Deleted;
                default: return LeadActionType.Unknown;
            }
        }
    }
}
