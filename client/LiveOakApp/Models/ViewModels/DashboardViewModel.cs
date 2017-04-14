using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Services;
using StudioMobile;
using System.Linq;

namespace LiveOakApp.Models.ViewModels
{
    public class DashboardViewModel : DataContext
    {
        public DashboardService DashboardService = ServiceLocator.Instance.DashboardService;
        public AsyncCommand LoadDashboardCommand { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public LeadsStatisticsViewModel LeadsStatistics { get; private set; }
        public ObservableList<DashboardEventViewModel> Events  = new ObservableList<DashboardEventViewModel>();
        public ObservableList<DashboardEventViewModel> EventsGoals = new ObservableList<DashboardEventViewModel>();
        public ObservableList<DashboardResourceViewModel> Resources = new ObservableList<DashboardResourceViewModel>();
        public ObservableList<ObservableList<DataContext>> AllTableBindingData { get; private set; } = new ObservableList<ObservableList<DataContext>>();
        const int RESOURCES_DATA_MAX_COUNT = 6;
        const int EVENT_EXPENSES_DATA_MAX_COUNT = 8;
        const int EVENT_GOALS_DATA_MAX_COUNT = 4;
        const int EVENT_GOALS_DATA_INDEX = 0;
        const int RESOURCES_DATA_INDEX = 1;
        const int EVENT_EXPENSES_DATA_INDEX = 2;

        public DashboardViewModel()
        {
            LeadsStatistics = new LeadsStatisticsViewModel(new LeadsStatisticsDTO(), this);

            LoadDashboardCommand = new AsyncCommand
            {
                Action = LoadDashboardAction,
                CanExecute = CanExecuteLoadDashboard
            };
        }

        bool CanExecuteLoadDashboard(object arg)
        {
            return !LoadDashboardCommand.IsRunning;
        }

        async Task LoadDashboardAction(object arg)
        {
            DashboardDTO dashboardDTO = await DashboardService.GetDashboard(LoadDashboardCommand.Token, 6, 8);
            if (dashboardDTO == null) return;

            CreatedAt = dashboardDTO.CreatedAt;
            LeadsStatistics.UpdateFromDTO(dashboardDTO.LeadsStatistics);
            Events.Reset(dashboardDTO.Events.ConvertAll(_ => new DashboardEventViewModel(_, this)));
            EventsGoals.Reset(dashboardDTO.Events.GetRange(0, Math.Min(EVENT_GOALS_DATA_MAX_COUNT, dashboardDTO.Events.Count)).ConvertAll(_ => new DashboardEventViewModel(_, this)));
            Resources.Reset(dashboardDTO.Resources.ConvertAll(_ => new DashboardResourceViewModel(_)));

            var goalStatisticsEvents = new List<DashboardEventViewModel>();
            for (int i = 0; i < EVENT_GOALS_DATA_MAX_COUNT && i < Events.Count; i++)
                goalStatisticsEvents.Add(Events[i]);
            AllTableBindingData.Clear();
            AllTableBindingData.AddRange(new ObservableList<ObservableList<DataContext>> {
                new ObservableList<DataContext>(EventsGoals.ToList().ConvertAll((input) => (DataContext)input)),
                new ObservableList<DataContext>(Resources.ToList().ConvertAll((input) => (DataContext)input)),
                new ObservableList<DataContext>(Events.ToList().ConvertAll((input) => (DataContext)input))
            });
            LeadStatisticsChanged();
        }

        public decimal GetMaxEventExpenses()
        {
            decimal maxEventExpenses = 0;
            if (AllTableBindingData.Count == EVENT_EXPENSES_DATA_INDEX + 1)
            {
                var dashboardEvents = new List<DataContext>(AllTableBindingData[EVENT_EXPENSES_DATA_INDEX]).ConvertAll((input) => (DashboardEventViewModel)input);
                foreach (DashboardEventViewModel dashboardEvent in dashboardEvents)
                    if (dashboardEvent.TotalExpenses.Amount > maxEventExpenses) maxEventExpenses = dashboardEvent.TotalExpenses.Amount;
            }
            return maxEventExpenses;
        }

        public void LeadStatisticsChanged()
        {
            RaisePropertyChanged(() => LeadsStatistics);
        }
    }
}
