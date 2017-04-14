using System;
using LiveOakApp.Models.Data.NetworkDTO;
using StudioMobile;

namespace LiveOakApp.Models.ViewModels
{
    public class AgendaItemViewModel : DataContext
    {
        private AgendaItemDTO AgendaItemDTO { get; set; }

        public string UID { get { return AgendaItemDTO.UID; } }
        public string Name { get { return AgendaItemDTO.Name; } }
        public string Description { get { return AgendaItemDTO.Description; } }
        public DateTime Date { get { return AgendaItemDTO.Date; } }
        public TimeSpan StartTime { get { return AgendaItemDTO.StartTime; } }
        public TimeSpan EndTime { get { return AgendaItemDTO.EndTime; } }
        public string Location { get { return AgendaItemDTO.Location; } }
        public string DetailsUrl { get { return AgendaItemDTO.DetailsUrl; } }
        public string LocationUrl { get { return AgendaItemDTO.LocationUrl; } }


        public AgendaItemViewModel(AgendaItemDTO agendaItemDTO)
        {
            AgendaItemDTO = agendaItemDTO;
        }

        public string GetTimeIntervalString()
        {
            return ServiceLocator.Instance.DateTimeService.TimeToDisplayString(new DateTime(StartTime.Ticks)) + " \u2014 " + ServiceLocator.Instance.DateTimeService.TimeToDisplayString(new DateTime(EndTime.Ticks));
        }

        public string AgendaItemToJson()
        {
            return ServiceLocator.Instance.JsonService.Serialize(AgendaItemDTO);
        }

        public static AgendaItemViewModel JsonToAgendaItem(string agendaItem)
        {
            return new AgendaItemViewModel(ServiceLocator.Instance.JsonService.Deserialize<AgendaItemDTO>(agendaItem));
        }
    }
}
