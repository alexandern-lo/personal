using Android.Content;
using Android.Util;
using Android.Widget;
using LiveOakApp.Models.ViewModels;
using StudioMobile;
using System;

namespace LiveOakApp.Droid.Views
{
    public class DashboardLeadCostView : CustomBindingsView
    {
        public DashboardLeadCostView(Context context) :
            base(context)
        {
            Initialize();
        }

        public DashboardLeadCostView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public DashboardLeadCostView(Context context, IAttributeSet attrs, int defStyle) :
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
                Bindings.Property(ViewModel.TotalExpenses, _ => _.Amount)
                        .Convert((amount) => ViewModel.TotalExpenses.GetCurrencySymbol() + amount.ToShortNumber())
                        .To(Spent.TextProperty());
                Bindings.Property(ViewModel, _ => _.TotalExpenses)
                        .Convert<int>((progress) => (int)Math.Round(ViewModel.GetMoneySpentProgress(MaxExpenses) * Progress.Max, MidpointRounding.AwayFromZero))
                        .UpdateTarget((progress) =>
                {
                    Progress.Progress = progress.Value;
                });
                Bindings.Property(ViewModel.LeadCost, _ => _.Amount)
                        .Convert((amount) => ViewModel.LeadCost.GetCurrencySymbol() + amount.ToShortNumber())
                        .To(LeadCost.TextProperty());
            }
        }

        public TextView Title { get; private set; }
        public TextView Spent { get; private set; }
        public ProgressBar Progress { get; private set; }
        public TextView LeadCost { get; private set; }

        public float MaxExpenses { get; set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.DashboardLeadCostItemLayout, this);
            Title = FindViewById<TextView>(Resource.Id.event_title);
            Spent = FindViewById<TextView>(Resource.Id.event_spent);
            Progress = FindViewById<ProgressBar>(Resource.Id.progress_bar);
            LeadCost = FindViewById<TextView>(Resource.Id.lead_cost);
        }
    }
}
