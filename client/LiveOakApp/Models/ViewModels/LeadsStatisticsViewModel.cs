using System;
using System.Threading.Tasks;
using LiveOakApp.Models.Data.NetworkDTO;
using StudioMobile;
using System.Linq;

namespace LiveOakApp.Models.ViewModels
{
    public class LeadsStatisticsViewModel : DataContext
    {
        Field<int> _allTimeCount;
        Field<int> _lastPeriodCount;
        Field<int> _lastPeriodGoal;

        public int AllTimeCount 
        { 
            get
            {
                return _allTimeCount.Value;
            }
            set
            {
                _allTimeCount.SetValue(value);
            }
        }
        public int LastPeriodCount 
        {
            get
            {
                return _lastPeriodCount.Value;
            }
            set
            {
                _lastPeriodCount.SetValue(value);
                RaisePropertyChanged(() => LastPeriodCountPercents);
                RaisePropertyChanged(() => LastPeriodGoalPercents);
            }
        }        
        public int LastPeriodGoal 
        {
            get
            {
                return _lastPeriodGoal.Value;
            }
            set
            {
                _lastPeriodGoal.SetValue(value);
                RaisePropertyChanged(() => LastPeriodCountPercents);
                RaisePropertyChanged(() => LastPeriodGoalPercents);
            }
        }
        public int LastPeriodCountPercents
        {
            get
            {
                var percentage = (int)Math.Truncate(((double)LastPeriodCount / (double)LastPeriodGoal) * 100);
                percentage = Math.Min(percentage, 100);
                percentage = Math.Max(0, percentage);
                return percentage;
            }
        }
        public int LastPeriodGoalPercents
        {
            get
            {
                return 100 - LastPeriodCountPercents;
            }
        }
        public MoneyViewModel ThisYearExpenses { get; private set; }
        public MoneyViewModel ThisYearCostPerLead { get; private set; }

        DashboardViewModel ParentViewModel { get; set; }

        public LeadsStatisticsViewModel(LeadsStatisticsDTO leadStatisticsDTO, DashboardViewModel parentViewModel)
        {
            Check.Argument(leadStatisticsDTO, nameof(leadStatisticsDTO)).NotNull();
            Check.Argument(parentViewModel, nameof(parentViewModel)).NotNull();

            ThisYearExpenses = new MoneyViewModel();
            ThisYearCostPerLead = new MoneyViewModel();

            _allTimeCount = Value(leadStatisticsDTO.AllTimeCount);
            _lastPeriodGoal = Value(leadStatisticsDTO.LastPeriodGoal);
            _lastPeriodCount = Value(leadStatisticsDTO.LastPeriodCount);

            ThisYearExpenses.UpdateFromDTO(leadStatisticsDTO.ThisYearExpenses);
            ThisYearCostPerLead.UpdateFromDTO(leadStatisticsDTO.ThisYearCostPerLead);

            ParentViewModel = parentViewModel;
            SendExpenseCommand = new AsyncCommand()
            {
                Action = SendExpenseAction,
                CanExecute = CanExecuteSendExpense
            };
        }

        public void UpdateFromDTO(LeadsStatisticsDTO dto) 
        {
            AllTimeCount = dto.AllTimeCount;
            LastPeriodCount = dto.LastPeriodCount;
            LastPeriodGoal = dto.LastPeriodGoal;

            ThisYearExpenses.UpdateFromDTO(dto.ThisYearExpenses);
            ThisYearCostPerLead.UpdateFromDTO(dto.ThisYearCostPerLead);
        }

        bool CanExecuteSendExpense(object arg)
        {
            return !SendExpenseCommand.IsRunning;
        }

        public AsyncCommand SendExpenseCommand { get; private set; }
        async Task SendExpenseAction(object arg)
        {
            if (arg is EventExpenseDTO)
                await ParentViewModel.DashboardService.AddExpense((EventExpenseDTO)arg);
        }

        public async Task AddExpense(EventViewModel @event, decimal newExpense, string description)
        {
            var expense = new EventExpenseDTO()
            {
                EventUid = new Guid(@event.UID),
                Expense = new MoneyViewModel(newExpense, ThisYearExpenses.Currency).MoneyDTO,
                Comments = description
            };
            await SendExpenseCommand.ExecuteAsync(expense);
            if (ThisYearExpenses.Amount != 0 && ThisYearCostPerLead.Amount != 0)
            {
                var thisYearLeadsCount = ThisYearExpenses.Amount / ThisYearCostPerLead.Amount;
                ThisYearCostPerLead.Amount = (ThisYearExpenses.Amount + newExpense) / thisYearLeadsCount;
            }
            ThisYearExpenses.Amount += newExpense;
            var dashboardEventVM = ParentViewModel.Events.FirstOrDefault((arg) => arg.Uid.ToString().Equals(@event.UID));
            if(dashboardEventVM != null)
                dashboardEventVM.TotalExpenses.Amount += newExpense;
            ParentViewModel.LeadStatisticsChanged();
        }
    }
}
