using System;
using StudioMobile;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Resources;

namespace LiveOakApp.iOS.View.Content
{
    public class AttendeesView : PersonsView
    {
        protected override void CreateView()
        {
            base.CreateView();
            EnableFilterButton = true;
            MainPartView.SearchTextField.Placeholder = L10n.Localize("AttendeesSearchHint", "Search attendees");
        }

        public Action<int> OnAttendeeAtIndexWillBeShown;

        public IUITableViewBinding GetPersonsBinding(ObservableList<AttendeeViewModel> persons)
        {
            return new PlainUITableViewBinding<AttendeeViewModel>()
            {
                TableView = MainPartView.PersonsTableView,
                DataSource = persons,
                CellFactory = (tableView, item, index) =>
                {
                    if (OnAttendeeAtIndexWillBeShown != null)
                    {
                        OnAttendeeAtIndexWillBeShown(index);
                    }
                    var cell = tableView.DequeueReusableCell(PersonCell.DefaultCellIdentifier) as PersonCell;
                    if (cell == null) cell = new PersonCell();
                    var photo = new FileResource(null, item.AvatarUrl);
                    cell.SetupCell(photo, item.FullName, item.Company);
                    return cell;
                }
            };
        }
    }
}
