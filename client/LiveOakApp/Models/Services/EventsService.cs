using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.Models.Services
{
    public class EventsService
    {
        const int PAGE_SIZE = 50;

        public List<EventDTO> Events { get { return EventsRequest.Result; } }
        public List<EventDTO> SelectableEvents { get { return SelectableEventsRequest.Result; } }

        public CachableRequest<List<EventDTO>> EventsRequest { get; private set; }
        public CachableRequest<List<EventDTO>> SelectableEventsRequest { get; private set; }

        public EventsService(CacheStorage CacheStorage)
        {
            EventsRequest = new CachableRequest<List<EventDTO>>(
                CacheStorage,
                "events",
                (eTag, token) => ServiceLocator.Instance.ApiService.GetEvents(PAGE_SIZE, eTag, token)
            );
            SelectableEventsRequest = new CachableRequest<List<EventDTO>>(
                CacheStorage,
                "events",
                (eTag, token) => ServiceLocator.Instance.ApiService.GetSelectableEvents(PAGE_SIZE, eTag, token)
            );
        }

        public async Task LoadEventsIfNeeded(CancellationToken? cancellationToken)
        {
            await EventsRequest.LoadFromCache();
            if (Events == null)
                await EventsRequest.LoadFromNetwork(cancellationToken);
            
            await SelectableEventsRequest.LoadFromCache();
            if (SelectableEvents == null)
                await SelectableEventsRequest.LoadFromNetwork(cancellationToken);
        }
    }
}
