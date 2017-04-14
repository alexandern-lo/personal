using System;
using Foundation;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.Controller.Content
{
    public class AttendeesFilterController : CustomController<AttendeesFilterView>
    {
        readonly AttendeeFiltersViewModel ViewModel;

        readonly Action<bool> OnFinishedEditing;

        public AttendeesFilterController(EventViewModel eventItem, Action<bool> onFinishedEditing)
        {
            Title = L10n.Localize("CategoryFilterNavigationBarTitle", "Category Filter");
            ViewModel = new AttendeeFiltersViewModel(eventItem);
            OnFinishedEditing = onFinishedEditing;

            CancelCommand = new Command
            {
                Action = CancelAction
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var cancelButton = new UIBarButtonItem(L10n.Localize("Cancel", "Cancel"), UIBarButtonItemStyle.Plain, null);
            var doneButton = new UIBarButtonItem(L10n.Localize("Done", "Done"), UIBarButtonItemStyle.Plain, null);
            NavigationItem.LeftBarButtonItem = cancelButton;
            NavigationItem.RightBarButtonItem = doneButton;

            var dataSource = View.GetSectionsBinding(ViewModel.Sections) as GroupedUITableViewDataSource<AttendeeFiltersViewModel.Section, AttendeeFiltersViewModel.OptionToggleViewModel>;

            Bindings.Command(ViewModel.ToggleOptionCommand)
                    .ParameterConverter((indexPath) => dataSource.DataSource[((NSIndexPath)indexPath).Section][((NSIndexPath)indexPath).Row])
                    .To(View.SwitchListSource);
            Bindings.Add(dataSource);
            Bindings.Command(ViewModel.ResetTogglesCommand)
                    .To(View.ResetButton.ClickTarget())
                    .AfterExecute((s, c) => View.FilterTableView.ReloadData());
            Bindings.Command(ViewModel.SaveChangesCommand)
                    .To(doneButton.ClickedTarget())
                    .AfterExecute((s, c) => OnFinishedEditing(true));
            Bindings.Command(CancelCommand)
                    .To(cancelButton.ClickedTarget());
        }

        #region Commands

        Command CancelCommand { get; set; }
        void CancelAction(object param)
        {
            OnFinishedEditing(false);
        }

        #endregion
    }
}
