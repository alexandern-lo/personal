using System;
using System.Net;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model;
using Avend.API.Services.Dashboard.NetworkDTO;
using Avend.API.Services.Subscriptions;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.DashboardController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("DashboardController")]
    [TestCategory("DashboardController.GetSuperadminDashboardSummary()")]
    // ReSharper disable once InconsistentNaming
    public class Dashboard_GetSuperadminSummary : BaseDashboardEndpointTest
    {
        public const string DashboardUrl = "dashboard/superadmin/summary";

        [TestMethod]
        public async Task ShouldReturnProperUsersAndSubscriptionsDataInSummaryWhenCalledBySuperadminOnDefaultSetup()
        {
            var avendResponse = await AlexSA.GetJsonAsync(DashboardUrl).AvendResponse<SuperadminDashboardDto>();

            var responseReceivedTime = DateTime.UtcNow;

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(responseReceivedTime.AddSeconds(-5), "because returned data should be recent");

            avendResponse.SubscriptionStats.AllTime.Should()
                .Match<PaidVsTrialStatsDto>(stats => stats.Trial == 0,
                    "because in default setup we have no data before today")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Paid == 0,
                    "because in default setup we have no data before today")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Total == 0,
                    "because in default setup we have no data before today")
                ;

            avendResponse.SubscriptionStats.LastPeriod.Should()
                .Match<PaidVsTrialStatsDto>(stats => stats.Trial == 0,
                    "because in default setup we have no data before today")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Paid == 0,
                    "because in default setup we have no data before today")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Total == 0,
                    "because in default setup we have no data before today")
                ;

            avendResponse.UsersStats.AllTime.Should()
                .Match<PaidVsTrialStatsDto>(stats => stats.Trial == 0,
                    "because in default setup we have no data before today")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Paid == 0,
                    "because in default setup we have no data before today")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Total == 0,
                    "because in default setup we have no data before today")
                ;

            avendResponse.UsersStats.Yesterday.Should()
                .Match<PaidVsTrialStatsDto>(stats => stats.Trial == 0,
                    "because in default setup we have no data before today")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Paid == 0,
                    "because in default setup we have no data before today")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Total == 0,
                    "because in default setup we have no data before today")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnProperUsersAndSubscriptionsDataInSummaryWhenPreparedForYesterday()
        {
            var subscriptionsData = new BaseEntityData<SubscriptionRecord, SubscriptionDto>(TestUser.AlexTester, System, "users");
            subscriptionsData.UpdateDbRecordCreationTime(BobSubscriptionUid, DateTime.UtcNow.AddDays(-1));

            var usersData = new BaseEntityData<Avend.API.Model.SubscriptionMember, SubscriptionMemberDto>(TestUser.AlexTester, System, "users");
            usersData.UpdateDbRecordCreationTime(TestUser.MarcTester.Uid, DateTime.UtcNow.AddDays(-1));

            var avendResponse = await AlexSA.GetJsonAsync(DashboardUrl).AvendResponse<SuperadminDashboardDto>();

            var responseReceivedTime = DateTime.UtcNow;

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(responseReceivedTime.AddSeconds(-5), "because returned data should be recent");

            avendResponse.SubscriptionStats.AllTime.Should()
                .Match<PaidVsTrialStatsDto>(stats => stats.Total == 1,
                    "because we setup a single subscription created within valid dates range")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Trial == 0,
                    "because we have no trial subscriptions created within valid dates range")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Paid == 1,
                    "because this single subscription created within dates range is paid")
                ;

            avendResponse.SubscriptionStats.LastPeriod.Should()
                .Match<PaidVsTrialStatsDto>(stats => stats.Total == 1,
                    "because we setup a single subscription created within last period")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Trial == 0,
                    "because we have no trial subscriptions created within last period")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Paid == 1,
                    "because this single subscription created within dates range is paid")
                ;

            avendResponse.UsersStats.AllTime.Should()
                .Match<PaidVsTrialStatsDto>(stats => stats.Total == 1,
                    "because only Marc was created before today")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Trial == 0,
                    "because we have no trial users created within valid dates range")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Paid == 1,
                    "because only Marc was created before today and he belongs to a paid subscription")
                ;

            avendResponse.UsersStats.Yesterday.Should()
                .Match<PaidVsTrialStatsDto>(stats => stats.Total == 1,
                    "because only Marc was created before today")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Trial == 0,
                    "because we have no trial users created within valid dates range")
                .And
                .Match<PaidVsTrialStatsDto>(stats => stats.Paid == 1,
                    "because only Marc was created before today and he belongs to a paid subscription")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnForbiddenWhenCalledByTenantAdmin()
        {
            var responseJson = BobTA.GetJsonAsync(DashboardUrl);

            var response = await responseJson.Response(HttpStatusCode.Forbidden, "because only superadmins should have access to superadmin dashboard data");

            var responseContent = await response.Content.ReadAsStringAsync();

            responseContent.Should()
                .BeNullOrEmpty("because the access denial should happen on controller level");
        }

        [TestMethod]
        public async Task ShouldReturnForbiddenWhenCalledBySeatUser()
        {
            var responseJson = CecileSU.GetJsonAsync(DashboardUrl);

            var response = await responseJson.Response(HttpStatusCode.Forbidden, "because only superadmins should have access to superadmin dashboard data");

            var responseContent = await response.Content.ReadAsStringAsync();

            responseContent.Should()
                .BeNullOrEmpty("because the access denial should happen on controller level");
        }

        [TestMethod]
        public async Task ShouldCountLeadsStatsProperly()
        {
            var conferenceEventDto = await ConferenceEventData.AddFromSample();

            var bobLeadsData = LeadData.Init(TestUser.BobTester, conferenceEventDto.Uid.GetValueOrDefault(), System);

            // This lead is to recent and should not get into the stats
            // ReSharper disable once UnusedVariable
            var leadDto0 = await bobLeadsData.Add();

            var leadDto1 = await bobLeadsData.Add();
            bobLeadsData.UpdateDbRecordCreationTime(leadDto1, DateTime.UtcNow.AddDays(-1));
            var leadDto2 = await bobLeadsData.Add();
            bobLeadsData.UpdateDbRecordCreationTime(leadDto2, DateTime.UtcNow.AddDays(-40));
            var avendResponse = await AlexSA.GetJsonAsync(DashboardUrl).AvendResponse<SuperadminDashboardDto>();

            avendResponse.LeadsStats.AllTime.Total.Should()
                .Be(2);
        }

        [TestMethod]
        public async Task ShouldCountEventsStatsProperly()
        {
            var conferenceEventDto1 = await ConferenceEventData.AddFromSample();
            ConferenceEventData.UpdateDbRecordCreationTime(conferenceEventDto1.Uid, DateTime.UtcNow.AddDays(-1));

            //  The event below should not get into the events stats
            //  ReSharper disable once UnusedVariable
            var conferenceEventDto2 = await ConferenceEventData.AddFromSample();

            var bobEventsData = new EventData(TestUser.BobTester, System);
            var eventDto1 = await bobEventsData.AddFromSample();
            bobEventsData.UpdateDbRecordCreationTime(eventDto1.Uid, DateTime.UtcNow.AddDays(-10));

            var cecileEventsData = new EventData(TestUser.CecileTester, System);
            //  The event below should not get into the events stats
            //  ReSharper disable once UnusedVariable
            var eventDto2 = await cecileEventsData.AddFromSample();

            var avendResponse = await AlexSA.GetJsonAsync(DashboardUrl).AvendResponse<SuperadminDashboardDto>();

            avendResponse.EventsStats.AllTime.Total.Should()
                .Be(2, "because we have added a single conference and single Bob's event before today");

            avendResponse.EventsStats.AllTime.Conference.Should()
                .Be(1, "because we have added a single conference event before today");
        }
    }
}