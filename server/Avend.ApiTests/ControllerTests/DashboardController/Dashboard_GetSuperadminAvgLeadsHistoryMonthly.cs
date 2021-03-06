using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Subscriptions;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.DashboardController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("DashboardController")]
    [TestCategory("DashboardController.GetSuperadminAverageLeadsHistoryMonthly()")]
    // ReSharper disable once InconsistentNaming
    public class Dashboard_GetSuperadminAvgLeadsHistoryMonthly: BaseDashboardEndpointTest
    {
        public const string DashboardUrl = "dashboard/superadmin/history/average_leads/monthly";

        public LeadData LeadData { get; private set; }

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();

            if (ConferenceEventData.Event == null)
                await ConferenceEventData.AddFromSample();

            LeadData = LeadData.Init(TestUser.BobTester, ConferenceEventData.Event.Uid.GetValueOrDefault(), System);
        }

        [TestMethod]
        public async Task ShouldReturnEmptyListWhenCalledBySuperadminOnDefaultSetup()
        {
            var avendResponse = await AlexSA.GetJsonAsync(DashboardUrl).AvendResponse<List<DateIndexedTupleDto<decimal>>>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.Should()
                .HaveCount(3, "because returned history data by default should contain week-long data");

            for (var index = 0; index < avendResponse.Count; index++)
            {
                var elem = avendResponse[index];

                elem.Value.Should()
                    .Be(0, "because we expect {0} item of user history to be zero on default setup",
                    new object[] { index }
                );
            }
        }

        [TestMethod]
        public async Task ShouldReturnProperLeadsHistoryWhenPreparedForLastMonthAnd2MonthsAgo()
        {
            var usersData = new BaseEntityData<Avend.API.Model.SubscriptionMember, SubscriptionMemberDto>(TestUser.AlexTester, System, "users");
            usersData.UpdateDbRecordCreationTime(TestUser.MarcTester.Uid, DateTime.UtcNow.AddMonths(-1));
            usersData.UpdateDbRecordCreationTime(TestUser.BobTester.Uid, DateTime.UtcNow.AddMonths(-2));

            var leadDto1 = await LeadData.Add();
            LeadData.UpdateDbRecordCreationTime(leadDto1, DateTime.UtcNow.AddMonths(-1));
            var leadDto2 = await LeadData.Add();
            LeadData.UpdateDbRecordCreationTime(leadDto2, DateTime.UtcNow.AddMonths(-2));

            var avendResponse = await AlexSA.GetJsonAsync(DashboardUrl + "?limit=11").AvendResponse<List<DateIndexedTupleDto<decimal>>>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.Should()
                .HaveCount(11, "because returned history data should contain proper number of items");

            for (var index = 0; index < avendResponse.Count; index++)
            {
                var elem = avendResponse[index];

                if (index < 9)
                    elem.Value.Should()
                        .Be(0, "because we expect {0} item of user history to be zero",
                            new object[] { index }
                        );
                else if (index == 9)
                    elem.Value.Should()
                        .Be(1M, "because we expect {0} item of user history to equal 1M",
                            new object[] { index }
                        );
                else if (index == 10)
                    elem.Value.Should()
                        .Be(0.5M, "because we expect {0} item of user history to equal 0.5M",
                            new object[] { index }
                        );
            }
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
    }
}