using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.Models.Services
{
    public class AgendaService
    {
        readonly ApiService ApiService;
        public List<AgendaItemDTO> CachedAgenda { get; private set; }
        public string ETag { get; private set; }

        private EventDTO _currentEvent;
        public EventDTO CurrentEvent
        {
            get { return _currentEvent; }
            set
            {
                var eventChanged = _currentEvent?.UID != value?.UID;
                _currentEvent = value;
                if (eventChanged)
                {
                    ClearAgendaCache();
                }
            }
        }

        public AgendaService(ApiService apiService)
        {
            ApiService = apiService;
        }

        public async Task<List<AgendaItemDTO>> GetAgendaItems(CancellationToken? cancellationToken)
        {
            var @event = CurrentEvent;
            var eTag = ETag;
            var result = await ApiService.GetAgendaItems(@event.UID, eTag, cancellationToken);

            switch (result.Status)
            {
                case ApiResultStatus.Ok:
                    if (CurrentEvent.UID == @event.UID)
                    {
                        ETag = eTag;
                        CachedAgenda = result.Content;
                    }
                    return result.Content;
                case ApiResultStatus.NotModified:
                    return CachedAgenda;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void ClearAgendaCache()
        {
            ETag = null;
            CachedAgenda = null;
        }

    }
}
