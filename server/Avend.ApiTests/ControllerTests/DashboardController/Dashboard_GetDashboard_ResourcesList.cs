using System;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
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
    public class Dashboard_GetDashboard_ResourcesList: BaseDashboardEndpointTest
    {
        public const string DashboardUrl = "dashboard";
        public readonly Uri DashboardUri = new Uri(DashboardUrl, UriKind.Relative);

        [TestMethod]
        public async Task ShouldReturnSingleResourceRecordWhenItIsAddedToDatabase()
        {
            var resourceData11 = await ResourceData.InitWithSample(TestUser.BobTester, System);
            var resourceUid11 = resourceData11.Resource.Uid;

            var resourceData12 = await ResourceData.InitWithSample(TestUser.BobTester, System);
            var resourceUid12 = resourceData12.Resource.Uid;

            var resourceData21 = await ResourceData.InitWithSample(TestUser.AliceTester, System);
            var resourceUid22 = resourceData21.Resource.Uid;

            var responseJson = BobTA.GetJsonAsync(DashboardUrl);

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.Resources.Should()
                .HaveCount(2, "because we have added just 2 resources for Bob")
                .And
                .Contain(resource => resource.Uid == resourceUid11)
                .And
                .Contain(resource => resource.Uid == resourceUid12)
                .And
                .NotContain(resource => resource.Uid == resourceUid22)
                ;

            avendResponse.Events.Should()
                .HaveCount(0, "because in empty database no events could exist");

            avendResponse.LeadsStatistics.AllTimeCount.Should()
                .Be(0, "because in empty database no lead records could exist");

            avendResponse.LeadsStatistics.AllTimeGoal.Should()
                .Be(0, "because in empty database no lead goals could exist");
        }

        [TestMethod]
        public async Task ShouldNotReturnResourceRecordsFromAnotherTenant()
        {
            var resourceData11 = await ResourceData.InitWithSample(TestUser.BobTester, System);
            var resourceUid11 = resourceData11.Resource.Uid;

            var resourceData12 = await ResourceData.InitWithSample(TestUser.BobTester, System);
            var resourceUid12 = resourceData12.Resource.Uid;

            var resourceData21 = await ResourceData.InitWithSample(TestUser.AliceTester, System);
            var resourceUid22 = resourceData21.Resource.Uid;

            var responseJson = BobTA.GetJsonAsync(DashboardUrl);

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.Resources.Should()
                .HaveCount(2, "because we have added just 2 resources for Bob")
                .And
                .Contain(resource => resource.Uid == resourceUid11)
                .And
                .Contain(resource => resource.Uid == resourceUid12)
                .And
                .NotContain(resource => resource.Uid == resourceUid22)
                ;

            avendResponse.Events.Should()
                .HaveCount(0, "because in empty database no events could exist");

            avendResponse.LeadsStatistics.AllTimeCount.Should()
                .Be(0, "because in empty database no lead records could exist");

            avendResponse.LeadsStatistics.AllTimeGoal.Should()
                .Be(0, "because in empty database no lead goals could exist");
        }

        [TestMethod]
        public async Task ShouldNotReturnResourceRecordsFromSeatUsersForTenantAdmin()
        {
            var resourceData11 = await ResourceData.InitWithSample(TestUser.BobTester, System);
            var resourceUid11 = resourceData11.Resource.Uid;

            var resourceData12 = await ResourceData.InitWithSample(TestUser.BobTester, System);
            var resourceUid12 = resourceData12.Resource.Uid;

            var resourceData21 = await ResourceData.InitWithSample(TestUser.AliceTester, System);
            var resourceUid22 = resourceData21.Resource.Uid;

            var responseJson = BobTA.GetJsonAsync(DashboardUrl);

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.Resources.Should()
                .HaveCount(2, "because we have added just 2 resources for Bob")
                .And
                .Contain(resource => resource.Uid == resourceUid11)
                .And
                .Contain(resource => resource.Uid == resourceUid12)
                .And
                .NotContain(resource => resource.Uid == resourceUid22)
                ;

            avendResponse.Events.Should()
                .HaveCount(0, "because in empty database no events could exist");

            avendResponse.LeadsStatistics.AllTimeCount.Should()
                .Be(0, "because in empty database no lead records could exist");

            avendResponse.LeadsStatistics.AllTimeGoal.Should()
                .Be(0, "because in empty database no lead goals could exist");
        }

        [TestMethod]
        public async Task ResourcesSortedByNameAscTest()
        {
            var resourceData11 = await ResourceData.InitWithSample(TestUser.BobTester, System);
            var resourceUid11 = resourceData11.Resource.Uid;

            var resourceData12 = await ResourceData.InitWithSample(TestUser.BobTester, System);
            var resourceUid12 = resourceData12.Resource.Uid;

            var resourceData13 = await ResourceData.InitWithSample(TestUser.BobTester, System);
            var resourceUid13 = resourceData13.Resource.Uid;

            var resourceData21 = await ResourceData.InitWithSample(TestUser.AliceTester, System);
            var resourceUid22 = resourceData21.Resource.Uid;

            var responseJson = BobTA.GetJsonAsync(DashboardUrl + "?resources_sort_field=name&resources_sort_order=asc");

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.Resources.Should()
                .HaveCount(3, "because we have added 3 resources for Bob")
                .And
                .Contain(resource => resource.Uid == resourceUid11)
                .And
                .Contain(resource => resource.Uid == resourceUid12)
                .And
                .Contain(resource => resource.Uid == resourceUid13)
                .And
                .NotContain(resource => resource.Uid == resourceUid22)
                .And
                .BeInAscendingOrder(resource => resource.Name)
                ;
        }

        [TestMethod]
        public async Task ResourcesSortedByUrlDescTest()
        {
            var resourceData11 = await ResourceData.InitWithSample(TestUser.BobTester, System);
            var resourceUid11 = resourceData11.Resource.Uid;

            var resourceData12 = await ResourceData.InitWithSample(TestUser.BobTester, System);
            var resourceUid12 = resourceData12.Resource.Uid;

            var resourceData13 = await ResourceData.InitWithSample(TestUser.BobTester, System);
            var resourceUid13 = resourceData13.Resource.Uid;

            var resourceData21 = await ResourceData.InitWithSample(TestUser.AliceTester, System);
            var resourceUid22 = resourceData21.Resource.Uid;

            var responseJson = BobTA.GetJsonAsync(DashboardUrl + "?resources_sort_field=url&resources_sort_order=desc");

            var avendResponse = await responseJson.AvendResponse<DashboardDTO>();

            avendResponse.Should()
                .NotBeNull("because response should contain a valid dashboard DTO");

            avendResponse.CreatedAt.Should()
                .BeAfter(DateTime.UtcNow.AddSeconds(-5), "because returned data should be recent");

            avendResponse.Resources.Should()
                .HaveCount(3, "because we have added 3 resources for Bob")
                .And
                .Contain(resource => resource.Uid == resourceUid11)
                .And
                .Contain(resource => resource.Uid == resourceUid12)
                .And
                .Contain(resource => resource.Uid == resourceUid13)
                .And
                .NotContain(resource => resource.Uid == resourceUid22)
                .And
                .BeInDescendingOrder(resource => resource.Url)
                ;
        }
    }
}