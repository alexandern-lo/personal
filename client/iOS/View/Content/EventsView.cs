using System;
using CoreGraphics;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.iOS.View.TableHeaders;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Models.Data;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Content
{
    public class EventsView : CustomView
    {
        [View(0)]
        public UITableView EventsTableView { get; private set; }

        [View(1)]
        [ButtonSkin("AddLeadButton")]
        public HighlightButton AddButton { get; private set; }

        [View(2)]
        public ListUpdatingView ListUpdatingView { get; private set; }

        public UIRefreshControl RefreshControl { get; private set; } = new UIRefreshControl();
        const float listUpdatingViewHeight = 40;

        bool eventsFetchRunning = false;
        public bool EventsFetchRunning
        {
            get { return eventsFetchRunning; }
            set
            {
                SetNeedsLayout();
                LayoutIfNeeded();

                eventsFetchRunning = value;

                UIView.Animate(0.4, 0, UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.AllowUserInteraction, () => { LayoutSubviews(); }, null);
            }
        }

        bool isEventSelectionMode = false;
        public bool IsEventSelectionMode
        {
            get { return isEventSelectionMode; }
            set
            {
                isEventSelectionMode = value;
                SetNeedsLayout();
            }
        }

        protected override void CreateView()
        {
            base.CreateView();
            EventsTableView.RowHeight = EventCell.RowHeight;
            EventsTableView.TableFooterView = new UIView(new CGRect(0, 0, 0, 0));
            RefreshControl.BackgroundColor = Colors.RefreshViewBackgroundColor;
            RefreshControl.TintColor = Colors.LightGray;

            UIView bottomBackgroundForRefresh = new UIView();
            bottomBackgroundForRefresh.Frame = new CGRect(0, RefreshControl.Bounds.Height, RefreshControl.Bounds.Width, 10);
            bottomBackgroundForRefresh.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleWidth;
            bottomBackgroundForRefresh.BackgroundColor = Colors.RefreshViewBackgroundColor;
            bottomBackgroundForRefresh.ClipsToBounds = false;
            RefreshControl.AddSubview(bottomBackgroundForRefresh);
            EventsTableView.AddSubview(RefreshControl);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            ListUpdatingView.Frame = this.LayoutBox()
                .Top(EventsFetchRunning ? 0 : -listUpdatingViewHeight)
                .Height(40)
                .Left(0)
                .Right(0);

            EventsTableView.Frame = this.LayoutBox()
                .Below(ListUpdatingView, 0)
                .Bottom(0)
                .Left(0)
                .Right(0);

            AddButton.Frame = this.LayoutBox()
                .Height(Bounds.Height * 0.0814f)
                .Width(Bounds.Width * 0.213f)
                .Bottom(Bounds.Height * 0.02f)
                .Right(Bounds.Width * 0.04f);

            AddButton.Hidden = isEventSelectionMode;
        }

        public IUITableViewBinding GetSectionsBinding(ObservableList<EventsViewModel.Section> sections)
        {
            return new GroupedUITableViewDataSource<EventsViewModel.Section, EventViewModel>
            {
                DataSource = sections,
                CellFactory = (UITableView tableView, EventViewModel item, int section, int index) =>
                {
                    var cell = tableView.DequeueReusableCell(EventCell.DefaultCellIdentifier) as EventCell;
                    if (cell == null) cell = new EventCell();
                    var viewModel = new EventDetailsViewModel(item);
                    cell.SetupCell(viewModel.Title, viewModel.Date, viewModel.Location, section);
                    return cell;
                },
                HeaderFactory = (UITableView tableView, EventsViewModel.Section section, int sectionIndex) =>
                {
                    switch (section.Header)
                    {
                        case EventsViewModel.SectionHeader.Now:
                            var header = new SectionHeaderView(L10n.Localize("SectionNow", "Happening now"));
                            CommonSkin.HappeningNowSectionHeaderView(header);
                            return header;
                        case EventsViewModel.SectionHeader.Recent:
                            var recentHeader = new SectionHeaderView(L10n.Localize("SectionRecent", "Recent events"));
                            CommonSkin.RecentSectionHeaderView(recentHeader);
                            return recentHeader;
                        case EventsViewModel.SectionHeader.Upcoming: return new SectionHeaderView(L10n.Localize("SectionUpcoming", "Upcoming events"));
                        default: return null;
                    }
                },
                HeaderHeightFactory = (UITableView tableView, EventsViewModel.Section section, int sectionIndex) => {
                    return SectionHeaderView.HeaderHeight;
                },
                FooterFactory = (UITableView arg1, EventsViewModel.Section arg2, int arg3) =>
                {
                    return new UIView()
                    {
                        BackgroundColor = UIColor.Clear
                    };
                },
                FooterHeightFactory = (UITableView arg1, EventsViewModel.Section arg2, int arg3) =>
                {
                    return 1.1f;
                },
                TableView = EventsTableView
            };
        }
    }
}
