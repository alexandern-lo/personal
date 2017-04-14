using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.iOS.View.TableHeaders;
using LiveOakApp.Models;
using LiveOakApp.Models.ViewModels;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Content
{
    public class AgendaView : CustomView
    {
        [View(0)]
        public UITableView AgendaTableView { get; private set; }

        [View(1)]
        public ListUpdatingView ListUpdatingView { get; private set; }

        [View(2)]
        public UIScrollView DatesScrollView { get; private set; }

        [View(3)]
        public CustomMessageView MessageView { get; private set; }

        [View(4)]
        public CustomErrorView ErrorView { get; private set; }

        public List<AgendaDateButton> AgendaDateButtons { get; private set; } = new List<AgendaDateButton>();
        public UIRefreshControl RefreshControl { get; private set; } = new UIRefreshControl();

        public event EventHandler CurrentDateChanged;
        DateTime currentDate;
        public DateTime CurrentDate
        {
            get { return currentDate; }
            set
            {
                if (CurrentDateChanged != null)
                {
                    CurrentDateChanged(this, EventArgs.Empty);
                }
                currentDate = value;
                UpdateDateButtonsStyle();
            }
        }

        public void SetupDateButtons(IEnumerable<AgendaViewModel.AgendaDate> dates, Action<object> dateClickAction)
        {
            AgendaDateButtons.Clear();
            foreach (var subview in DatesScrollView.Subviews)
                subview.RemoveFromSuperview();

            foreach (AgendaViewModel.AgendaDate agendaDate in dates)
            {
                AgendaDateButton dateView = new AgendaDateButton(dateClickAction);
                dateView.ImageViewSize = CGSize.Empty;
                dateView.TitleLabel.Font = Fonts.SmallRegular;
                dateView.SetTitleColor(UIColor.Black, UIControlState.Normal);
                dateView.SetTitle(ServiceLocator.Instance.DateTimeService.DateToDisplayString(agendaDate.Date).ToUpper(), UIControlState.Normal);
                dateView.EnableSeparator = false;
                dateView.BackgroundColor = Colors.MainGrayColor;
                AgendaDateButtons.Add(dateView);
                DatesScrollView.AddSubview(dateView);
            }
            SetNeedsLayout();
            LayoutIfNeeded();
            UpdateDateButtonsStyle();
        }

        public void UpdateDateButtonsStyle()
        {
            foreach (AgendaDateButton dateButton in AgendaDateButtons)
            {
                if (dateButton.CurrentTitle == null) continue;
                if (dateButton.CurrentTitle.Equals(ServiceLocator.Instance.DateTimeService.DateToDisplayString(CurrentDate).ToUpper()))
                {
                    dateButton.SetActive(true);
                    DatesScrollView.SetContentOffset(new CGPoint(dateButton.Frame.GetMinX() - datesButtonWidth, 0), true);
                }
                else dateButton.SetActive(false);
            }
        }

        const float listUpdatingViewHeight = 40;
        const float datesScrollViewHeight = 35;
        const float datesButtonWidth = 105;

        protected override void CreateView()
        {
            base.CreateView();
            ErrorView.Hidden = true;
            MessageView.Hidden = true;
            AgendaTableView.RowHeight = AgendaItemCell.RowHeight;
            AgendaTableView.TableFooterView = new UIView();
            RefreshControl.BackgroundColor = Colors.RefreshViewBackgroundColor;
            RefreshControl.TintColor = Colors.LightGray;
            AgendaTableView.AddSubview(RefreshControl);
            DatesScrollView.BackgroundColor = Colors.MainGrayColor;
            DatesScrollView.ShowsHorizontalScrollIndicator = false;
        }

        bool fetchRunning = false;
        public bool FetchRunning
        {
            get { return fetchRunning; }
            set
            {
                SetNeedsLayout();
                LayoutIfNeeded();

                fetchRunning = value;
                UIView.Animate(0.4, 0, UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.AllowUserInteraction, () => { LayoutSubviews(); }, null);
            }
        }

        public IUITableViewBinding GetSectionsBinding(ObservableList<AgendaViewModel.AgendaSection> sections, Action<string> tableHeaderClickAction)
        {
            return new GroupedUITableViewDataSource<AgendaViewModel.AgendaSection, AgendaItemViewModel>
            {
                DataSource = sections,
                CellFactory = (UITableView tableView, AgendaItemViewModel item, int section, int index) =>
                {
                    var cell = tableView.DequeueReusableCell(AgendaItemCell.DefaultCellIdentifier) as AgendaItemCell;
                    if (cell == null) cell = new AgendaItemCell();
                    cell.SetupCell(item.Name, item.Description, item.GetTimeIntervalString());
                    return cell;
                },
                HeaderFactory = (UITableView tableView, AgendaViewModel.AgendaSection section, int sectionIndex) =>
                {
                    var header = new AgendaHeaderView();
                    UITapGestureRecognizer headerTapRecognizer = new UITapGestureRecognizer((obj) =>
                    {
                        tableHeaderClickAction(section.LocationUrl);
                    });
                    header.AddGestureRecognizer(headerTapRecognizer);
                    header.LocationLabel.Text = section.Location;
                    return header;

                },
                HeaderHeightFactory = (UITableView tableView, AgendaViewModel.AgendaSection section, int sectionIndex) =>
                {
                    return AgendaHeaderView.HeaderHeight;
                },

                TableView = AgendaTableView
            };
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            ErrorView.SizeToFit();
            MessageView.SizeToFit();

            DatesScrollView.Frame = this.LayoutBox()
                .Height(datesScrollViewHeight)
                .Left(0)
                .Right(0)
                .Top(0);

            ListUpdatingView.Frame = this.LayoutBox()
                .Below(DatesScrollView, FetchRunning ? 0 : -listUpdatingViewHeight)
                .Height(listUpdatingViewHeight)
                .Left(0)
                .Right(0);

            AgendaTableView.Frame = this.LayoutBox()
                .Below(ListUpdatingView, 0)
                .Bottom(0)
                .Left(0)
                .Right(0);

            ErrorView.Frame = this.LayoutBox()
                .Height(ErrorView.Bounds.Height)
                .CenterVertically(-30)
                .Left(15)
                .Right(15);
            ErrorView.SizeToFit();
            MessageView.Frame = this.LayoutBox()
                .Height(MessageView.Bounds.Height)
                .CenterVertically(-30)
                .Left(15)
                .Right(15);
            MessageView.SizeToFit();

            int questionViewsIndex = 0;
            foreach (AgendaDateButton dateButton in AgendaDateButtons)
            {
                dateButton.Frame = new CGRect(datesButtonWidth * questionViewsIndex,
                                                0,
                                              datesButtonWidth,
                                              datesScrollViewHeight);
                questionViewsIndex++;
            }
            if (AgendaDateButtons.IsNullOrEmpty())
                DatesScrollView.ContentSize = new CGSize(this.Bounds.Width, datesScrollViewHeight);
            else
                DatesScrollView.ContentSize = new CGSize(AgendaDateButtons.Last().Frame.GetMaxX(), datesScrollViewHeight);
        }
    }

    public static class AgendaViewProperties
    {
        public static readonly RuntimeEvent CurrentDateChangedEvent = new RuntimeEvent(typeof(AgendaView), "CurrentDateChanged");
        public static readonly IPropertyBindingStrategy CurrentDateChangedBinding = new EventHandlerBindingStrategy(CurrentDateChangedEvent);

        public static IProperty<DateTime> CurrentDateProperty(this AgendaView agendaView)
        {
            return agendaView.GetProperty(_ => _.CurrentDate, CurrentDateChangedBinding);
        }

        [Preserve]
        static void LinkerTrick()
        {
            new AgendaView().CurrentDateChanged += (o, a) => { };
        }
    }
}
