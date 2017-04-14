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
    [TestCategory("DashboardController.GetSuperadminAverageEventsHistoryMonthly()")]
    // ReSharper disable once InconsistentNaming
    public class Dashboard_GetSuperadminAvgEventsHistoryMonthly_TypeIsConference : BaseDashboardEndpointTest
    {
        public const string DashboardUrl = "dashboard/superadmin/history/average_events/monthly?type=conference";

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

            var lastMonthEventDto = await ConferenceEventData.AddFromSample();
            ConferenceEventData.UpdateDbRecordCreationTime(lastMonthEventDto.Uid.GetValueOrDefault(), DateTime.UtcNow.AddMonths(-1));
            var earlierEventDto = await ConferenceEventData.AddFromSample();
            ConferenceEventData.UpdateDbRecordCreationTime(earlierEventDto.Uid.GetValueOrDefault(), DateTime.UtcNow.AddMonths(-2));

            var avendResponse = await AlexSA.GetJsonAsync(DashboardUrl + "&limit=11").AvendResponse<List<DateIndexedTupleDto<decimal>>>();

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
   }
}