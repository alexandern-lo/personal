using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Avend.ApiTests.Infrastructure;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Events.NetworkDTO;
using Qoden.Validation;

namespace Avend.ApiTests.DataSamples
{
    public class EventExpenseDataFixtures
    {
        public TestSystem System { get; }
        public TestUser User { get; }

        public EventDto Event { get; }
        public List<EventUserExpenseDto> EventExpenses { get; }

        public EventExpenseDataFixtures(TestUser user, TestSystem system, EventDto eventDto)
        {
            Assert.Argument(user, nameof(user)).NotNull();
            Assert.Argument(system, nameof(system)).NotNull();
            Assert.Argument(eventDto, nameof(eventDto)).NotNull();

            User = user;
            System = system;
            Event = eventDto;

            EventExpenses = new List<EventUserExpenseDto>();
        }

        public async Task<EventUserExpenseDto> AddFromDto(EventUserExpenseDto dto)
        {
            EventUserExpenseDto addedDto;

            using (var http = System.CreateClient(User.Token))
            {
                addedDto = await http.PostJsonAsync($"events/{dto.EventUid}/expenses", dto).AvendResponse<EventUserExpenseDto>();

                EventExpenses.Add(addedDto);
            }

            return addedDto;
        }

        public async Task<EventUserExpenseDto> AddFromSample(Action<EventUserExpenseDto> postProcessor = null)
        {
            var expenseDto = MakeSample(Event.Uid.Value, postProcessor);

            return await AddFromDto(expenseDto);
        }

        #region Static methods and fields

        private static int amountValue = 100;
        private static int commentIndex = 0;

        public static async Task<EventExpenseDataFixtures> InitWithSampleExpense(TestUser user, TestSystem system, EventDto eventDto)
        {
            var fixtures = new EventExpenseDataFixtures(user, system, eventDto);

            await fixtures.AddFromSample();

            return fixtures;
        }

        public EventUserExpenseDto MakeSample(Guid eventUid, Action<EventUserExpenseDto> postProcessor = null)
        {
            amountValue++;
            commentIndex++;

            var newEventExpenseDto = new EventUserExpenseDto()
            {
                EventUid = eventUid,
                Comments = $"Comments N{commentIndex} from " + User.FirstName,
                Expense = new MoneyDto()
                {
                    Currency = CurrencyCode.USD,
                    Amount = amountValue,
                }
            };

            postProcessor?.Invoke(newEventExpenseDto);

            return newEventExpenseDto;
        }

        #endregion
    }
}