using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Infrastructure.Responses;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Dashboard.NetworkDTO;
using Avend.API.Services.Events.NetworkDTO;
using Avend.API.Services.Leads.NetworkDTO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.DashboardController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("DashboardController")]
    [TestCategory("DashboardController.GetTenantStatsForExpensesByUser()")]
    // ReSharper disable once InconsistentNaming
    public class Dashboard_GetTenantStatsForExpensesByUser : BaseDashboardEndpointTest
    {
        private HttpClient cecilBrowser;

        public const string ListUserExpensesUrl = "dashboard/users/expenses";
        public readonly Uri ListUserExpensesUri = new Uri(ListUserExpensesUrl, UriKind.Relative);

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();

            cecilBrowser = System.CreateClient(TestUser.CecileTester.Token);
        }

        public override void Dispose()
        {
            cecilBrowser?.Dispose();

            base.Dispose();
        }

        [TestMethod]
        [TestCategory("Integrational")]
        [TestCategory("DashboardController")]
        [TestCategory("DashboardController.GetTenantStatsForExpensesByUser()")]
        public async Task ShouldGenerateEmptyExpensesDataWhenNoExpensesAdded()
        {
            List<Guid?> eventUids = null;

            var responseJson = BobTA.PostJsonAsync(ListUserExpensesUrl,
                new FilterByEventsRequestDTO() {EventUids = eventUids});

            var avendResponse = await responseJson.AvendResponse<UserTotalExpensesListDto>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid list of expenses");

            avendResponse.TotalExpenses.Should()
                .NotBeNull("because the total amount should always be populated")
                .And
                .Match(rec => (rec as MoneyDto).Amount == 0.00M, "because the total amount in empty database is zero")
                .And
                .Match(rec => (rec as MoneyDto).Currency == CurrencyCode.USD, "because the default currency is USD")
                ;

            avendResponse.UserExpenses.Should()
                .NotBeNull("because response should contain a valid list of expenses")
                .And
                .HaveCount(2, "because even for empty database we should return empty results for the active users")
                .And
                .Contain(record => record.UserUid == TestUser.BobTester.Uid
                                   && record.FirstName == TestUser.BobTester.FirstName
                                   && record.LastName == TestUser.BobTester.LastName
                                   && record.Amount.Amount == 0
                                   && record.Amount.Currency == CurrencyCode.USD,
                    "because Bob should be in the list")
                .And
                .Contain(record => record.UserUid == TestUser.CecileTester.Uid
                                   && record.FirstName == TestUser.CecileTester.FirstName
                                   && record.LastName == TestUser.CecileTester.LastName
                                   && record.Amount.Amount == 0
                                   && record.Amount.Currency == CurrencyCode.USD,
                    "because Cecile should be in the list")
                ;
        }

        [TestMethod]
        [TestCategory("Integrational")]
        [TestCategory("DashboardController")]
        [TestCategory("DashboardController.GetTenantStatsForExpensesByUser()")]
        public async Task ShouldProperlyReturnExistingExpensesData()
        {
            List<Guid?> eventUids = null;

            var eventData = await EventData.InitWithSampleEvent(TestUser.BobTester, System);

            var eventUid = eventData.Event.Uid;

            var bobExpenseDataFixtures = await EventExpenseDataFixtures.InitWithSampleExpense(TestUser.BobTester, System, eventData.Event);

            var bobExpense1 = bobExpenseDataFixtures.EventExpenses[0];

            var bobExpense2 = await bobExpenseDataFixtures.AddFromSample();

            // ReSharper disable once UnusedVariable
            LeadDto leadBob = await LeadData.Init(TestUser.BobTester, eventUid.Value, System).Add();

            // LeadDTO leadCecile1 = await LeadData.Init(TestUser.CecileTester, eventUid.Value, System).Add();
            // LeadDTO leadCecile2 = await LeadData.Init(TestUser.CecileTester, eventUid.Value, System).Add();

            var cecilExpenseDataFixtures = await EventExpenseDataFixtures.InitWithSampleExpense(TestUser.CecileTester, System, eventData.Event);

            var cecilExpense1 = cecilExpenseDataFixtures.EventExpenses[0];

            var responseJson = BobTA.PostJsonAsync(ListUserExpensesUrl,
                new FilterByEventsRequestDTO() {EventUids = eventUids});

            var avendResponse = await responseJson.AvendResponse<UserTotalExpensesListDto>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid list of expenses");

            var expectedTotalAmount = bobExpense1.Expense.Amount + bobExpense2.Expense.Amount + cecilExpense1.Expense.Amount;

            avendResponse.TotalExpenses.Should()
                .NotBeNull("because the total amount should always be populated")
                .And
                .Match<MoneyDto>(rec => rec.Amount == expectedTotalAmount, "because the total amount in database is the sum for two records from Bob and single one from Cecile")
                .And
                .Match<MoneyDto>(rec => rec.Currency == CurrencyCode.USD, "because the used currency is USD")
                ;

            /*
                        avendResponse.EventUids.Should()
                            .Equal(eventUids, "because we should get back the same events list we have passed over");
            */

            var expectedBobTotalAmount = bobExpense1.Expense.Amount + bobExpense2.Expense.Amount;

            avendResponse.UserExpenses.Should()
                .NotBeNull("because response should contain a valid list of expenses")
                .And
                .HaveCount(2, "because we have two active users in this tenant")
                .And
                .Contain(
                    record => record.UserUid == TestUser.BobTester.Uid
                              && record.FirstName == TestUser.BobTester.FirstName
                              && record.LastName == TestUser.BobTester.LastName
                              && record.Amount.Amount == expectedBobTotalAmount,
                    "because we have added two expenses for Bob and expect to get the sum of those"
                )
                .And
                .Contain(
                    record => record.UserUid == TestUser.CecileTester.Uid
                              && record.FirstName == TestUser.CecileTester.FirstName
                              && record.LastName == TestUser.CecileTester.LastName
                              && record.Amount.Amount == cecilExpense1.Expense.Amount,
                    "because we have added single expense for Cecile and want to get the exact amount"
                );

            using (var enumerator = avendResponse.UserExpenses.GetEnumerator())
            {
                enumerator.MoveNext();

                enumerator.Current.UserUid.Should()
                    .Be(TestUser.BobTester.Uid,
                        "because we have added a single lead for Bob so he should be first in list"
                    );

                enumerator.MoveNext();

                enumerator.Current.UserUid.Should()
                    .Be(TestUser.CecileTester.Uid,
                        "because we have not added event a single lead for Cecile so she should be the last in list"
                    );
            }
        }

        [TestMethod]
        [TestCategory("Integrational")]
        [TestCategory("DashboardController")]
        [TestCategory("DashboardController.GetTenantStatsForExpensesByUser()")]
        public async Task ShouldProperlyLimitDashboardEventsList()
        {
            var eventData = await EventData.InitWithSampleEvent(TestUser.AlexTester, System);

            var eventUid = eventData.Event.Uid;

            var eventUserExpenseDto11 = new EventUserExpenseDto()
            {
                EventUid = eventUid,
                Comments = "Bobs expense 1",
                Expense = new MoneyDto()
                {
                    Amount = 11,
                    Currency = CurrencyCode.USD,
                },
            };

            var eventUserExpenseDto12 = new EventUserExpenseDto()
            {
                EventUid = eventUid,
                Comments = "Bobs expense 2",
                Expense = new MoneyDto()
                {
                    Amount = 12,
                    Currency = CurrencyCode.USD,
                },
            };

            await BobTA.PostJsonAsync($"events/{eventUid}/expenses", eventUserExpenseDto11).AvendResponse<EventUserExpenseDto>();

            await BobTA.PostJsonAsync($"events/{eventUid}/expenses", eventUserExpenseDto12).AvendResponse<EventUserExpenseDto>();

            // ReSharper disable once UnusedVariable
            LeadDto leadBob = await LeadData.Init(TestUser.BobTester, eventUid.Value, System).Add();

            // ReSharper disable once UnusedVariable
            LeadDto leadCecile1 = await LeadData.Init(TestUser.CecileTester, eventUid.Value, System).Add();
            // ReSharper disable once UnusedVariable
            LeadDto leadCecile2 = await LeadData.Init(TestUser.CecileTester, eventUid.Value, System).Add();

            var eventUserExpenseDto21 = new EventUserExpenseDto()
            {
                EventUid = eventUid,
                Expense = new MoneyDto()
                {
                    Amount = 21,
                    Currency = CurrencyCode.EUR,
                },
            };

            await cecilBrowser.PostJsonAsync($"events/{eventUid}/expenses", eventUserExpenseDto21).AvendResponse<EventUserExpenseDto>();

            var responseJson = BobTA.PostJsonAsync(ListUserExpensesUrl,
                new FilterByEventsRequestDTO() {EventUids = null, Limit = 1});

            var avendResponse = await responseJson.AvendResponse<UserTotalExpensesListDto>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid list of expenses");

            avendResponse.TotalExpenses.Should()
                .NotBeNull("because the total amount should always be populated")
                .And
                .Match<MoneyDto>(rec => rec.Amount == 44.00M, "because the total amount in database is 11+12+21=44")
                .And
                .Match<MoneyDto>(rec => rec.Currency == CurrencyCode.USD, "because the used currency is USD")
                ;

            avendResponse.UserExpenses.Should()
                .NotBeNull("because response should contain a valid list of expenses")
                .And
                .HaveCount(1, "because we have a limit for 1 user in this tenant")
                .And
                .Contain(
                    record => record.UserUid == TestUser.CecileTester.Uid
                              && record.FirstName == TestUser.CecileTester.FirstName
                              && record.LastName == TestUser.CecileTester.LastName
                              && record.Amount.Amount == 21,
                    "because we have added 2 leads for Cecile and thus she's the first in the list ordered by leads count")
                ;
        }

        [TestMethod]
        [TestCategory("Integrational")]
        [TestCategory("DashboardController")]
        [TestCategory("DashboardController.GetTenantStatsForExpensesByUser()")]
        public async Task ExpensesPopulatedWithEventUidsListEqualToNullAndLimitedListTest()
        {
            var responseJson = BobTA.PostJsonAsync(ListUserExpensesUrl,
                new FilterByEventsRequestDTO()
                {
                    EventUids = new List<Guid?>()
                    {
                        null,
                    },
                    Limit = 1
                });

            var avendResponse = await responseJson.AvendResponse<UserTotalExpensesListDto>();

            avendResponse.UserExpenses.Should()
                .NotBeNull("because response should contain a valid list of expenses")
                .And
                .HaveCount(1, "because we have a limit for 1 user in this tenant")
                .And
                .Contain(
                    record => record.UserUid == TestUser.BobTester.Uid
                              && record.FirstName == TestUser.BobTester.FirstName
                              && record.LastName == TestUser.BobTester.LastName
                              && record.Amount.Currency == CurrencyCode.USD
                              && record.Amount.Amount == 0,
                    "because we have added no leads and no expenses and Bob's name goes before Cecile's")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnForbiddenIfCalledBySeatUser()
        {
            var responseBody = await cecilBrowser.PostJsonAsync(ListUserExpensesUrl,
                new FilterByEventsRequestDTO()
                {
                    EventUids = new List<Guid?>()
                    {
                        null,
                    },
                    Limit = 1
                }).Response<string>(HttpStatusCode.Forbidden, "because seat user should not be able to even get into controller");

            responseBody.Should()
                .BeNullOrEmpty("because seat user should not be able to even get into controller");
        }

        [TestMethod]
        public async Task ShouldReturnForbiddenIfCalledBySuperAdmin()
        {
            var alexBrowser = System.CreateClient(TestUser.AlexTester.Token);

            var errors = await alexBrowser.PostJsonAsync(ListUserExpensesUrl,
                new FilterByEventsRequestDTO()
                {
                    EventUids = new List<Guid?>()
                    {
                        null,
                    },
                    Limit = 1
                }).AvendErrorResponse(HttpStatusCode.Forbidden, "because super admin should not be able to get this data");

            errors.Should()
                .HaveCount(1, "because we should get exactly single error");

            errors[0].Should()
                .Match<Error>(error => error.Code == "forbidden")
                .And
                .Match<Error>(error => error.Message.Contains("Only tenant admin"))
                ;
        }
    }
}