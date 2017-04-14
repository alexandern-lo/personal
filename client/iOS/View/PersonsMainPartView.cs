using UIKit;
using Foundation;
using StudioMobile;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Resources;
using CoreGraphics;

namespace LiveOakApp.iOS.View.Content
{
    public class PersonsMainPartView : CustomView, IUITextFieldDelegate
    {
        [View(1)]
        [CommonSkin("EventPanelBackgroundView")]
        public UIView EventPanelBackgroundView { get; protected set; }

        [View(2)]
        [CommonSkin("SearchTextFieldBackgroundView")]
        public UIView SearchTextFieldBackgroundView { get; protected set; }

        [View(3)]
        public UITableView PersonsTableView { get; protected set; }

        [View(4)]
        [ButtonSkin("PersonsListFilterButton")]
        public UIButton FilterButton { get; protected set; }

        [View(5)]
        [CommonSkin("PersonsListSearchIcon")]
        public UIImageView SearchIconImageView { get; protected set; }

        [View(6)]
        [CommonSkin("PersonsListSearchTextField")]
        public UITextField SearchTextField { get; protected set; }

        [View(7)]
        [CommonSkin("SelectEventIconImageView")]
        public UIImageView SelectEventIconImageView { get; protected set; }

        [View(8)]
        [ButtonSkin("PersonsListEventButton")]
        public UIButton SelectEventButton { get; protected set; }

        [View(12)]
        public UIActivityIndicatorView ActivityIndicatorView { get; protected set; }

        public UIRefreshControl RefreshControl { get; private set; } = new UIRefreshControl();
        UIActivityIndicatorView loadingPageIndicator = new UIActivityIndicatorView();
        KeyboardScroller scroller;


        public bool EnableFilterButton
        {
            get
            {
                return !FilterButton.Hidden;
            }
            set
            {
                FilterButton.Hidden = !value;
            }
        }

        bool searchRunning = false;
        public bool SearchRunning
        {
            get { return searchRunning; }
            set
            {
                searchRunning = value;
                RefreshActivityIndicator();
            }
        }

        void RefreshActivityIndicator()
        {
            if (SearchRunning)
            {
                ActivityIndicatorView.StartAnimating();
                PersonsTableView.Hidden = true;
            }
            else
            {
                ActivityIndicatorView.StopAnimating();
                PersonsTableView.Hidden = false;
            }
        }

        protected override void CreateView()
        {
            base.CreateView();
            SearchTextField.ReturnKeyType = UIReturnKeyType.Search;
            SearchTextField.ClearButtonMode = UITextFieldViewMode.Always;
            PersonsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            PersonsTableView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.Interactive;
            PersonsTableView.RowHeight = PersonCell.RowHeight;
            RefreshControl.BackgroundColor = UIColor.Clear;
            RefreshControl.TintColor = Colors.LightGray;
            PersonsTableView.AddSubview(RefreshControl);
            ActivityIndicatorView.HidesWhenStopped = true;
            ActivityIndicatorView.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;

            scroller = new KeyboardScroller()
            {
                ScrollView = PersonsTableView
            };
            loadingPageIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
            loadingPageIndicator.Frame = new CGRect(0, 0, 1, 30);

            var tapGesture = new UITapGestureRecognizer(() => { this.EndEditing(true); });
            tapGesture.CancelsTouchesInView = false;
            AddGestureRecognizer(tapGesture);
        }

        const float eventPanelHeight = 50;
        const float selectEventIconImageHeight = 11.5f;
        const float searchPanelHeight = 40;
        const float searchIconHeight = 23;
        const float searchTopMargin = 14.75f;
        const float tableViewTopMargin = 5f;
        const float tableFooterHeight = 40;

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var pW = Bounds.Width;
            var pH = Bounds.Height;

            // EVENT PANEL
            var leftAndRightMargin = pW * 0.04f;

            EventPanelBackgroundView.Frame = this.LayoutBox()
                .Height(eventPanelHeight)
                .Top(0)
                .Left(0)
                .Right(0);

            SelectEventIconImageView.Frame = this.LayoutBox()
                .Height(selectEventIconImageHeight)
                .Width(pW * 0.0533f)
                .Right(pH * 0.0216f)
                .CenterVertically(EventPanelBackgroundView);

            SelectEventButton.Frame = this.LayoutBox()
                .Height(eventPanelHeight)
                .Top(0)
                .Left(0)
                .Right(0);

            SelectEventButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, leftAndRightMargin);
            SelectEventButton.TitleEdgeInsets = new UIEdgeInsets(0, leftAndRightMargin, 0, 0);

            // SEARCH TEXT FIELD AND FILTER
            var searchRightMargin = pW * 0.02667f;

            var searchTextFieldRightMargin = leftAndRightMargin;
            if (EnableFilterButton)
            {
                FilterButton.Frame = this.LayoutBox()
                    .Width(searchPanelHeight)
                    .Height(searchPanelHeight)
                    .Right(leftAndRightMargin)
                    .Below(EventPanelBackgroundView, searchTopMargin);
                searchTextFieldRightMargin += searchRightMargin + searchPanelHeight;
            }

            SearchTextFieldBackgroundView.Frame = this.LayoutBox()
                .Height(searchPanelHeight)
                .Left(leftAndRightMargin)
                .Right(searchTextFieldRightMargin)
                .Below(EventPanelBackgroundView, searchTopMargin);

            SearchIconImageView.Frame = this.LayoutBox()
                .Height(searchIconHeight)
                .Width(searchIconHeight)
                .CenterVertically(SearchTextFieldBackgroundView)
                .Left(SearchTextFieldBackgroundView, pH * 0.013f);

            SearchTextField.Frame = this.LayoutBox()
                .Height(searchPanelHeight)
                .After(SearchIconImageView, pW * 0.053f)
                .Right(SearchTextFieldBackgroundView, 0)
                .Below(EventPanelBackgroundView, searchTopMargin);

            // TABLE VIEW
            PersonsTableView.Frame = this.LayoutBox()
                .Below(SearchTextFieldBackgroundView, tableViewTopMargin)
                .Bottom(0)
                .Left(0)
                .Right(0);

            ActivityIndicatorView.Frame = this.LayoutBox()
                .Width(44.0f)
                .Height(44.0f)
                .Top(PersonsTableView, 5.0f)
                .CenterHorizontally();

            this.LayoutBox().Left(0);
        }

        public void SlideUpFooter(bool show, bool animated)
        {
            if (show)
            {
                PersonsTableView.TableFooterView = loadingPageIndicator;
                loadingPageIndicator.StartAnimating();
            }
            else {
                PersonsTableView.TableFooterView = null;
                loadingPageIndicator.StopAnimating();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                scroller.Dispose();
            }
        }
    }
}

