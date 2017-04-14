using System;
using System.Collections.Generic;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.Models.ViewModels
{
    public class EventsViewModel : DataContext
    {
        public enum SectionHeader
        {
            None,
            Now,
            Upcoming,
            Recent
        }

        public class Section : ObservableList<EventViewModel>
        {
            public Section(IEnumerable<EventViewModel> collection) : base(collection)
            {
            }
            public Section()
            {
            }
            public SectionHeader Header { get; set; }
        }


        public ObservableList<Section> Sections { get; } = new ObservableList<Section>();

        List<EventViewModel> events;
        List<EventViewModel> Events
        {
            get { return events; }
            set
            {
                events = value;
                RaisePropertyChanged();
                var newSections = GroupEventsInSections(events);
                Sections.Reset(newSections);
            }
        }

        public EventsViewModel(bool isCreatingForLeadEventSelection = false)
        {
            var service = ServiceLocator.Instance.EventsService;
            var eventDTOs = isCreatingForLeadEventSelection ? service.SelectableEvents : service.Events;
            Events = eventDTOs?.ConvertAll((EventDTO input) => new EventViewModel(input));

            LoadEventsCommand = new CachableCommandViewModel<List<EventDTO>>(isCreatingForLeadEventSelection ? service.SelectableEventsRequest : service.EventsRequest);

            Bindings.Property(LoadEventsCommand, _ => _.Result).UpdateTarget((a) =>
            {
                LOG.Debug("received events: {0}", a.Value.Count);
                Events = a.Value.ConvertAll((EventDTO input) => new EventViewModel(input));
            });
            Bindings.Bind();
        }

        public CachableCommandViewModel<List<EventDTO>> LoadEventsCommand { get; private set; }

        public Section GetEventSection(EventViewModel data)
        {
            foreach (var section in Sections)
            {
                if (section.Contains(data))
                {
                    return section;
                }
            }
            return null;
        }

        static List<Section> GroupEventsInSections(List<EventViewModel> events)
        {
            if (events == null) return new List<Section>();

            var pastEvents = new List<EventViewModel>();
            var currentEvents = new List<EventViewModel>();
            var futureEvents = new List<EventViewModel>();
            foreach (EventViewModel e in events)
            {
                if (e.EndDate.HasValue && e.EndDate.Value.Date < DateTime.Now.Date)
                {
                    pastEvents.Add(e);
                }
                else if (e.StartDate.HasValue && e.StartDate.Value.Date > DateTime.Now.Date)
                {
                    futureEvents.Add(e);
                }
                else {
                    currentEvents.Add(e);
                }
            }
            pastEvents.Sort(EventComparison);
            currentEvents.Sort(EventComparison);
            futureEvents.Sort(EventComparison);

            var newSections = new List<Section>();
            if (currentEvents.Count > 0)
            {
                var section = new Section(currentEvents);
                section.Header = SectionHeader.Now;
                newSections.Add(section);
            }
            if (futureEvents.Count > 0)
            {
                var section = new Section(futureEvents);
                section.Header = SectionHeader.Upcoming;
                newSections.Add(section);
            }
            if (pastEvents.Count > 0)
            {
                var section = new Section(pastEvents);
                section.Header = SectionHeader.Recent;
                newSections.Add(section);
            }
            return newSections;
        }

        static int EventComparison(EventViewModel e1, EventViewModel e2)
        {
            return Nullable.Compare(e1.StartDate, e2.StartDate);
        }
    }
}
