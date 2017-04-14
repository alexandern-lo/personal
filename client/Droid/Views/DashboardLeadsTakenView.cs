using System;
using Android.Content;
using Android.Util;
using Android.Widget;
using LiveOakApp.Models.ViewModels;
using StudioMobile;

namespace LiveOakApp.Droid.Views
{
    public class DashboardLeadsTakenView : CustomBindingsView
    {
        public DashboardLeadsTakenView(Context context) :
            base(context)
        {
            Initialize();
        }

        public DashboardLeadsTakenView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public DashboardLeadsTakenView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }
        DashboardEventViewModel _viewModel;
        public DashboardEventViewModel ViewModel 
        {
            get
            {
                return _viewModel;
            } 
            set
            {
                Bindings.Clear();
                _viewModel = value;

                Bindings.Property(ViewModel, _ => _.Name)
                        .To(Title.TextProperty());
                Bindings.Property(ViewModel, _ => _.LeadsCount)
                        .Convert((number) => number.ToString())
                        .To(LeadsTaken.TextProperty());
                Bindings.Property(ViewModel, _ => _.LeadsGoal)
                        .Convert((number) => number.ToString())
                        .To(Goal.TextProperty());
                Bindings.Property(ViewModel, _ => _.LeadsGoalProgress)
                        .Convert<int>((progress) => (int)(progress * Progress.Max))
                        .UpdateTarget((progress) =>
                {
                    Progress.Progress = progress.Value;
                });
            }
        }

        public TextView Title { get; private set; }
        public TextView LeadsTaken { get; private set; }
        public ProgressBar Progress { get; private set; }
        public TextView Goal { get; private set; }
        public ImageButton EditGoalButton { get; private set; }

        public Action<DashboardEventViewModel> EditGoalAction { get; set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.DashboardLeadsTakenItemLayout, this);
            Title = FindViewById<TextView>(Resource.Id.event_title);
            LeadsTaken = FindViewById<TextView>(Resource.Id.leads_taken);
            Progress = FindViewById<ProgressBar>(Resource.Id.progress_bar);
            Goal = FindViewById<TextView>(Resource.Id.goal_number);
            EditGoalButton = FindViewById<ImageButton>(Resource.Id.edit_goal_button);
            EditGoalButton.Click += (sender, e) =>
            {
                if (EditGoalAction == null) return;
                EditGoalAction(ViewModel);
            };
        }
    }
}
