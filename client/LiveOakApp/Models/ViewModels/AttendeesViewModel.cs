using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Services;
using System.Threading;
using LiveOakApp.Resources;

namespace LiveOakApp.Models.ViewModels
{
    public class AttendeesViewModel : DataContext
    {
        AttendeesService attendeesService;
        EventsService eventsService;
        AttendeesFiltersService filtersService;

        public AttendeesViewModel(EventViewModel @event)
        {
            attendeesService = ServiceLocator.Instance.AttendeesService;
            eventsService = ServiceLocator.Instance.EventsService;
            filtersService = ServiceLocator.Instance.AttendeesFiltersService;

            SearchCommand = new AsyncCommand
            {
                Action = SearchAction,
                Delay = TimeSpan.FromMilliseconds(1000)
            };
            LoadNextPageCommand = new AsyncCommand
            {
                Action = LoadNextPageAction,
                CanExecute = CanExecuteLoadNextPage
            };

            LoadEventsCommand = new AsyncCommand
            {
                Action = LoadEventsAction,
                CanExecute = CanExecuteLoadEvents
            };
            SetEventCommand = new Command
            {
                Action = SetEventAction
            };

            _event = @event;

            filtersService.CurrentEvent = @event.EventDTO;
            SetNewAttendees(filtersService.CachedAttendees);
        }

        public bool AreFiltersEnabled { get { return filtersService.IsFiltersEnabled; } }

        public ObservableList<EventViewModel> Events = new ObservableList<EventViewModel>();

        public ObservableList<AttendeeViewModel> Persons = new ObservableList<AttendeeViewModel>();

        EventViewModel _event;
        public EventViewModel Event
        {
            get { return _event; }
            set
            {
                if (!Equals(value, Event))
                {
                    _event = value;
                    RaisePropertyChanged();
                    filtersService.CurrentEvent = _event.EventDTO;
                    SearchCommand.ExecuteWithoutDelay(null).Ignore();
                }
            }
        }

        public string SearchText
        {
            get { return filtersService.SearchText; }
            set
            {
                filtersService.SearchText = value;
                RaisePropertyChanged();
                SearchCommand.Execute();
            }
        }

        void SetNewAttendees(List<AttendeeDTO> attendees)
        {
            if (attendees.IsNullOrEmpty())
            {
                Persons.Clear();
                return;
            }
            var newAttendeeViewModels = attendees.Select((AttendeeDTO input) => new AttendeeViewModel(input));
            Persons.Reset(newAttendeeViewModels);
        }

        void AppendAttendees(List<AttendeeDTO> attendees)
        {
            if (attendees.IsNullOrEmpty())
            {
                return;
            }
            var newAttendeeViewModels = attendees.Select((AttendeeDTO input) => new AttendeeViewModel(input));
            Persons.AddRange(newAttendeeViewModels);
        }

        #region Attendees Search

        public AsyncCommand SearchCommand { get; private set; }

        async Task SearchAction(object param)
        {
            LoadNextPageCommand.CancelCommand.Execute();
            var @event = Event;
            var newAttendees = await attendeesService.SearchAttendees(SearchCommand.Token);
            if (Equals(@event, Event))
            {
                SetNewAttendees(newAttendees);
            }
        }

        public void ExecuteSearchIfNeeded()
        {
            if (filtersService.CachedAttendees == null)
            {
                SearchCommand.ExecuteWithoutDelay(null).Ignore();
            }
        }

        #endregion

        #region Attendees Load Next Page

        public AsyncCommand LoadNextPageCommand { get; private set; }

        async Task LoadNextPageAction(object param)
        {
            var @event = Event;
            var newAttendees = await attendeesService.LoadNextAttendeesPage(LoadNextPageCommand.Token);
            if (Equals(@event, Event))
            {
                AppendAttendees(newAttendees);
            }
        }

        bool CanExecuteLoadNextPage(object param)
        {
            return !LoadNextPageCommand.IsRunning && attendeesService.CanLoadNextPage();
        }

        public void LoadNextPageIfNeeded(int attendeeIndex)
        {
            var pageSize = AttendeesService.PageSize;
            if (attendeeIndex > pageSize / 2 + attendeesService.CurrentPageIndex() * pageSize)
            {
                LoadNextPageCommand.Execute();
            }
        }

        #endregion

        #region Load Events

        public AsyncCommand LoadEventsCommand { get; private set; }

        async Task LoadEventsAction(object arg)
        {
            await eventsService.LoadEventsIfNeeded(LoadEventsCommand.Token);
            var eventViewModels = eventsService.Events.Select(eventDTO => new EventViewModel(eventDTO));
            Events.Reset(eventViewModels);
        }

        bool CanExecuteLoadEvents(object arg)
        {
            return !LoadEventsCommand.IsRunning;
        }

        #endregion

        #region Set Event

        public Command SetEventCommand { get; private set; }

        void SetEventAction(object @event)
        {
            Event = (EventViewModel)@event;
        }

        #endregion

        #region QRScanner

        public async Task<byte[]> TryDownloadVCard(string uri, CancellationToken? cancellationToken)
        {
            try
            {
                return await ServiceLocator.Instance.ApiService.DownloadSmallFileToMemory(uri, cancellationToken);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Cannot write more bytes to the buffer than the configured maximum buffer"))
                    throw new Exception(L10n.Localize("TooLargeVCardFileException", "File is too large for a vCard"));
                else if (ex.Message.Contains("specified hostname could not be found"))
                    throw new Exception(L10n.Localize("InvalidQRCodeErrorMessage", "This QR code doesn't contain vCard"));
                else
                    throw;
            }
        }
        #endregion
    }
}
