using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Infrastructure.Responses;
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
    [TestCategory("DashboardController.GetTenantStatsForLeadsGoalsByUser()")]
    // ReSharper disable once InconsistentNaming
    public class Dashboard_GetTenantStatsForLeadsGoalsByUser : BaseDashboardEndpointTest
    {
        private HttpClient cecilBrowser;

        public const string ListUserLeadsGoalsUrl = "dashboard/users/leads_goals";
        public readonly Uri ListUserLeadsGoalsUri = new Uri(ListUserLeadsGoalsUrl, UriKind.Relative);

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
        [TestCategory("DashboardController.GetTenantStatsForLeadsGoalsByUser()")]
        public async Task ShouldReturnEmptyDataForAllExistingUsersWhenNoGoalsAdded()
        {
            var responseJson = BobTA.PostJsonAsync(ListUserLeadsGoalsUrl, new FilterByEventsRequestDTO());

            var avendResponse = await responseJson.AvendResponse<List<UserTotalLeadsGoalDto>>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid list of goals")
                .And
                .HaveCount(2, "because even for empty database we should return empty results for the existing users")
                .And
                .Contain(record => record.UserUid == TestUser.BobTester.Uid && record.LeadsGoal == 0,
                    "because Bob should be in the list")
                .And
                .Contain(record => record.UserUid == TestUser.CecileTester.Uid && record.LeadsGoal == 0,
                    "because Cecile should be in the list")
                ;
        }

        [TestMethod]
        [TestCategory("Integrational")]
        [TestCategory("DashboardController")]
        [TestCategory("DashboardController.GetTenantStatsForLeadsGoalsByUser()")]
        public async Task ShouldReturnProperDataWhenSingleGoalsRecordIsAdded()
        {
            var eventData = await EventData.InitWithSampleEvent(TestUser.BobTester, System);

            var eventUid = eventData.Event.Uid;

            // ReSharper disable once UnusedVariable
            LeadDto lead = await LeadData.Init(TestUser.BobTester, eventUid.Value, System).Add();

            var eventUserGoalDto2 = new EventUserGoalsDto()
            {
                EventUid = eventUid,
                LeadsGoal = 12,
            };

            await cecilBrowser.PostJsonAsync($"events/{eventUid}/goals", eventUserGoalDto2).AvendResponse<Guid>();

            var responseJson = BobTA.PostJsonAsync(ListUserLeadsGoalsUrl,
                new FilterByEventsRequestDTO() {EventUids = null});

            var avendResponse = await responseJson.AvendResponse<List<UserTotalLeadsGoalDto>>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid list of expenses")
                .And
                .HaveCount(2, "because we have two users in this tenant")
                .And
                .Contain(
                    record => record.UserUid == TestUser.BobTester.Uid
                              && record.LeadsGoal == 0
                              && record.LeadsCount == 1,
                    "because we have added leads goal for Bob equal to 11 but just a single leaad")
                .And
                .Contain(
                    record => record.UserUid == TestUser.CecileTester.Uid
                              && record.LeadsGoal == 12
                              && record.LeadsCount == 0,
                    "because we have added leads goal for Cecile equal to 12 and no real leads")
                .And
                .BeInDescendingOrder(record => record.LeadsCount,
                    "because the records should be sorted in descending leads count order")
                ;
        }

        [TestMethod]
        [TestCategory("Integrational")]
        [TestCategory("DashboardController")]
        [TestCategory("DashboardController.GetTenantStatsForLeadsGoalsByUser()")]
        public async Task ShouldProperlySortResultsListByLeadsCount()
        {
            var eventData = await EventData.InitWithSampleEvent(TestUser.BobTester, System);

            var eventUid = eventData.Event.Uid;

            var eventUserGoalDto1 = new EventUserGoalsDto()
            {
                EventUid = eventUid,
                LeadsGoal = 11,
            };

            await BobTA.PostJsonAsync($"events/{eventUid}/goals", eventUserGoalDto1).AvendResponse<Guid>();

            // ReSharper disable once UnusedVariable
            LeadDto lead = await LeadData.Init(TestUser.BobTester, eventUid.Value, System).Add();

            await BobTA.PostJsonAsync($"events/{eventUid}/goals", eventUserGoalDto1).AvendResponse<Guid>();

            var eventUserGoalDto2 = new EventUserGoalsDto()
            {
                EventUid = eventUid,
                LeadsGoal = 12,
            };

            await cecilBrowser.PostJsonAsync($"events/{eventUid}/goals", eventUserGoalDto2).AvendResponse<Guid>();

            var responseJson = BobTA.PostJsonAsync(ListUserLeadsGoalsUrl,
                new FilterByEventsRequestDTO() {EventUids = null});

            var avendResponse = await responseJson.AvendResponse<List<UserTotalLeadsGoalDto>>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid list of expenses")
                .And
                .HaveCount(2, "because we have two users in this tenant")
                .And
                .Contain(
                    record => record.UserUid == TestUser.BobTester.Uid
                              && record.LeadsGoal == 11
                              && record.LeadsCount == 1,
                    "because we have added leads goal for Bob equal to 11 but just a single lead")
                .And
                .Contain(
                    record => record.UserUid == TestUser.CecileTester.Uid
                              && record.LeadsGoal == 12
                              && record.LeadsCount == 0,
                    "because we have added leads goal for Cecile equal to 12 and no real leads")
                .And
                .BeInDescendingOrder(record => record.LeadsCount,
                    "because the records should be sorted in descending leads count order")
                ;
        }

        [TestMethod]
        [TestCategory("Integrational")]
        [TestCategory("DashboardController")]
        [TestCategory("DashboardController.GetTenantStatsForLeadsGoalsByUser()")]
        public async Task ShouldProperlyLimitListWhenAllUsersHaveLeadGoals()
        {
            var eventData = await EventData.InitWithSampleEvent(TestUser.BobTester, System);

            var eventUid = eventData.Event.Uid;

            var eventUserGoalDto1 = new EventUserGoalsDto()
            {
                EventUid = eventUid,
                LeadsGoal = 11,
            };

            await BobTA.PostJsonAsync($"events/{eventUid}/goals", eventUserGoalDto1).AvendResponse<Guid>();

            // ReSharper disable once UnusedVariable
            LeadDto lead = await LeadData.Init(TestUser.BobTester, eventUid.Value, System).Add();

            var eventUserGoalDto2 = new EventUserGoalsDto()
            {
                EventUid = eventUid,
                LeadsGoal = 12,
            };

            await cecilBrowser.PostJsonAsync($"events/{eventUid}/goals", eventUserGoalDto2).AvendResponse<Guid>();

            var responseJson = BobTA.PostJsonAsync(ListUserLeadsGoalsUrl,
                new FilterByEventsRequestDTO() {EventUids = null, Limit = 1});

            var avendResponse = await responseJson.AvendResponse<List<UserTotalLeadsGoalDto>>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid list of expenses")
                .And
                .HaveCount(1, "because we have limit for 1 user in request")
                .And
                .Contain(
                    record => record.UserUid == TestUser.BobTester.Uid
                              && record.LeadsGoal == 11
                              && record.LeadsCount == 1,
                    "because we have a single lead for Bob so he's displayed first")
                .And
                .BeInDescendingOrder(record => record.LeadsCount,
                    "because the records should always be sorted in descending leads count order")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnForbiddenIfCalledBySeatUser()
        {
            var responseBody = await cecilBrowser.PostJsonAsync(ListUserLeadsGoalsUrl,
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

            var errors = await alexBrowser.PostJsonAsync(ListUserLeadsGoalsUrl,
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