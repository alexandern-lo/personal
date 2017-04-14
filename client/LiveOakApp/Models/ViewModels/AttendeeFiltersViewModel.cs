using System.Collections.Generic;
using StudioMobile;
using LiveOakApp.Models.Services;
using LiveOakApp.Models.Data.Entities;

namespace LiveOakApp.Models.ViewModels
{
    public class AttendeeFiltersViewModel : DataContext
    {
        AttendeesFiltersService filtersService;

        bool LegacyImmediateSave;

        public AttendeeFiltersViewModel(EventViewModel @event, bool legacyImmediateSave = false)
        {
            LegacyImmediateSave = legacyImmediateSave;
            filtersService = ServiceLocator.Instance.AttendeesFiltersService;
            if (filtersService.CurrentEvent.UID != @event.EventDTO.UID)
            {
                filtersService.CurrentEvent = @event.EventDTO;
            }
            CurrentEvent = new EventViewModel(@event.EventDTO);
            FiltersEditor = new AttendeeFiltersEditor();
            ReloadSelections();

            ToggleOptionCommand = new Command
            {
                Action = ToggleOptionAction
            };

            ResetTogglesCommand = new Command
            {
                Action = ResetTogglesAction
            };

            SaveChangesCommand = new Command
            {
                Action = SaveChangesAction
            };
        }

        public EventViewModel CurrentEvent { get; private set; }

        readonly AttendeeFiltersEditor FiltersEditor;

        public ObservableList<Section> Sections = new ObservableList<Section>();

        void ReloadSelections()
        {
            List<Section> newSections = new List<Section>();
            var categorySelections = FiltersEditor.CategorySelections;
            foreach (var category in CurrentEvent.EventDTO.Categories)
            {
                var optionVMs = new List<OptionToggleViewModel>();
                foreach (var option in category.Options)
                {
                    var selection = categorySelections.Find(_ => _.OptionUID.Equals(option.UID));
                    var optionVM = new OptionToggleViewModel(selection, option.Name);
                    optionVMs.Add(optionVM);
                }
                var section = new Section(optionVMs, category.Name);
                newSections.Add(section);
            }
            Sections.Reset(newSections);
        }

        #region Toggle Selection

        public Command ToggleOptionCommand { get; private set; }

        void ToggleOptionAction(object option)
        {
            var optionVM = (OptionToggleViewModel)option;
            optionVM.InvertSelection(FiltersEditor);
            if (LegacyImmediateSave) FiltersEditor.SaveFilters();
        }

        #endregion

        #region Reset Selection

        public Command ResetTogglesCommand { get; private set; }

        void ResetTogglesAction(object obj)
        {
            FiltersEditor.ResetSelection();
            if (LegacyImmediateSave) FiltersEditor.SaveFilters();
            ReloadSelections();
        }

        #endregion

        #region Save Changes

        public Command SaveChangesCommand { get; private set; }

        void SaveChangesAction(object obj)
        {
            FiltersEditor.SaveFilters();
        }

        #endregion

        public class Section : ObservableList<OptionToggleViewModel>
        {
            public Section(IEnumerable<OptionToggleViewModel> collection, string header) : base(collection)
            {
                Header = header;
            }
            public readonly string Header;
        }

        public class OptionToggleViewModel
        {
            public CategoryOptionSelection Selection { get; private set; }
            public readonly string OptionName;
            public readonly string OptionUID;
            public bool IsSelected { get; private set; }

            public OptionToggleViewModel(CategoryOptionSelection selection, string optionName)
            {
                Selection = selection;
                OptionName = optionName;
                OptionUID = selection.OptionUID;
                IsSelected = selection.IsSelected;
            }

            public void InvertSelection(AttendeeFiltersEditor editor)
            {
                editor.ToggleSelection(Selection);
                Selection = Selection.InvertedSelection();
                IsSelected = Selection.IsSelected;
            }
        }
    }
}
