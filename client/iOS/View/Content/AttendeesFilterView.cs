using System;
using CoreGraphics;
using Foundation;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.iOS.View.TableHeaders;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS
{
    public class AttendeesFilterView : CustomView
    {
        [View(0)]
        public UITableView FilterTableView { get; private set; }

        [View(1)]
        [ButtonSkin("AttendeesFilterResetButton")]
        public UIButton ResetButton { get; private set; }

        public EventListSource<UISwitch> SwitchListSource { get; private set; }

        protected override void CreateView()
        {
            base.CreateView();
            FilterTableView.TableFooterView = new UIView(new CGRect(0, 0, 0, 0));
            FilterTableView.BackgroundColor = Colors.DefaultTableViewBackgroundColor;
            FilterTableView.RowHeight = FilterItemCell.RowHeight;
            FilterTableView.SeparatorColor = new UIColor(0.902f, 0.902f, 0.902f, 1f);

            ResetButton.SetTitle(L10n.Localize("AttendeesFilterResetAll", "Reset all filters"), UIControlState.Normal);

            SwitchListSource = new EventListSource<UISwitch>(UIControlEvents.ValueChangedEvent)
            {
                ParameterExtractor = (sender, args) => GetPositionOfCellView((UITableViewCell)sender)
            };
        }

        NSIndexPath GetPositionOfCellView(UITableViewCell cell)
        {
            return FilterTableView.IndexPathForCell(cell);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            ResetButton.Frame = this.LayoutBox()
                .Left(0)
                .Right(0)
                .Bottom(0)
                .Height(50);
            FilterTableView.Frame = this.LayoutBox()
                .Top(0)
                .Above(ResetButton, 0)
                .Width(Bounds.Width)
                .Left(0);
        }

        public IUITableViewBinding GetSectionsBinding(ObservableList<AttendeeFiltersViewModel.Section> sections)
        {
            return new GroupedUITableViewDataSource<AttendeeFiltersViewModel.Section, AttendeeFiltersViewModel.OptionToggleViewModel>
            {
                DataSource = sections,
                CellFactory = (UITableView tableView, AttendeeFiltersViewModel.OptionToggleViewModel item, int section, int index) =>
                {
                    var cell = tableView.DequeueReusableCell(FilterItemCell.DefaultCellIdentifier) as FilterItemCell;
                    if (cell == null)
                    {
                        cell = new FilterItemCell();
                        SwitchListSource.Listen(cell.Switch, cell);
                    }
                    cell.SetupCell(item.OptionName, item.IsSelected);
                    return cell;
                },
                HeaderFactory = (UITableView tableView, AttendeeFiltersViewModel.Section section, int sectionIndex) =>
                {
                    var header = new FilterHeaderView();
                    header.TitleLabel.Text = section.Header;
                    return header;
                },
                HeaderHeightFactory = (tableView, section, sectionIndex) =>
                {
                    return 55f;
                },
                TableView = FilterTableView
            };
        }
    }
}
