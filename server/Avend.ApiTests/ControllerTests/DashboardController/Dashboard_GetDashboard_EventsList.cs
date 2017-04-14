using System;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model;
using Avend.API.Services.Dashboard.NetworkDTO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.DashboardController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("DashboardController")]
    [TestCategory("DashboardController.GetDashBoard()")]
    // ReSharper disable once InconsistentNaming
    public class Dashboard_GetDashboard_EventsList : BaseDashboardEndpointTest
    {
        public const string EventsUrl = "events";
        public const string DashboardUrl = "dashboard";

        [TestMethod]
        public async Task ShouldReturnProperSingleEventWhenOnlyOneOngoingConferenceEventIsAddedAndNoSortFieldPassed()
        {
            var confEventDto = await ConferenceEventData.AddFromSample();

            var responseJson = BobTA.GetJsonAsync(DashboardUrl);

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.Events.Should()
                .HaveCount(1, "because when no sorting field selected ongoing conference event should show up in dashboard")
                .And
                .Contain(eventDTO => eventDTO.Uid == confEventDto.Uid, "because the UID of the returned event should match the one we've added");
        }

        [TestMethod]
        public async Task ShouldReturnProperSingleEventWhenOnlyOneRecentConferenceEventIsAddedAndNoSortFieldPassed()
        {
            var confEventDto = await ConferenceEventData.AddFromSample(dto =>
            {
                dto.StartDate = DateTime.UtcNow.AddDays(-5);
                dto.EndDate = DateTime.UtcNow.AddDays(-2);
            });

            var responseJson = BobTA.GetJsonAsync(DashboardUrl);

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.Events.Should()
                .HaveCount(1, "because when no sorting field selected ongoing conference event should show up in dashboard")
                .And
                .Contain(eventDTO => eventDTO.Uid == confEventDto.Uid, "because the UID of the returned event should match the one we've added");
        }

        [TestMethod]
        public async Task ShouldReturnEmptyListWhenOnlyPersonalNonRecurringEventAddedAndNoSortFieldPassed()
        {
            var bobEventData = new EventData(TestUser.BobTester, System);

            await bobEventData.AddFromSample();

            var responseJson = BobTA.GetJsonAsync(DashboardUrl);

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.Events.Should()
                .HaveCount(0, "because when no sorting field selected non-recurring personal events are not showing in dashboard");
        }

        [TestMethod]
        public async Task ShouldReturnEmptyListWhenConferenceEventIsDeletedAndNoSortFieldPassed()
        {
            var confEventDto = await ConferenceEventData.AddFromSample();

            var eventDeletedResponse = await AlexSA.DeleteJsonAsync(EventsUrl + "/" + confEventDto.Uid).AvendResponse<Guid>();

            eventDeletedResponse.Should()
                .NotBeEmpty("because response should contain a valid Uid")
                .And
                .Be(confEventDto.Uid.Value, "because response should contain the same Uid of the deleted event")
                ;

            var avendResponse = await BobTA.GetJsonAsync(DashboardUrl)
                .AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.Events.Should()
                .HaveCount(0, "because we have deleted a single conference event in database now");
        }

        [TestMethod]
        public async Task ShouldReturnEmptyListWhenRecurringEventIsDeletedAndNoSortFieldPassed()
        {
            var bobEventData = new EventData(TestUser.BobTester, System);

            var recurringEventDto = await bobEventData.AddFromSample(dto =>
            {
                dto.Recurring = true;
                dto.EndDate = null;
            });

            var eventDeletedResponse = await BobTA.DeleteJsonAsync(EventsUrl + "/" + recurringEventDto.Uid).AvendResponse<Guid>();

            eventDeletedResponse.Should()
                .NotBeEmpty("because response should contain a valid Uid")
                .And
                .Be(recurringEventDto.Uid.Value, "because response should contain the same Uid of the deleted event")
                ;

            var responseJson = BobTA.GetJsonAsync(DashboardUrl);

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.Events.Should()
                .HaveCount(0, "because we have deleted a single recurring event in database now");
        }

        [TestMethod]
        public async Task ShouldReturnProperSingleEventWhenGoalsAddedByTenantAdminForHimselfForSingleRecurringEventAndNoSortFieldPassed()
        {
            var confEventDto = await ConferenceEventData.AddFromSample();

            var aliceGoalsDataFixtures = new EventGoalsDataFixtures(TestUser.AliceTester, System, confEventDto);
            // ReSharper disable once UnusedVariable
            var aliceGoals = await aliceGoalsDataFixtures.AddFromSample();

            var bobGoalsDataFixtures = new EventGoalsDataFixtures(TestUser.BobTester, System, confEventDto);
            var bobGoals = await bobGoalsDataFixtures.AddFromSample();

            var responseJson = BobTA.GetJsonAsync(DashboardUrl);

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.Events.Should()
                .HaveCount(1, "because we have a single event in database now")
                .And
                .Contain(eventDTO => eventDTO.Uid == confEventDto.Uid, "because the UID of the returned event should match the one we've added");

            avendResponse.LeadsStatistics.AllTimeGoal.Should()
                .Be(bobGoals.LeadsGoal, "because we added a single leads goal for Bob");
        }

        [TestMethod]
        public async Task ShouldReturnProperSingleEventWhenTwoExpensesAddedBySameUserForSingleRecurringEventAndNoSortFieldPassed()
        {
            var confEventDto = await ConferenceEventData.AddFromSample();

            var aliceExpensesDataFixtures = new EventExpenseDataFixtures(TestUser.AliceTester, System, confEventDto);
            // ReSharper disable once UnusedVariable
            var aliceExpense = await aliceExpensesDataFixtures.AddFromSample();

            var bobExpenseDataFixtures = new EventExpenseDataFixtures(TestUser.BobTester, System, confEventDto);
            var bobExpense1 = await bobExpenseDataFixtures.AddFromSample();
            var bobExpense2 = await bobExpenseDataFixtures.AddFromSample();

            var responseJson = BobTA.GetJsonAsync(DashboardUrl);

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.Resources.Should()
                .HaveCount(0, "because in empty database no resources could exist");

            avendResponse.Events.Should()
                .HaveCount(1, "because we have a single event in database now")
                .And
                .Contain(eventDTO => eventDTO.Uid == confEventDto.Uid, "because the UID of the returned event should match the one we've added");

            avendResponse.LeadsStatistics.AllTimeGoal.Should()
                .Be(0, "because in empty database no lead records could exist");

            avendResponse.LeadsStatistics.ThisYearExpenses.Amount.Should()
                .Be(bobExpense1.Expense.Amount + bobExpense2.Expense.Amount, "because we added two expenses for bob");

            avendResponse.LeadsStatistics.ThisYearExpenses.Currency.Should()
                .Be(CurrencyCode.USD, "because we added both expenses in USD");
        }

        [TestMethod]
        public async Task ShouldReturnProperThreeSortedEventsWhenComplexSetOfExpensesAddedAndSortingByNameDesc()
        {
            var eventDto1 = await ConferenceEventData.AddFromSample();
            var eventUid1 = eventDto1.Uid;

            var eventDto2 = await ConferenceEventData.AddFromSample();
            var eventUid2 = eventDto2.Uid;

            var eventDto3 = await ConferenceEventData.AddFromSample();
            var eventUid3 = eventDto3.Uid;

            var bobExpenseDataFixtures = new EventExpenseDataFixtures(TestUser.BobTester, System, eventDto1);
            var bobExpense11 = await bobExpenseDataFixtures.AddFromSample();
            var bobExpense12 = await bobExpenseDataFixtures.AddFromSample();
            var bobExpense21 = await bobExpenseDataFixtures.AddFromSample(dto => { dto.EventUid = eventUid2; });
            var bobExpense22 = await bobExpenseDataFixtures.AddFromSample(dto => { dto.EventUid = eventUid2; });

            var aliceExpensesDataFixtures = new EventExpenseDataFixtures(TestUser.AliceTester, System, eventDto1);
            // ReSharper disable once UnusedVariable
            var aliceExpense1 = await aliceExpensesDataFixtures.AddFromSample();
            // ReSharper disable once UnusedVariable
            var aliceExpense2 = await aliceExpensesDataFixtures.AddFromSample(dto => { dto.EventUid = eventUid2; });

            var responseJson = BobTA.GetJsonAsync(DashboardUrl + "?events_sort_field=name&events_sort_order=desc");

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.Events.Should()
                .HaveCount(3, "because we have three events in database now")
                .And
                .Contain(record =>
                    record.Uid == eventUid1.Value
                    && record.TotalExpenses.Amount == bobExpense11.Expense.Amount + bobExpense12.Expense.Amount,
                    "because Bob's total expenses for event N1 are composed from the first couple expenses"
                )
                .And
                .Contain(record =>
                    record.Uid == eventUid2.Value
                    && record.TotalExpenses.Amount == bobExpense21.Expense.Amount + bobExpense22.Expense.Amount,
                    "because Bob's total expenses for event N2 are composed from the second couple expenses"
                )
                .And
                .Contain(record =>
                    record.Uid == eventUid3.Value
                    && record.TotalExpenses.Amount == 0.00M,
                    "because Bob has no expenses for event N3"
                )
                ;

            avendResponse.LeadsStatistics.ThisYearExpenses.Amount.Should()
                .Be(bobExpense11.Expense.Amount + bobExpense12.Expense.Amount +
                    bobExpense21.Expense.Amount + bobExpense22.Expense.Amount,
                    "because we have added 4 expenses for Bob that should sum up properly"
                    );

            avendResponse.LeadsStatistics.ThisYearExpenses.Currency.Should()
                .Be(CurrencyCode.USD, "because we have added Bob's expenses in USD");
        }
    }
}