using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using StudioMobile;
using ServiceStack;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Services;
using LiveOakApp.Resources;
using LiveOakApp.Models.Data.Entities;
using System.Threading;
using System;

namespace LiveOakApp.Models.ViewModels
{
    public class LeadsViewModel : DataContext
    {
        public static readonly string FAKE_EVENT_UID_ANY = "FAKE_EVENT_UID_ANY";

        LeadsService leadsService;
        EventsService eventsService;

        public LeadsViewModel()
        {
            leadsService = ServiceLocator.Instance.LeadsService;
            eventsService = ServiceLocator.Instance.EventsService;

            SearchCommand = new Command
            {
                Action = SearchAction
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

            TrackResourceSentCommand = new AsyncCommand
            {
                Action = TrackResourceSent
            };

            LoadLeadsCommand = new CachableCommandViewModel<List<LeadDTO>>(leadsService.LeadsRequest);

            Bindings.Property(LoadLeadsCommand, _ => _.Result).UpdateTarget((a) =>
            {
                GetLeads().Ignore();
                LOG.Debug("received leads: network: {0}, all: {1}", a.Value?.Count, AllLeads?.Count);
            });
            Bindings.Bind();

            leadsService.CleanupOldOrphanedFiles().Ignore();
        }

        #region Leads

        public async Task GetLeads()
        {
            var leads = await leadsService.GetAllLeads(null);
            AllLeads = leads?.ConvertAll((Lead input) => new LeadViewModel(input));
        }

        public ObservableList<LeadViewModel> Leads { get; } = new ObservableList<LeadViewModel>();

        public CachableCommandViewModel<List<LeadDTO>> LoadLeadsCommand { get; private set; }

        List<LeadViewModel> allLeads;
        List<LeadViewModel> AllLeads
        {
            get { return allLeads; }
            set
            {
                allLeads = value;
                SearchCommand.Execute();
            }
        }

        #endregion

        #region Search

        EventViewModel @event = new EventViewModel(new EventDTO() { Name = L10n.Localize("LeadsAnyEvent", "AnyEvent"), UID = FAKE_EVENT_UID_ANY });
        public EventViewModel Event
        {
            get { return @event; }
            set
            {
                if (!Equals(value, Event))
                {
                    @event = value;
                    RaisePropertyChanged();
                    SearchCommand.Execute();
                }
            }
        }

        string searchText = "";
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                SearchCommand.Execute();
                RaisePropertyChanged();
            }
        }

        public Command SearchCommand { get; private set; }

        void SearchAction(object param)
        {
            var filteredLeads = FilterLeads(AllLeads, Event?.UID, SearchText);
            filteredLeads = filteredLeads.OrderByDescending(_ => _.LastEditDate);
            LOG.Debug("filtered leads: {0}", filteredLeads.Count());
            Leads.Reset(filteredLeads);
        }

        public IEnumerable<LeadViewModel> FilterLeads(List<LeadViewModel> allLeads, string eventUid, string searchText)
        {
            if (allLeads == null) return new List<LeadViewModel>();

            var leads = allLeads.AsEnumerable();
            if (eventUid != FAKE_EVENT_UID_ANY)
            {
                leads = leads.Where(_ => _.EventUID == eventUid);
            }
            if (!searchText.IsNullOrEmpty())
            {
                var searchLower = searchText.ToLowerInvariant();
                leads = leads.Where(lead =>
                {
                    var fields = new List<string> { lead.FullName, lead.JobInfo };
                    return fields.Any(field =>
                    {
                        if (field.IsNullOrEmpty()) return false;
                        return field.ToLowerInvariant().Contains(searchLower);
                    });
                });
            }
            return leads;
        }

        #endregion

        #region Events

        public ObservableList<EventViewModel> Events { get; } = new ObservableList<EventViewModel>();

        public AsyncCommand LoadEventsCommand { get; private set; }

        async Task LoadEventsAction(object arg)
        {
            await eventsService.LoadEventsIfNeeded(LoadEventsCommand.Token);
            Events.Reset(eventsService.Events.ConvertAll((EventDTO input) => new EventViewModel(input)));
        }

        bool CanExecuteLoadEvents(object arg)
        {
            return !LoadEventsCommand.IsRunning;
        }

        public Command SetEventCommand { get; private set; }

        void SetEventAction(object eventObj)
        {
            Event = (EventViewModel)eventObj;
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
                if (ex.Message.Contains("specified hostname could not be found"))
                    throw new Exception(L10n.Localize("InvalidQRCodeErrorMessage", "This QR code doesn't contain vCard"));
                throw;
            }
        }
        #endregion

        #region Resources

        public AsyncCommand TrackResourceSentCommand { get; private set; }

        async Task TrackResourceSent(object resourcesGuids)
        {
            var resources = resourcesGuids as List<Guid>;
            await ServiceLocator.Instance.ApiService.SendResourceSentTrackingRequest(resources, TrackResourceSentCommand.Token);
        }

        #endregion    
    }
}

