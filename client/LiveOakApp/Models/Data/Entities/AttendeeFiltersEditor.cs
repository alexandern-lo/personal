using System.Collections.Generic;
using SL4N;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Services;

namespace LiveOakApp.Models.Data.Entities
{
    public class AttendeeFiltersEditor
    {
        static readonly ILogger LOG = LoggerFactory.GetLogger<AttendeeFiltersEditor>();

        readonly AttendeesFiltersService filtersService;

        public EventDTO CurrentEvent { get; set; }

        public AttendeeFiltersEditor()
        {
            filtersService = ServiceLocator.Instance.AttendeesFiltersService;

            CurrentEvent = filtersService.CurrentEvent;
            if (CurrentEvent == null)
            {
                LOG.Error("No CurrentEvent, editor will not work");
                return;
            }

            CategorySelections = DeepCopyCategotySelections(filtersService.CategorySelections)
               ?? AllCategorySelectionsOfEvent(CurrentEvent);
        }

        #region Filters

        public List<CategoryOptionSelection> CategorySelections { get; private set; }

        public void SaveFilters()
        {
            if (CurrentEvent == null) return;
            var newSelections = CategorySelections;
            if (newSelections.HasSameObjects(AllCategorySelectionsOfEvent(CurrentEvent)))
            {
                newSelections = null;
            }
            if (!newSelections.HasSameObjects(filtersService.CategorySelections))
            {
                filtersService.CategorySelections = newSelections;
            }
        }

        #endregion

        #region Categories Selection

        public void ToggleSelection(CategoryOptionSelection selection)
        {
            if (CurrentEvent == null) return;
            if (CategorySelections.IsNullOrEmpty())
            {
                LOG.Error("No CategorySelections in Event: {0}, ToggleSelection failed: {1}", CurrentEvent, selection);
                return;
            }
            var index = CategorySelections.IndexOf(selection);
            if (index == -1)
            {
                LOG.Error("selection {0} not found in event: {1}", selection, CurrentEvent);
                return;
            }
            CategorySelections[index] = selection.InvertedSelection();
        }

        public void ResetSelection()
        {
            if (CurrentEvent == null) return;
            CategorySelections = AllCategorySelectionsOfEvent(CurrentEvent);
        }

        List<CategoryOptionSelection> AllCategorySelectionsOfEvent(EventDTO @event)
        {
            var result = new List<CategoryOptionSelection>();
            foreach (var category in @event.Categories)
            {
                foreach (var option in category.Options)
                {
                    var selection = new CategoryOptionSelection(category.UID, option.UID);
                    result.Add(selection);
                }
            }
            return result;
        }

        List<CategoryOptionSelection> DeepCopyCategotySelections(List<CategoryOptionSelection> selections)
        {
            if (selections == null) return null;
            return selections.ConvertAll(_ => new CategoryOptionSelection(_.CategoryUID, _.OptionUID, _.IsSelected));
        }

        #endregion
    }
}
