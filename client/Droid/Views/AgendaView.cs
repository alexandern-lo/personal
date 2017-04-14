using Android.Content;
using Android.Util;
using Android.Widget;
using Android.Support.Design.Widget;
using SE.Emilsjolander.Stickylistheaders;
using Android.Views;
using Android.Animation;
using Android.Support.V4.View;
using StudioMobile;
using LiveOakApp.Models.ViewModels;
using System;
using LiveOakApp.Droid.CustomViews.Adapters;
using LiveOakApp.Models;
using Android.OS;
using Android.Support.V4.Widget;

namespace LiveOakApp.Droid.Views
{
    public class AgendaView : FrameLayout, TabLayout.IOnTabSelectedListener, StickyListHeadersListView.IOnHeaderClickListener2
    {

        public AgendaView(Context context) :
            base(context)
        {
            Initialize();
        }

        public AgendaView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public AgendaView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        public StickyListHeadersListView AgendaList { get; private set; }
        public TabLayout Tabs { get; private set; }
        public View ProgressBar { get; private set; }
        private View contentView;
        public Action<int> HeaderClickAction { get; set; }
        public Action<int> DateClickAction { get; set; }
        public TextView MessageView { get; set; }
        public SwipeRefreshLayout AgendaRefresher { get; private set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.AgendaView, this);

            Tabs = FindViewById<TabLayout>(Resource.Id.tabs);
            AgendaList = FindViewById<StickyListHeadersListView>(Resource.Id.agendaList);
            ProgressBar = FindViewById(Resource.Id.progressBar);
            contentView = FindViewById(Resource.Id.content);
            MessageView = FindViewById<TextView>(Resource.Id.message);
            AgendaRefresher = FindViewById<SwipeRefreshLayout>(Resource.Id.list_refresher);
            Tabs.SetOnTabSelectedListener(this);
            AgendaList.SetOnHeaderClickListener(this);
        }

        private bool isShowingProgressBar;
        public bool IsShowingProgressBar
        {
            get
            {
                return isShowingProgressBar;
            }
            set
            {
                isShowingProgressBar = value;

                if (isShowingProgressBar)
                {
                    if (ViewCompat.IsLaidOut(this))
                    {
                        animateShowProgressBar();
                    }
                    else
                    {
                        ProgressBar.Visibility = ViewStates.Visible;
                        contentView.SetY(TypedValue.ApplyDimension(ComplexUnitType.Dip, 50, Context.Resources.DisplayMetrics));
                    }
                }
                else if (ViewCompat.IsLaidOut(this))
                {
                    animateHideProgressBar();
                }
                else 
                {
                    ProgressBar.Visibility = ViewStates.Invisible;
                    contentView.SetY(TypedValue.ApplyDimension(ComplexUnitType.Dip, 0, Context.Resources.DisplayMetrics));
                }
            }
        }

        private void animateShowProgressBar()
        {
            var animator = CreateEventsScroll(0, ProgressBar.Height);
            animator.AnimationStart += (sender, e) => ProgressBar.Visibility = ViewStates.Visible;
            animator.Start();
        }

        private void animateHideProgressBar()
        {
            var animator = CreateEventsScroll(ProgressBar.Height, 0);
            animator.AnimationEnd += (sender, e) => ProgressBar.Visibility = ViewStates.Invisible;
            animator.Start();
        }

        private ValueAnimator CreateEventsScroll(int y0, int y1)
        {
            var animator = ValueAnimator.OfInt(y0, y1);
            animator.SetDuration(500);
            animator.Update += (sender, e) => contentView.SetY((int)e.Animation.AnimatedValue);
            return animator;
        }

        public StickyHeadersListViewAdapter<AgendaViewModel.AgendaSection, AgendaItemViewModel> GetSectionsAdapter(ObservableList<AgendaViewModel.AgendaSection> sections)
        {
            return sections.GetStickyHeadersAdapter<AgendaViewModel.AgendaSection, AgendaItemViewModel>(GetAgendaItemView, GetSectionHeaderView);
        }

        public View GetAgendaItemView(int position, AgendaViewModel.AgendaSection section, AgendaItemViewModel data, View convertView, View parent)
        {
            AgendaItemView itemView;
            if (convertView == null)
            {
                itemView = new AgendaItemView(parent.Context);
            }
            else {
                itemView = (AgendaItemView)convertView;
            }
            itemView.ViewModel = data;

            return itemView;
        }

        public static View GetSectionHeaderView(int position, AgendaViewModel.AgendaSection section, View convertView, View parent)
        {
            AgendaHeaderView headerView;
            if (convertView == null)
                headerView = new AgendaHeaderView(parent.Context);
            else
                headerView = (AgendaHeaderView)convertView;
            headerView.LocationView.Text = section.Location;
            return headerView;
        }

        public void SetupDateButtons(ObservableList<AgendaViewModel.AgendaDate> agendaDates, DateTime selected)
        {
            foreach (AgendaViewModel.AgendaDate date in agendaDates)
            {
                Tabs.AddTab(Tabs.NewTab().SetText(ServiceLocator.Instance.DateTimeService.DateToDisplayString(date.Date).ToUpper()), date.Date.Equals(selected));
            }
            //workaround for tablayout issue https://code.google.com/p/android/issues/detail?id=181665
            Handler h = new Handler();
            Action myAction = () =>
            {
                int position = Tabs.SelectedTabPosition;
                if (position != -1)
                    Tabs.GetTabAt(position).Select();
            };

            h.PostDelayed(myAction, 300);
        }

        public void OnTabReselected(TabLayout.Tab tab)
        {
            //do nothing
        }

        public void OnTabSelected(TabLayout.Tab tab)
        {
            DateClickAction(tab.Position);
        }

        public void OnTabUnselected(TabLayout.Tab tab)
        {
            //do nothing
        }

        public void OnHeaderClick(StickyListHeadersListView p0, View p1, int position, long p3, bool p4)
        {
            HeaderClickAction(position);
        }
    }
}
