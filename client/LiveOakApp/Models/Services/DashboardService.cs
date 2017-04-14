using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiveOakApp.Models.Data.NetworkDTO;
using SL4N;

namespace LiveOakApp.Models.Services
{
    public class DashboardService
    {
        readonly ApiService ApiService;

        public DashboardService(ApiService apiService)
        {
            ApiService = apiService;
        }

        public async Task<DashboardDTO> GetDashboard(CancellationToken? cancellationToken, int resourcesLimit, int eventsLimit)
        {


            var result = await ApiService.GetDashboard(cancellationToken, resourcesLimit, eventsLimit);
            if (result.Status == ApiResultStatus.Ok)
                return result.Content;
            else
                throw new ArgumentOutOfRangeException();
        }

        public async Task EditEventGoal(EventUserGoalDTO eventUserGoal)
        {
            await ApiService.SetNewEventGoal(eventUserGoal, null);
        }

        public async Task AddExpense(EventExpenseDTO eventExpenseDTO)
        {
            await ApiService.AddExpense(eventExpenseDTO, null);
        }
    }
}
