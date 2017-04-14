using System;
using Foundation;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.Data;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Content
{
    public class MyResourcesView : CustomView
    {
        [View(0)]
        public ListUpdatingView ListUpdatingView { get; protected set; }

        [View(1)]
        public UITableView ResourcesTableView { get; private set; }

        [View(2)]
        [ButtonSkin("ResourcesButton")]
        public UIButton SendButton { get; private set; }

        [View(3)]
        public CustomErrorView ErrorView { get; private set; }

        [View(4)]
        public CustomMessageView MessageView { get; private set; }

        public UIRefreshControl RefreshControl { get; private set; } = new UIRefreshControl();
        const float listUpdatingViewHeight = 40;

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
                UIView.Animate(0.4, 0, UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.AllowUserInteraction, () => { LayoutSubviews(); }, null);
            }
        }

        protected override void CreateView()
        {
            base.CreateView();
            BackgroundColor = UIColor.White;
            ResourcesTableView.RowHeight = ResourceCell.RowHeight;
            ResourcesTableView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);
            ResourcesTableView.AllowsMultipleSelection = true;
            RefreshControl.BackgroundColor = Colors.RefreshViewBackgroundColor;
            RefreshControl.TintColor = Colors.LightGray;
            ResourcesTableView.AddSubview(RefreshControl);
        }

        public PlainUITableViewBinding<ResourceViewModel> GetSectionsBinding(ObservableList<ResourceViewModel> resourcesItems)
        {
            var tableViewBinding = new PlainUITableViewBinding<ResourceViewModel>
            {
                DataSource = resourcesItems,
                CellFactory = (UITableView tableView, ResourceViewModel item, int index) =>
                {
                    var cell = tableView.DequeueReusableCell(ResourceCell.DefaultCellIdentifier) as ResourceCell;
                    if (cell == null) cell = new ResourceCell(ResourcesTableView, CheckCellSelectionAndSendButtonEnabling, ResourceCell.DefaultCellIdentifier);

                    cell.SetupCell(item);

                    return cell;
                },
                TableView = ResourcesTableView,
            };
            return tableViewBinding;
        }

        public void CheckCellSelectionAndSendButtonEnabling(ResourceCell resourceCell = null)
        {
            if (resourceCell != null)
            {
                if (resourceCell.Selected)
                    ResourcesTableView.DeselectRow(ResourcesTableView.IndexPathForCell(resourceCell), false);
                else
                    ResourcesTableView.SelectRow(ResourcesTableView.IndexPathForCell(resourceCell), false, UITableViewScrollPosition.None);
            }

            if (ResourcesTableView.IndexPathsForSelectedRows == null || ResourcesTableView.IndexPathsForSelectedRows?.Length == 0)
                DisableSendButton();
            else {
                SendButton.Enabled = true;
                SendButton.BackgroundColor = Colors.MainBlueColor;
            }
        }

        public void DisableSendButton()
        {
            SendButton.Enabled = false;
            SendButton.BackgroundColor = Colors.LightGray;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            SendButton.Frame = this.LayoutBox()
                .Left(15)
                .Height(50)
                .Bottom(15)
                .Right(15);
            
            ResourcesTableView.Frame = this.LayoutBox()
                .Top(ResourcesFetchRunning ? listUpdatingViewHeight : 0)
                .Above(SendButton, 15)
                .Width(Bounds.Width)
                .Left(0);
            
            ListUpdatingView.Frame = this.LayoutBox()
                .Top(ResourcesFetchRunning ? 0 : -listUpdatingViewHeight)
                .Above(ResourcesTableView, 0)
                .Left(0)
                .Right(0);

            ErrorView.SizeToFit();
            ErrorView.Frame = this.LayoutBox()
                .Height(ErrorView.Bounds.Height)
                .CenterVertically(-30)
                .Left(15)
                .Right(15);

            MessageView.SizeToFit();
            MessageView.Frame = this.LayoutBox()
                .Height(MessageView.Bounds.Height)
                .CenterVertically(-30)
                .Left(15)
                .Right(15);

        }
    }
}

