using System;
using System.Threading;
using System.Threading.Tasks;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.Models.Services
{
    public class EventService
    {
        readonly ApiService ApiService;

        public EventService(ApiService apiService)
        {
            ApiService = apiService;
        }

        async public Task<MoneyDTO> GetTotalExpenses(string eventUID, CancellationToken? cancellationToken)
        {
            var result = await ApiService.GetTotalExpenses(eventUID, cancellationToken);
            return result.Content;
        }

        public async Task AddExpense(EventExpenseDTO eventExpenseDTO)
        {
            await ApiService.AddExpense(eventExpenseDTO, null);
        }
    }
}
