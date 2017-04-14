using System;
using CoreGraphics;
using LiveOakApp.iOS.View;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.iOS.View.TableHeaders;
using LiveOakApp.Models.ViewModels;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS
{
    public class AttendeeDetailsView : CustomView
    {
        
        public AttendeeInfoSectionHeader SectionHeader { get; private set; } = new AttendeeInfoSectionHeader();
        public AttendeeDetailsTableHeader TableHeader { get; private set; } = new AttendeeDetailsTableHeader();

        [View(1)]
        public UITableView InfoTableView { get; private set; }

        [View(2)]
        [ButtonSkin("AddLeadButton")]
        public HighlightButton AddButton { get; private set; }


        protected override void CreateView()
        {
            base.CreateView();
            InfoTableView.RowHeight = AttendeeInfoItemCell.RowHeight;
            InfoTableView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

            InfoTableView.TableHeaderView = TableHeader;
        }

        public IUITableViewBinding GetSectionsBinding(ObservableList<ObservableList<AttendeeInfoItemViewModel>> sections)
        {
            return new GroupedUITableViewDataSource<ObservableList<AttendeeInfoItemViewModel>, AttendeeInfoItemViewModel>
            {
                DataSource = sections,
                CellFactory = (UITableView tableView, AttendeeInfoItemViewModel item, int section, int index) =>
                {
                    var cell = tableView.DequeueReusableCell(AttendeeInfoItemCell.DefaultCellIdentifier) as AttendeeInfoItemCell;
                    if (cell == null) cell = new AttendeeInfoItemCell(AttendeeInfoItemCell.DefaultCellIdentifier);

                    cell.SetupCell(item);
                    return cell;
                },
                HeaderFactory = (UITableView tableView, ObservableList<AttendeeInfoItemViewModel> section, int sectionIndex) =>
                {
                    return SectionHeader;
                },
                HeaderHeightFactory = (UITableView arg1, ObservableList<AttendeeInfoItemViewModel> arg2, int arg3) =>
                {
                    return AttendeeInfoSectionHeader.HeaderHeight;
                },
                TableView = InfoTableView
            };
        }


        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var pH = this.Bounds.Height;
            var pW = this.Bounds.Width;

            AddButton.Frame = this.LayoutBox()
                .Height(pH * 0.0814f)
                .Width(pW * 0.213f)
                .Bottom(pH * 0.02f)
                .Right(pW * 0.04f);

            InfoTableView.Frame = this.LayoutBox()
                .Top(0)
                .Width(pW)
                .Bottom(0);
        }
    }
}

