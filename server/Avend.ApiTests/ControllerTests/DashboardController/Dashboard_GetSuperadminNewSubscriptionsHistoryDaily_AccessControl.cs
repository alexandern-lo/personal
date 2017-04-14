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
    public class Dashboard_GetSuperadminNewSubscriptionsHistoryDaily_AccessControl : BaseDashboardEndpointTest
    {
        public const string DashboardUrl = "dashboard/superadmin/history/new_subscriptions/daily?type=paid";

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