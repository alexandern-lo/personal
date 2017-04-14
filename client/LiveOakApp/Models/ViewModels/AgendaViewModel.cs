using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Services;
using StudioMobile;

namespace LiveOakApp.Models.ViewModels
{
    public class AgendaViewModel : DataContext
    {
        public class AgendaDate : List<AgendaSection>
        {
            public AgendaDate(AgendaItemViewModel agendaItem) : base()
            {
                this.Add(new AgendaSection(agendaItem));
                this.Date = agendaItem.Date;
            }
            public DateTime Date { get; private set; }
        }

        public class AgendaSection : ObservableList<AgendaItemViewModel>
        {
            public AgendaSection(AgendaItemViewModel agendaItem) : base()
            {
                this.Add(agendaItem);
                this.Location = agendaItem.Location;
                this.LocationUrl = agendaItem.LocationUrl;
            }
            public string Location { get; private set; }
            public string LocationUrl { get; private set; }
        }

        AgendaService agendaService;

        public ObservableList<AgendaDate> AgendaDates = new ObservableList<AgendaDate>();
        public ObservableList<AgendaSection>  CurrentAgendaSections = new ObservableList<AgendaSection>();

        private DateTime _currentAgendaDate;
        public DateTime CurrentAgendaDate
        {
            get { return _currentAgendaDate; }
            set
            {
                _currentAgendaDate = value;
                CurrentAgendaSections.Clear();

                var newAgendaSections = AgendaDates.FirstOrDefault((AgendaDate agendaDate) => EqualDays(agendaDate.Date, value));
                CurrentAgendaSections.Reset(newAgendaSections);
                RaisePropertyChanged(() => CurrentAgendaDate );
            }
        }

        public AgendaViewModel(EventViewModel @event)
        {
            agendaService = ServiceLocator.Instance.AgendaService;
            agendaService.CurrentEvent = @event.EventDTO;

            LoadAgendaCommand = new AsyncCommand
            {
                Action = LoadAgendaAction
            };
        }

        public AsyncCommand LoadAgendaCommand { get; private set; }

        async Task LoadAgendaAction(object param)
        {
            var newAgenda = await agendaService.GetAgendaItems(LoadAgendaCommand.Token);
            SetNewAgenda(newAgenda);
        }

        public async Task ExecuteLoadIfNeeded()
        {
            if (agendaService.CachedAgenda == null)
            {
                await LoadAgendaCommand.ExecuteAsync();
            }
            else {
                SetNewAgenda(agendaService.CachedAgenda);
            }
        }

        void SetNewAgenda(List<AgendaItemDTO> agenda)
        {
            AgendaDates.Clear();
            CurrentAgendaSections.Clear();
            if (agenda.IsNullOrEmpty()) return;
            agenda.Sort((x, y) => { return DateTime.Compare(x.Date.Date.Add(x.StartTime), y.Date.Date.Add(y.StartTime)); });

            agenda.ForEach((AgendaItemDTO agendaDTO) =>
            {
                if (AgendaDates.Count == 0)
                {
                    AgendaDates.Add(new AgendaDate(new AgendaItemViewModel(agendaDTO)));
                    return;
                }

                if (EqualDays(agendaDTO.Date, AgendaDates.Last().Date))
                {
                    if (agendaDTO.Location.Equals(AgendaDates.Last().Last().Location))
                        AgendaDates.Last().Last().Add(new AgendaItemViewModel(agendaDTO));
                    else
                        AgendaDates.Last().Add(new AgendaSection(new AgendaItemViewModel(agendaDTO)));
                }
                else {
                    AgendaDates.Add(new AgendaDate(new AgendaItemViewModel(agendaDTO)));
                }
            });

            var closestDate = AgendaDates.FirstOrDefault((agendaDate) =>
            {
                return agendaDate.Date.Date >= DateTime.Now.Date;
            });

            if (closestDate != null)
                CurrentAgendaDate = closestDate.Date;
            else {
                if (AgendaDates.Count != 0) CurrentAgendaDate = AgendaDates.First().Date;
            }
        }

        static bool EqualDays(DateTime first, DateTime second)
        {
            return first.Date.Year == second.Date.Year && first.Date.DayOfYear == second.Date.DayOfYear;
        }
    }
}
