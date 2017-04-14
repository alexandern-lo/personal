using System;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Content
{
    public class InboxView : CustomView
    {
        [View(0)]
        public ListUpdatingView ListUpdatingView { get; private set; }

        [View(1)]
        public UIView UpdatedInfoBackgroundView { get; private set; }

        [View(2)]
        [LabelSkin("SmallRegularBlackLabel")]
        public UILabel UpdatedInfoLabel { get; private set; }

        [View(3)]
        public UITableView InboxTableView { get; private set; }

        public UIRefreshControl RefreshControl { get; private set; } = new UIRefreshControl();

        const float listUpdatingViewHeight = 40;
        const float updatedInfoViewHeight = 25;
        bool resourcesFetchRunning = false;
        public bool ResourcesFetchRunning
        {
            get { return resourcesFetchRunning; }
            set
            {
                SetNeedsLayout();
                LayoutIfNeeded();
                resourcesFetchRunning = value;
                RefreshControl.Subviews[0].Subviews[0].Hidden = value;
                Animate(0.4, 0, UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.AllowUserInteraction, LayoutSubviews, null);
            }
        }

        protected override void CreateView()
        {
            base.CreateView();

            UpdatedInfoBackgroundView.BackgroundColor = Colors.DefaultTableViewBackgroundColor;
            UpdatedInfoLabel.Text = "Updated at " + ServiceLocator.Instance.DateTimeService.TimeToDisplayString(DateTime.Now);
            InboxTableView.RowHeight = InboxItemCell.CellHeight();
            RefreshControl.TintColor = Colors.LightGray;
            InboxTableView.AddSubview(RefreshControl);
        }

        public PlainUITableViewBinding<InboxItemViewModel> GetInboxItemsBinding(ObservableList<InboxItemViewModel> inboxItems)
        {
            PlainUITableViewBinding<InboxItemViewModel> tableViewBinding = null;
            tableViewBinding = new PlainUITableViewBinding<InboxItemViewModel>
            {
                DataSource = inboxItems,
                CellFactory = (UITableView tableView, InboxItemViewModel item, int index) =>
                {
                    var cell = tableView.DequeueReusableCell(InboxItemCell.DefaultCellIdentifier) as InboxItemCell;
                    if (cell == null) cell = new InboxItemCell(InboxItemCell.DefaultCellIdentifier);
                    cell.SetupCell(item);
                    return cell;
                },
                EditActionsForRow = (tableView, indexPath) => {
                    UITableViewRowAction deleteAction = UITableViewRowAction.Create(UITableViewRowActionStyle.Destructive, L10n.Localize("Delete", "Delete"), (action, indexPath1) =>
                    {
                        inboxItems.Remove((InboxItemViewModel)tableViewBinding.ItemForSelection(indexPath1));
                    });
                    return new []{deleteAction};
                },
                TableView = InboxTableView
            };
            return tableViewBinding;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            UpdatedInfoLabel.SizeToFit();

            InboxTableView.Frame = this.LayoutBox()
                .Top(ResourcesFetchRunning ? listUpdatingViewHeight + updatedInfoViewHeight : updatedInfoViewHeight)
                .Bottom(0)
                .Left(0)
                .Right(0);

            UpdatedInfoBackgroundView.Frame = this.LayoutBox()
                .Above(InboxTableView, 0)
                .Height(updatedInfoViewHeight)
                .Left(0)
                .Right(0);

            UpdatedInfoLabel.Frame = this.LayoutBox()
                .CenterVertically(UpdatedInfoBackgroundView)
                .CenterHorizontally(UpdatedInfoBackgroundView)
                .Width(UpdatedInfoLabel.Bounds.Width)
                .Height(UpdatedInfoLabel.Bounds.Height);

            ListUpdatingView.Frame = this.LayoutBox()
                .Top(ResourcesFetchRunning ? 0 : -listUpdatingViewHeight)
                .Above(UpdatedInfoBackgroundView, 0)
                .Left(0)
                .Right(0);
        }
    }
}

