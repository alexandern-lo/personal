using System.Net;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.Events
{
    [TestClass]
    public class EventGetExpensesTest : EventsControllerTestBase
    {
        [TestMethod]
        public async Task GetEventExpensesForInaccessibleEvent()
        {
            await BobTA.GetJsonAsync($"events/{MarcEvent.Uid}/expenses_total")
                .Response(HttpStatusCode.NotFound, "Cannot access event from another subscription even by tenant admin");
        }

        [TestMethod]
        [TestCategory("Integrational")]
        [TestCategory("EventsController")]
        [TestCategory("EventsController.GetEventExpenses()")]
        public async Task GetEventExpensesByEventUid()
        {
            var totalAmount = 0M;

            var aliceExpenseFixtures = new EventExpenseDataFixtures(TestUser.AliceTester, System, AlexEvent);

            var expense0 = await aliceExpenseFixtures.AddFromSample();

            expense0.Should()
                .NotBeNull("because the event expense for Alice should be added smoothly");

            var bobExpenseFixtures = new EventExpenseDataFixtures(TestUser.BobTester, System, AlexEvent);

            var expense1 = await bobExpenseFixtures.AddFromSample();

            totalAmount += expense1.Expense.Amount;

            var expense2 = await bobExpenseFixtures.AddFromSample();

            totalAmount += expense2.Expense.Amount;

            var totalExpenses = await BobTA.GetJsonAsync($"events/{AlexEvent.Uid}/expenses_total")
                .AvendResponse<MoneyDto>();

            totalExpenses.Currency.Should()
                .NotBeNull("because returned data structure should be a correct MoneyDto for Bob");

            totalExpenses.Currency.Should()
                .Be(CurrencyCode.USD, "because returned currency should be correct for Bob");

            totalExpenses.Amount.Should()
                .Be(totalAmount, "because returned amount should be a total of Bob's expenses");
        }

        [TestMethod]
        public async Task GetEventExpensesByEventUidWithoutExpenses()
        {
            var meetingEvent = await BobTA.GetJsonAsync($"events/{BobEvent.Uid}/expenses_total")
                .AvendResponse<MoneyDto>();

            meetingEvent.Currency.Should()
                .Be(CurrencyCode.USD, "because returned currency when no expenses should be USD");

            meetingEvent.Amount.Should()
                .Be(0.00M, "because returned amount should be zero");
        }
    }
}