using System;
using System.Net;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Infrastructure.Responses;
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
    public class Dashboard_GetDashboard_GeneralStats : BaseDashboardEndpointTest
    {
        public const string DashboardUrl = "dashboard";
        public readonly Uri DashboardUri = new Uri(DashboardUrl, UriKind.Relative);

        [TestMethod]
        public async Task ShouldReturnZeroStatsAndEmptyListsWhenDatabaseIsEmpty()
        {
            var responseJson = BobTA.GetJsonAsync(DashboardUrl);

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.LeadsStatistics.Should()
                .Match<LeadsStatisticsDTO>(stats => stats.ThisYearExpenses != null,
                    "because even in empty database this year expenses object should be valid")
                .And
                .Match<LeadsStatisticsDTO>(stats => stats.ThisYearExpenses.Currency == CurrencyCode.USD,
                    "because even in empty database this year expenses should be in USD")
                .And
                .Match<LeadsStatisticsDTO>(stats => stats.ThisYearExpenses.Amount == 0.00M,
                    "because even in empty database this year expenses amount should be zero")
                ;

            avendResponse.LeadsStatistics.Should()
                .Match<LeadsStatisticsDTO>(stats => stats.ThisYearCostPerLead != null,
                    "because even in empty database this year CPL object should be valid")
                .And
                .Match<LeadsStatisticsDTO>(stats => stats.ThisYearCostPerLead.Currency == CurrencyCode.USD,
                    "because even in empty database this year CPL should be in USD")
                .And
                .Match<LeadsStatisticsDTO>(stats => stats.ThisYearCostPerLead.Amount == 0.00M,
                    "because even in empty database this year CPL amount should be zero")
                ;

            avendResponse.LeadsStatistics.Should()
                .Match<LeadsStatisticsDTO>(stats => stats.AllTimeCount == 0, "because in empty database no lead records could exist")
                .And
                .Match<LeadsStatisticsDTO>(stats => stats.LastPeriodCount == 0, "because in empty database no lead records could exist")
                ;

            avendResponse.LeadsStatistics.Should()
                .Match<LeadsStatisticsDTO>(stats => stats.AllTimeGoal == 0, "because in empty database no lead goal records could exist")
                .And
                .Match<LeadsStatisticsDTO>(stats => stats.LastPeriodGoal == 0, "because in empty database no lead goal records could exist")
                ;

            avendResponse.Resources.Should()
                .HaveCount(0, "because in empty database no resources could exist");

            avendResponse.Events.Should()
                .HaveCount(0, "because in empty database no event records could exist");
        }

        [TestMethod]
        public async Task ShouldReturnValidLeadCountStatsWhenHaveSingleRecentLead()
        {
            var eventData = await EventData.InitWithSampleEvent(TestUser.BobTester, System);
            var bobLeadData = LeadData.Init(TestUser.BobTester, eventData.Event.Uid.Value, System);

            var leadDto = await bobLeadData.Add();

            bobLeadData.UpdateDbRecordCreationTime(leadDto, DateTime.UtcNow.AddDays(-1));

            var responseJson = BobTA.GetJsonAsync(DashboardUrl);

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.LeadsStatistics.Should()
                .Match<LeadsStatisticsDTO>(stats => stats.AllTimeCount == 1,
                    "because we have added a single lead for yesterday")
                .And
                .Match<LeadsStatisticsDTO>(stats => stats.LastPeriodCount == 1,
                    "because we have added a single lead for yesterday")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnValidLeadCountStatsWhenHaveSingleLeadTwoMonthsOld()
        {
            var eventData = await EventData.InitWithSampleEvent(TestUser.BobTester, System);
            var bobLeadData = LeadData.Init(TestUser.BobTester, eventData.Event.Uid.Value, System);

            var leadDto = await bobLeadData.Add();

            bobLeadData.UpdateDbRecordCreationTime(leadDto, DateTime.UtcNow.AddMonths(-2));

            var responseJson = BobTA.GetJsonAsync(DashboardUrl);

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.LeadsStatistics.Should()
                .Match<LeadsStatisticsDTO>(stats => stats.AllTimeCount == 1,
                    "because we have added a single lead for yesterday")
                .And
                .Match<LeadsStatisticsDTO>(stats => stats.LastPeriodCount == 0,
                    "because we have added a single lead for yesterday")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnValidCplStatsWhenHaveTwoLeadsThisYearAndOneLeadEarlier()
        {
            var eventData = await EventData.InitWithSampleEvent(TestUser.BobTester, System);

            var expenseData = await EventExpenseDataFixtures.InitWithSampleExpense(TestUser.BobTester, System, eventData.Event);

            var expense = expenseData.EventExpenses[0].Expense;

            var bobLeadData = LeadData.Init(TestUser.BobTester, eventData.Event.Uid.Value, System);

            var oldLeadDto = await bobLeadData.Add();

            var leadDto1 = await bobLeadData.Add();
            var leadDto2 = await bobLeadData.Add();

            bobLeadData.UpdateDbRecordCreationTime(oldLeadDto, DateTime.UtcNow.AddYears(-1));

            var responseJson = BobTA.GetJsonAsync(DashboardUrl);

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.LeadsStatistics.Should()
                .Match<LeadsStatisticsDTO>(stats => stats.ThisYearExpenses != null,
                    "because this year expenses data should always be in reply")
                .And
                .Match<LeadsStatisticsDTO>(stats => stats.ThisYearCostPerLead != null,
                    "because this year CPL data should always be in reply")
                .And
                .Match<LeadsStatisticsDTO>(stats => stats.ThisYearExpenses.Amount == expense.Amount,
                    "because we have added a single expense this year")
                .And
                .Match<LeadsStatisticsDTO>(stats => stats.ThisYearCostPerLead.Amount == expense.Amount / 2.0M,
                    "because we have added two leads for this year so CPL should be half of the expenses value")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnForbiddenWhenSuperAdminSendsRequest()
        {
            var responseJson = AlexSA.GetJsonAsync(DashboardUrl);

            var avendResponse = await responseJson.AvendErrorResponse(HttpStatusCode.Forbidden);

            avendResponse.Should()
                .NotBeNull("because we should get array of errors in response")
                .And
                .HaveCount(1, "because we should get only a single forbidden error in errors array");

            avendResponse[0].Should()
                .Match<Error>(record => record.Code == ErrorCodes.Forbidden, "because returned code should be forbidden")
                .And
                .Match<Error>(record => record.Message == "User's dashboard is not available for superadmins", "because returned message should explain what's wrong")
                ;
        }
    }
}