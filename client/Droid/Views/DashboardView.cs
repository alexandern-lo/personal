
using System;
using System.Collections.Generic;
using Android.Animation;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using LiveOakApp.Droid.CustomViews;
using LiveOakApp.Models.ViewModels;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Data;
using StudioMobile;

namespace LiveOakApp.Droid.Views
{
    public class DashboardView : FrameLayout
    {
        public DashboardView(Context context) :
            base(context)
        {
            Initialize();
        }

        public DashboardView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public DashboardView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        bool isShowingProgressBar;
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
                        AnimateShowProgressBar();
                    }
                    else
                    {
                        ProgressBar.Visibility = ViewStates.Visible;
                        ContentView.SetY(TypedValue.ApplyDimension(ComplexUnitType.Dip, 50, Context.Resources.DisplayMetrics));
                    }
                }
                else if (ViewCompat.IsLaidOut(this))
                {
                    AnimateHideProgressBar();
                }
            }
        }

        void AnimateShowProgressBar()
        {
            var animator = CreateContentScroll(0, ProgressBar.Height);
            animator.AnimationStart += (sender, e) => ProgressBar.Visibility = ViewStates.Visible; LeadStatsChart.Invalidate();
            animator.Start();
        }

        void AnimateHideProgressBar()
        {
            var animator = CreateContentScroll(ProgressBar.Height, 0);
            animator.AnimationEnd += (sender, e) => ProgressBar.Visibility = ViewStates.Invisible; LeadStatsChart.Invalidate();
            animator.Start();
        }

        ValueAnimator CreateContentScroll(int y0, int y1)
        {
            var animator = ValueAnimator.OfInt(y0, y1);
            animator.SetDuration(500);
            animator.Update += (sender, e) => ContentView.SetY((int)e.Animation.AnimatedValue);
            return animator;
        }


        public TextView LeadsMonthly { get; private set; }
        public TextView LeadsTotal { get; private set; }
        public TextView LeadsPeriodPercent { get; private set; }
        public TextView LeadsGoalPercent { get; private set; }
        public ExpandedListView LeadsTakenList { get; private set; }
        public ExpandedListView ResourcesList { get; private set; }
        public ExpandedListView CostPerLeadList { get; private set; }
        public Button AddExpenseButton { get; private set; }
        public TextView YearlyExpenses { get; private set; }
        public TextView AverageLeadCost { get; private set; }
        public PieChart LeadStatsChart { get; private set; }
        public ImageButton LeadsTab { get; private set; }
        public ImageButton EventsTab { get; private set; }
        public ImageButton RecentActivityTab { get; private set; }
        public ImageButton ResourcesTab { get; private set; }
        public SwipeRefreshLayout DashboardRefresher { get; private set; }
        public PieDataSet LeadStatsDataSet { get; private set; }
        public View ProgressBar { get; private set; }
        public View ContentView { get; private set; }
        public View EventGoalsHolder { get; private set; }
        public View ResourcesHolder { get; private set; }
        public View CostPerLeadHolder { get; private set; }
        public ErrorMessageView ErrorView { get; private set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.DashboardLayout, this);
            LeadsMonthly = FindViewById<TextView>(Resource.Id.leads_number);
            LeadsTotal = FindViewById<TextView>(Resource.Id.leads_total_number);
            LeadsPeriodPercent = FindViewById<TextView>(Resource.Id.leads_for_period_percent);
            LeadsGoalPercent = FindViewById<TextView>(Resource.Id.leads_goal_percent);
            LeadsTakenList = FindViewById<ExpandedListView>(Resource.Id.leads_taken_list);
            ResourcesList = FindViewById<ExpandedListView>(Resource.Id.resources_list);
            CostPerLeadList = FindViewById<ExpandedListView>(Resource.Id.cost_per_lead_list);
            AddExpenseButton = FindViewById<Button>(Resource.Id.add_expense_button);
            YearlyExpenses = FindViewById<TextView>(Resource.Id.yearly_expenses);
            AverageLeadCost = FindViewById<TextView>(Resource.Id.average_lead_cost);
            LeadStatsChart = FindViewById<PieChart>(Resource.Id.leads_chart);
            LeadsTab = FindViewById<ImageButton>(Resource.Id.leads_tab);
            EventsTab = FindViewById<ImageButton>(Resource.Id.events_tab);
            RecentActivityTab = FindViewById<ImageButton>(Resource.Id.recent_activity_tab);
            ResourcesTab = FindViewById<ImageButton>(Resource.Id.resources_tab);
            DashboardRefresher = FindViewById<SwipeRefreshLayout>(Resource.Id.dashboard_refresher);
            ProgressBar = FindViewById(Resource.Id.progressBar);
            ContentView = FindViewById(Resource.Id.content_view);
            EventGoalsHolder = FindViewById(Resource.Id.leads_taken_holder);
            ResourcesHolder = FindViewById(Resource.Id.resources_holder);
            CostPerLeadHolder = FindViewById(Resource.Id.cost_per_lead_holder);
            ErrorView = FindViewById<ErrorMessageView>(Resource.Id.error_view);

            ErrorView.SetShowsButton(true);

            var dataSetValues = new List<PieEntry>(new PieEntry[] {new PieEntry(0), new PieEntry(0)});
            LeadStatsDataSet = new PieDataSet(dataSetValues, "stats");

            SetupPieChart(Context, LeadStatsChart, LeadStatsDataSet);

            ChildrenBindingList = new WeakBindingList();
        }
        static void SetupPieChart(Context context, PieChart chart, PieDataSet dataSet) 
        {
            chart.DrawHoleEnabled = true;
            chart.LogEnabled = false;
            chart.HighlightPerTapEnabled = false;
            chart.Legend.Enabled = false;
            chart.Description.Enabled = false;
            chart.HoleRadius = 80; // percents
            chart.RotationEnabled = false;
            chart.SetTransparentCircleAlpha(0);
            dataSet.SetColors(new int[]
            {
                ContextCompat.GetColor(context, Resource.Color.primary_dark),
                ContextCompat.GetColor(context, Resource.Color.primary_blue)
            });
            dataSet.SliceSpace = 10;
            dataSet.SetDrawValues(false);
            chart.Data = new PieData(dataSet);
        }

        public WeakBindingList ChildrenBindingList { get; private set; }

        public void ShowError()
        {
            ErrorView.Visibility = ViewStates.Visible;
        }
        public void HideError()
        {
            ErrorView.Visibility = ViewStates.Gone;
        }

        public void ShowEventsLists()
        {
            EventGoalsHolder.Visibility = ViewStates.Visible;
            CostPerLeadHolder.Visibility = ViewStates.Visible;
        }
        public void HideEventsLists()
        {
            EventGoalsHolder.Visibility = ViewStates.Gone;
            CostPerLeadHolder.Visibility = ViewStates.Gone;
        }

        public void ShowResourcesList()
        {
            ResourcesHolder.Visibility = ViewStates.Visible;
        }
        public void HideResourcesList()
        {
            ResourcesHolder.Visibility = ViewStates.Gone;
        }

        #region Adapters

        public ObservableAdapter<DashboardEventViewModel> GetEventLeadsTakenAdapter(ObservableList<DashboardEventViewModel> events)
        {
            return new ObservableAdapter<DashboardEventViewModel>
            {
                DataSource = events,
                ViewFactory = GetEventLeadsTakenView
            };
        }

        public Action<DashboardEventViewModel> EditGoalAction { get; set; }

        View GetEventLeadsTakenView(int position, DashboardEventViewModel eventViewModel, View convertView, View parent)
        {
            var view = (DashboardLeadsTakenView)convertView;
            if (view == null)
            {
                view = new DashboardLeadsTakenView(parent.Context);
                var bindings = new BindingList();
                view.ResetBingings(bindings);
                ChildrenBindingList.Add(bindings);
            }
            view.EditGoalAction = EditGoalAction;
            view.ViewModel = eventViewModel;

            return view;
        }


        public ObservableAdapter<DashboardEventViewModel> GetEventLeadCostAdapter(ObservableList<DashboardEventViewModel> events, float maxExpenses)
        {
            return new ObservableAdapter<DashboardEventViewModel>
            {
                DataSource = events,
                ViewFactory = (position, eventViewModel, convertView, parent) => GetEventLeadCostView(position, eventViewModel, convertView, parent, maxExpenses)
            };
        }

        View GetEventLeadCostView(int position, DashboardEventViewModel eventViewModel, View convertView, View parent, float maxExpenses)
        {
            var view = (DashboardLeadCostView)convertView;
            if (view == null)
            {
                view = new DashboardLeadCostView(parent.Context);
                ChildrenBindingList.Add(view.Bindings);
            }
            view.MaxExpenses = maxExpenses;
            view.ViewModel = eventViewModel;

            return view;
        }
        public ObservableAdapter<DashboardResourceViewModel> GetResourcesAdapter(ObservableList<DashboardResourceViewModel> resources)
        {
            return new ObservableAdapter<DashboardResourceViewModel>
            {
                DataSource = resources,
                ViewFactory = GetResourceView
            };
        }
        View GetResourceView(int position, DashboardResourceViewModel resourceViewModel, View convertView, View parent)
        {
            var view = (DashboardResourceView)convertView;
            if (view == null)
            {
                view = new DashboardResourceView(parent.Context);
                ChildrenBindingList.Add(view.Bindings);
            }
            view.ViewModel = resourceViewModel;
            return view;
        }

        #endregion
    }
}
