using System;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Resources;

namespace LiveOakApp.Models.ViewModels
{
    public class EventViewModel : DataContext
    {
        public EventDTO EventDTO { get; private set; }

        public bool NotRealEvent { get; private set; }

        public string UID { get { return EventDTO.UID; } }
        public string Name { get { return EventDTO.Name; } }
        public string VenueName { get { return EventDTO.VenueName; } }
        public DateTime? StartDate { get { return EventDTO.StartDate; } }
        public DateTime? EndDate { get { return EventDTO.EndDate; } }
        public string Address { get { return EventDTO.Address; } }
        public string ZipCode { get { return EventDTO.ZipCode; } }
        public string City { get { return EventDTO.City; } }
        public string State { get { return EventDTO.State; } }
        public string Country { get { return EventDTO.Country; } }
        public string Industry { get { return EventDTO.Industry; } }
        public string WebsiteUrl { get { return EventDTO.WebsiteUrl; } }
        public string LogoUrl { get { return EventDTO.LogoUrl; } }
        public bool IsReccuring { get { return EventDTO.IsReccuring; } }

        public EventType TypeEnum
        {
            get
            {
                switch (EventDTO.Type)
                {
                    case "personal":
                        return EventType.Personal;
                    case "conference":
                        return EventType.Conference;
                    default:
                        return EventType.Unknown;
                }
            }
        }

        public string Type
        {
            get
            {
                switch (TypeEnum)
                {
                    case EventType.Personal:
                        return L10n.Localize("EventTypePersonal", "Personal event");
                    case EventType.Conference:
                        return L10n.Localize("EventTypeConference", "Conference");
                    case EventType.Unknown:
                        return null;
                }
                throw new ArgumentOutOfRangeException(EventDTO.Type, "unknown type");
            }
        }

        public EventViewModel(EventDTO eventDTO)
        {
            EventDTO = eventDTO;
        }

        public static EventViewModel CreateAnyEvent()
        {
            var eventDto = new EventDTO
            {
                Name = L10n.Localize("LeadsAnyEvent", "All Leads"),
                UID = LeadsViewModel.FAKE_EVENT_UID_ANY
            };
            return new EventViewModel(eventDto)
            {
                NotRealEvent = true
            };
        }

        public static EventViewModel CreateLoadingEvent(string eventUID)
        {
            var eventDto = new EventDTO
            {
                Name = string.Format(L10n.Localize("LeadsEventNumberFormat", "Event #{0}"), eventUID),
                UID = eventUID
            };
            return new EventViewModel(eventDto)
            {
                NotRealEvent = true
            };
        }

        public string EventToJson()
        {
            return ServiceLocator.Instance.JsonService.Serialize(EventDTO);
        }

        public static EventViewModel JsonToEvent(string @event)
        {
            return new EventViewModel(ServiceLocator.Instance.JsonService.Deserialize<EventDTO>(@event));
        }

        public override int GetHashCode()
        {
            return UID.GetHashCode();
        }

        public bool IsSameUID(EventViewModel other)
        {
            return UID.Equals(other?.UID);
        }

        public enum EventType
        {
            Unknown,
            Personal,
            Conference
        }
    }
}
