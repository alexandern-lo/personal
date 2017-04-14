using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using SL4N;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Data.Entities;
using ServiceStack;

namespace LiveOakApp.Models.Services
{
    public class AttendeesFiltersService
    {
        readonly EventsService EventsService;

        public AttendeesFiltersService(EventsService eventsService)
        {
            EventsService = eventsService;
            Debug.Assert(EventsService != null, "EventsService must not be null");
        }

        #region Filters

        string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                ClearAttendeesCache();
            }
        }

        List<CategoryOptionSelection> _categorySelections;
        public List<CategoryOptionSelection> CategorySelections
        {
            get { return _categorySelections; }
            set
            {
                _categorySelections = value;
                ClearAttendeesCache();
            }
        }

        EventDTO _currentEvent;
        public EventDTO CurrentEvent
        {
            get { return _currentEvent; }
            set
            {
                var eventChanged = _currentEvent?.UID != value?.UID;
                _currentEvent = value;
                if (eventChanged)
                {
                    SearchText = null;
                    CategorySelections = null;
                    ClearAttendeesCache();
                }
            }
        }

        #endregion

        #region Attendees Cache

        public bool IsAllDataLoaded()
        {
            var total = CachedResult?.Response?.TotalFilteredRecords;
            if (total == null) return false;
            var loaded = CachedAttendees?.Count ?? 0;
            return (total ?? 0) <= loaded;
        }

        public string ETag { get; private set; }

        ApiResult<List<AttendeeDTO>> CachedResult { get; set; }

        public List<AttendeeDTO> CachedAttendees { get; private set; }

        public void ReplaceCache(EventDTO @event, ApiResult<List<AttendeeDTO>> result, string eTag)
        {
            if (CurrentEvent.UID != @event.UID) return;
            ETag = eTag;
            CachedResult = result;
            CachedAttendees = result.Content;
        }

        public void AppendPage(EventDTO @event, ApiResult<List<AttendeeDTO>> result)
        {
            if (CurrentEvent.UID != @event.UID) return;
            CachedAttendees.AddRange(result.Content);
        }

        public bool IsFiltersEnabled
        {
            get
            {
                var filter = FilterForCurrentSelection();
                if (filter == null) return false;
                return !filter.Query.IsNullOrEmpty() || !filter.CategoryFilters.IsNullOrEmpty();
            }
        }

        public bool IsCategoryFiltersEnabled
        {
            get
            {
                var filter = FilterForCurrentSelection();
                if (filter == null) return false;
                return !filter.CategoryFilters.IsNullOrEmpty();
            }
        }

        void ClearAttendeesCache()
        {
            ETag = null;
            CachedAttendees = null;
            CachedResult = null;
        }

        #endregion

        #region Api Filter

        public AttendeeFilterDTO FilterForCurrentSelection()
        {
            var filter = new AttendeeFilterDTO();
            filter.Query = SearchText;
            filter.CategoryFilters = CategoryFiltersForCurrentSelection();
            return filter;
        }

        List<AttendeeCategoryFilterDTO> CategoryFiltersForCurrentSelection()
        {
            if (CategorySelections == null)
            {
                return null;
            }
            return CategorySelections.GroupBy(sel => sel.CategoryUID).Where(gr =>
            {
                return gr.Any(sel => sel.IsSelected);
            }).Select(_ =>
            {
                var selections = _.Where(sel => sel.IsSelected).Select(sel => sel.OptionUID);
                return new AttendeeCategoryFilterDTO
                {
                    Uid = _.Key,
                    Values = selections.ToList()
                };
            }).ToList();
        }

        #endregion
    }
}
