using System;
using System.Collections.Generic;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.iOS.View.TableFooters;
using LiveOakApp.iOS.View.TableHeaders;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Content
{
    public class DashboardView : CustomView
    {
        [View(0)]
        public ListUpdatingView ListUpdatingView { get; protected set; }

        [View(1)]
        public UITableView DashboardTableView { get; private set; }

        [View(2)]
        public UIView MenuBarBackgroundView { get; private set; }

        [View(3)]
        public UIButton LeadsTabBarButton { get; protected set; }

        [View(4)]
        public UIButton EventsTabBarButton { get; protected set; }

        [View(5)]
        public UIButton ResourcesTabBarButton { get; protected set; }

        [View(6)]
        public UIButton RecentActivityTabBarButton { get; protected set; }

        [View(7)]
        public CustomErrorView ErrorView { get; private set; }

        public UIRefreshControl RefreshControl { get; private set; } = new UIRefreshControl();
        const float listUpdatingViewHeight = 40;
        const int eventGoalsDataIndex = 0;
        const int resourcesDataIndex = 1;
        const int eventExpensesDataIndex = 2;

        bool dashboardFetchRunning = false;
        public bool DashboardFetchRunning
        {
            get { return dashboardFetchRunning; }
            set
            {
                SetNeedsLayout();
                LayoutIfNeeded();
                dashboardFetchRunning = value;
                UIView.Animate(0.4, 0, UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.AllowUserInteraction, () => { LayoutSubviews(); }, null);
            }
        }

        protected override void CreateView()
        {
            base.CreateView();
            BackgroundColor = Colors.DefaultTableViewBackgroundColor;
            MenuBarBackgroundView.BackgroundColor = Colors.MainGrayColor;
            DashboardTableView.RowHeight = 50f;
            DashboardTableView.SeparatorColor = Colors.GraySeparatorColor;
            DashboardTableView.BackgroundColor = Colors.DefaultTableViewBackgroundColor;
            DashboardTableView.TableHeaderView = new DashboardTableHeaderView();
            DashboardTableView.TableFooterView = new DashboardTableFooterView();
            DashboardTableView.AllowsSelection = false;
            RefreshControl.BackgroundColor = Colors.RefreshViewBackgroundColor;
            RefreshControl.TintColor = Colors.LightGray;
            DashboardTableView.AddSubview(RefreshControl);

            LeadsTabBarButton.SetImage(UIImage.FromBundle("menu_leads"), UIControlState.Normal);
            EventsTabBarButton.SetImage(UIImage.FromBundle("menu_events"), UIControlState.Normal);
            ResourcesTabBarButton.SetImage(UIImage.FromBundle("menu_resources"), UIControlState.Normal);
            RecentActivityTabBarButton.SetImage(UIImage.FromBundle("menu_recentactivity"), UIControlState.Normal);
        }

        public void SetupTableHeaderAndFooter(LeadsStatisticsViewModel leadsStatistics)
        {
            ((DashboardTableHeaderView)DashboardTableView.TableHeaderView).SetupHeader(leadsStatistics);
            ((DashboardTableFooterView)DashboardTableView.TableFooterView).SetupFooter(leadsStatistics);
        }

        public void SetupThisYearExpenses(MoneyViewModel expenses)
        {
            ((DashboardTableFooterView)DashboardTableView.TableFooterView).ThisYearExpensesValueLabel.Text =
                expenses.GetCurrencySymbol() + Math.Round(expenses.Amount, MidpointRounding.AwayFromZero);
            SetNeedsLayout();
        }

        public void SetupThisYearCostPerLead(MoneyViewModel costPerLead)
        {
            ((DashboardTableFooterView)DashboardTableView.TableFooterView).AverageCostPerLeadValueLabel.Text =
                costPerLead.GetCurrencySymbol() + Math.Round(costPerLead.Amount, MidpointRounding.AwayFromZero);
            SetNeedsLayout();
        }

        public IUITableViewBinding GetSectionsBinding(ObservableList<ObservableList<DataContext>> sections, Func<decimal> getMaxEventExpenses, Action<DashboardEventViewModel> goalEditingButtonClickAction)
        {
            return new GroupedUITableViewDataSource<ObservableList<DataContext>, DataContext>
            {
                DataSource = sections,
                CellFactory = (UITableView tableView, DataContext item, int section, int index) =>
                {
                    if (section == eventGoalsDataIndex)
                    {
                        var cell = tableView.DequeueReusableCell(DashboardEventGoalsCell.DefaultCellIdentifier) as DashboardEventGoalsCell;
                        if (cell == null) cell = new DashboardEventGoalsCell(DashboardEventGoalsCell.DefaultCellIdentifier);
                        cell.SetupCell(item as DashboardEventViewModel, goalEditingButtonClickAction);
                        return cell;
                    }
                    else if (section == resourcesDataIndex)
                    {
                        var cell = tableView.DequeueReusableCell(DashboardResourceCell.DefaultCellIdentifier) as DashboardResourceCell;
                        if (cell == null) cell = new DashboardResourceCell(DashboardResourceCell.DefaultCellIdentifier);
                        cell.SetupCell(item as DashboardResourceViewModel);
                        return cell;
                    }
                    else {
                        var cell = tableView.DequeueReusableCell(DashboardEventExpensesCell.DefaultCellIdentifier) as DashboardEventExpensesCell;
                        if (cell == null) cell = new DashboardEventExpensesCell(DashboardEventExpensesCell.DefaultCellIdentifier);
                        cell.SetupCell(item as DashboardEventViewModel, getMaxEventExpenses);
                        return cell;
                    }
                },
                HeaderFactory = (UITableView tableView, ObservableList<DataContext> section, int sectionIndex) =>
                {
                    UIView headerView;
                    if (sectionIndex == eventGoalsDataIndex) headerView = new DashboardSectionHeaderView(L10n.Localize("Events", "Events"), L10n.Localize("LeadsTaken", "Leads taken"));
                    else if (sectionIndex == resourcesDataIndex) headerView = new DashboardSectionHeaderView(L10n.Localize("ResourceConversions", "Resource conversions"), L10n.Localize("Sent/Opened", "Sent/Opened"));
                    else headerView = new DashboardSectionHeaderView(L10n.Localize("Events", "Events"), L10n.Localize("CostPerLead", "Cost per lead"));
                    return headerView;
                },
                HeaderHeightFactory = (UITableView tableView, ObservableList<DataContext> section, int sectionIndex) =>
                {
                    return DashboardSectionHeaderView.HeaderHeight;
                },
                FooterFactory = (UITableView tableView, ObservableList<DataContext> section, int sectionIndex) =>
                {
                    if (sectionIndex == eventExpensesDataIndex) return null;
                    return new DashboardSectionFooterView();
                },
                FooterHeightFactory = (UITableView tableView, ObservableList<DataContext> section, int sectionIndex) =>
                {
                    if (sectionIndex == eventExpensesDataIndex) return 0;
                    return DashboardSectionFooterView.FooterHeight;
                },
                TableView = DashboardTableView
            };
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            LeadsTabBarButton.ImageView.SizeToFit();
            EventsTabBarButton.SizeToFit();
            ResourcesTabBarButton.SizeToFit();
            RecentActivityTabBarButton.SizeToFit();

            MenuBarBackgroundView.Frame = this.LayoutBox()
                .Bottom(0)
                .Height(60)
                .Left(0)
                .Right(0);

            DashboardTableView.Frame = this.LayoutBox()
                .Top(DashboardFetchRunning ? listUpdatingViewHeight : 0)
                .Above(MenuBarBackgroundView, 0)
                .Left(0)
                .Right(0);

            ListUpdatingView.Frame = this.LayoutBox()
                .Top(DashboardFetchRunning ? 0 : -listUpdatingViewHeight)
                .Above(DashboardTableView, 0)
                .Left(0)
                .Right(0);

            var tabBarButtonWidth = MenuBarBackgroundView.Bounds.Width / 4;

            LeadsTabBarButton.Frame = this.LayoutBox()
                .Bottom(0)
                .Left(0)
                .Height(MenuBarBackgroundView.Bounds.Height)
                .Width(tabBarButtonWidth);

            EventsTabBarButton.Frame = this.LayoutBox()
                .Bottom(0)
                .After(LeadsTabBarButton, 0)
                .Height(MenuBarBackgroundView.Bounds.Height)
                .Width(tabBarButtonWidth);

            ResourcesTabBarButton.Frame = this.LayoutBox()
                .Bottom(0)
                .After(EventsTabBarButton, 0)
                .Height(MenuBarBackgroundView.Bounds.Height)
                .Width(tabBarButtonWidth);

            RecentActivityTabBarButton.Frame = this.LayoutBox()
                .Bottom(0)
                .After(ResourcesTabBarButton, 0)
                .Height(MenuBarBackgroundView.Bounds.Height)
                .Width(tabBarButtonWidth);
            
            ErrorView.SizeToFit();
            ErrorView.Frame = this.LayoutBox()
                .Height(ErrorView.Bounds.Height)
                .CenterVertically(-30)
                .Left(15)
                .Right(15);

            DashboardTableHeaderView tableHeaderView = (DashboardTableHeaderView)DashboardTableView.TableHeaderView;
            tableHeaderView.Frame = new CoreGraphics.CGRect(0, 0, 1, DashboardTableView.Bounds.Width * 0.58);
            DashboardTableView.TableHeaderView = tableHeaderView;

            DashboardTableFooterView tableFooterView = (DashboardTableFooterView)DashboardTableView.TableFooterView;
            tableFooterView.Frame = new CoreGraphics.CGRect(0, 0, 1, DashboardTableFooterView.FooterHeight);
            DashboardTableView.TableFooterView = tableFooterView;
        }
    }
}

