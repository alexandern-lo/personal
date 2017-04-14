using System;
using System.Threading.Tasks;
using LiveOakApp.Models.Data.NetworkDTO;
using StudioMobile;

namespace LiveOakApp.Models.ViewModels
{
    public class DashboardEventViewModel : DataContext
    {
        public MoneyViewModel LeadCost { get; private set; }
        public MoneyViewModel TotalExpenses { get; private set; }

        Field<Guid> _uid;
        Field<string> _name;
        Field<string> _websiteUrl;
        Field<int> _leadsGoal;
        Field<int> _leadsCount;

        public Guid Uid 
        {
            get
            { 
                return _uid.Value; 
            }
            set
            {
                _uid.SetValue(value);
            }
        }
        public string Name 
        {
            get
            {
                return _name.Value;
            }
            set
            {
                _name.SetValue(value);
            }
        }
        public string WebsiteUrl 
        {
            get
            {
                return _websiteUrl.Value;
            }
            set
            {
                _websiteUrl.SetValue(value);
            }
        }
        public int LeadsGoal
        {
            get
            {
                return _leadsGoal.Value;
            }
            set
            {
                _leadsGoal.SetValue(value);
                RaisePropertyChanged(() => LeadsGoalProgress);
            }
        }
        public int LeadsCount
        {
            get
            {
                return _leadsCount.Value;
            }
            set
            {
                _leadsCount.SetValue(value);
                RaisePropertyChanged(() => LeadsGoalProgress);
                if (LeadsCount != 0 && TotalExpenses.Amount > 0)
                    LeadCost.Amount = TotalExpenses.Amount / LeadsCount;
                else
                    LeadCost.Amount = 0;
            }
        }
        public float LeadsGoalProgress
        {
            get
            {
                return (float)LeadsCount / LeadsGoal;
            }
        }
        public float GetMoneySpentProgress(float maxExpenses)
        {
            return (float)TotalExpenses.Amount / maxExpenses;
        }

        DashboardViewModel ParentViewModel { get; set; }

        public DashboardEventViewModel(DashboardEventDTO dashboardEventDTO, DashboardViewModel parentViewModel)
        {
            Check.Argument(dashboardEventDTO, nameof(dashboardEventDTO)).NotNull();
            Check.Argument(parentViewModel, nameof(parentViewModel)).NotNull();

            TotalExpenses = new MoneyViewModel();
            TotalExpenses.UpdateFromDTO(dashboardEventDTO.TotalExpenses);

            _uid = Value(dashboardEventDTO.Uid);
            _name = Value(dashboardEventDTO.Name);
            _leadsCount = Value(dashboardEventDTO.LeadsCount);
            _leadsGoal = Value(dashboardEventDTO.LeadsGoal);
            _websiteUrl = Value(dashboardEventDTO.WebsiteUrl);

            LeadCost = new MoneyViewModel()
            {
                Amount = 0,
                Currency = TotalExpenses.Currency
            };
            if (LeadsCount != 0 && TotalExpenses.Amount > 0)
                LeadCost.Amount = TotalExpenses.Amount / LeadsCount;

            ParentViewModel = parentViewModel;
            SendEditGoalCommand = new AsyncCommand()
            {
                Action = SendEditGoalAction,
                CanExecute = CanExecuteSendEditGoal
            };
        }

        bool CanExecuteSendEditGoal(object arg)
        {
            return !SendEditGoalCommand.IsRunning;
        }

        public AsyncCommand SendEditGoalCommand { get; private set; }
        async Task SendEditGoalAction(object arg)
        {
            if (arg is EventUserGoalDTO)
                await ParentViewModel.DashboardService.EditEventGoal((EventUserGoalDTO)arg);
        }

        public async Task EditEventGoal(int newGoal)
        {
            var eventUserGoal = new EventUserGoalDTO()
            {
                EventUid = Uid,
                LeadsGoal = newGoal
            };
            await SendEditGoalCommand.ExecuteAsync(eventUserGoal);
            var eventIndex = ParentViewModel.AllTableBindingData[0].IndexOf(this);
            LeadsGoal = newGoal;
            ParentViewModel.AllTableBindingData[0][eventIndex] = this;
        }
    }
}
