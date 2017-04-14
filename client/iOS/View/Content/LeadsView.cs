using StudioMobile;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;

namespace LiveOakApp.iOS.View.Content
{
    public class LeadsView : PersonsView
    {
        protected override void CreateView()
        {
            base.CreateView();
            EnableFilterButton = false;
            MainPartView.SearchTextField.Placeholder = L10n.Localize("LeadsSearchHint", "Search leads");
        }

        public IUITableViewBinding GetPersonsBinding(ObservableList<LeadViewModel> persons)
        {
            return new PlainUITableViewBinding<LeadViewModel>()
            {
                TableView = MainPartView.PersonsTableView,
                DataSource = persons,
                CellFactory = (tableView, item, index) =>
                {
                    var cell = tableView.DequeueReusableCell(PersonCell.DefaultCellIdentifier) as PersonCell;
                    if (cell == null) cell = new PersonCell();
                    cell.SetupCell(item.PhotoResource, item.FullName, item.JobInfo);
                    return cell;
                }
            };
        }
    }
}
