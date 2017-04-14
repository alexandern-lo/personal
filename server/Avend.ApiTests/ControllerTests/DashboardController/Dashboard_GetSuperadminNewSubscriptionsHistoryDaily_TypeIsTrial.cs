using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Subscriptions;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.DashboardController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("DashboardController")]
    [TestCategory("DashboardController.GetSuperadminNewSubscriptionsHistoryDaily()")]
    // ReSharper disable once InconsistentNaming
    public class Dashboard_GetSuperadminNewSubscriptionsHistoryDaily_TypeIsTrial : BaseDashboardEndpointTest
    {
        public const string DashboardUrl = "dashboard/superadmin/history/new_subscriptions/daily?type=trial";

        [TestMethod]
        public async Task ShouldReturnEmptyListWhenCalledBySuperadminOnDefaultSetup()
        {
            var avendResponse = await AlexSA.GetJsonAsync(DashboardUrl).AvendResponse<List<DateIndexedTupleDto<decimal>>>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.Should()
                .HaveCount(7, "because returned history data by default should contain week-long data");

            for (var index = 0; index < avendResponse.Count; index++)
            {
                var elem = avendResponse[index];

                elem.Value.Should()
                    .Be(0, "because we expect {0} item of subscriptions history to be zero on default setup",
                    new object[] { index }
                );
            }
        }

        [TestMethod]
        public async Task ShouldReturnProperUsersHistoryWhenPreparedForYesterdayAnd2MonthsAgo()
        {
            Guid GeorgeSubscriptionUid;

            var GeorgeTester = new TestUser
            {
                Uid = new Guid("8365d134-5aa6-43af-ae20-2fe5c1f9b658"),
                Token = "George Tester",
                FirstName = "George",
                LastName = "Tester",
                Email = "george@avenddev.onmicrosoft.com"
            };

            //  Alice does have subscription but does NOT have subscription member record 
            AddUserToFakeAD(GeorgeTester);

            using (var scope = System.GetServices())
            {
                GeorgeSubscriptionUid = CreateSubscriptionAndUser(scope, GeorgeTester, "trial", "");
            }

            var subscriptionsData = new BaseEntityData<SubscriptionRecord, SubscriptionDto>(TestUser.AlexTester, System, "users");
            subscriptionsData.UpdateDbRecordCreationTime(BobSubscriptionUid, DateTime.UtcNow.AddDays(-1));
            subscriptionsData.UpdateDbRecordCreationTime(GeorgeSubscriptionUid, DateTime.UtcNow.AddDays(-1));
            subscriptionsData.UpdateDbRecordCreationTime(MarcSubscriptionUid, DateTime.UtcNow.AddDays(-31));

            var avendResponse = await AlexSA.GetJsonAsync(DashboardUrl + "&limit=30").AvendResponse< List<DateIndexedTupleDto<decimal>>>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.Should()
                .HaveCount(30, "because returned history data should contain proper number of items");

            for (var index = 0; index < avendResponse.Count; index++)
            {
                var elem = avendResponse[index];

                if (index != 29)
                    elem.Value.Should()
                        .Be(0, "because we expect {0} item of subscriptions history to be zero",
                            new object[] {index}
                        );
                else
                    elem.Value.Should()
                        .Be(1, "because we expect {0} item of subscriptions history to equal 1",
                            new object[] { index }
                        );
            }
        }
    }
}