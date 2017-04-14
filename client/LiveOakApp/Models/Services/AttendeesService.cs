using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.Models.Services
{
    public class AttendeesService
    {
        public const int PageSize = 50;
        const string SortField = "first_name";

        readonly ApiService ApiService;
        readonly AttendeesFiltersService FiltersService;

        public AttendeesService(ApiService apiService, AttendeesFiltersService filtersService)
        {
            ApiService = apiService;
            FiltersService = filtersService;
        }

        public async Task<List<AttendeeDTO>> SearchAttendees(CancellationToken? cancellationToken)
        {
            var @event = FiltersService.CurrentEvent;
            var filter = FiltersService.FilterForCurrentSelection();
            var eTag = FiltersService.ETag;
            var result = await ApiService.SearchAttendees(@event.UID, filter, SortField, 0, PageSize, eTag, cancellationToken);

            switch (result.Status)
            {
                case ApiResultStatus.Ok:
                    FiltersService.ReplaceCache(@event, result, result.ETag);
                    return result.Content;
                case ApiResultStatus.NotModified:
                    return FiltersService.CachedAttendees;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<List<AttendeeDTO>> LoadNextAttendeesPage(CancellationToken? cancellationToken)
        {
            var nextPage = CurrentPageIndex() + 1;
            if (nextPage == 0)
            {
                throw new Exception("Run SearchAttendees first");
            }

            var @event = FiltersService.CurrentEvent;
            var filter = FiltersService.FilterForCurrentSelection();
            var result = await ApiService.SearchAttendees(@event.UID, filter, SortField, nextPage, PageSize, null, cancellationToken);

            switch (result.Status)
            {
                case ApiResultStatus.Ok:
                    FiltersService.AppendPage(@event, result);
                    return result.Content;
                case ApiResultStatus.NotModified:
                    return FiltersService.CachedAttendees;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public int CurrentPageIndex()
        {
            var loaded = FiltersService.CachedAttendees?.Count ?? 0;
            var nextPage = loaded / PageSize;
            return nextPage - 1;
        }

        public bool CanLoadNextPage()
        {
            return FiltersService.CachedAttendees != null && !FiltersService.IsAllDataLoaded();
        }
    }
}
