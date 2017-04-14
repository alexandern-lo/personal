using System.Net;
using System.Threading.Tasks;

using Avend.ApiTests.Infrastructure;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.Common.TestBed.Attributes;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.TenantsController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("TenantsController")]
    [TestCategory("TenantsController.GetRecords()")]
    // ReSharper disable once InconsistentNaming
    public class Tenants_GetRecords_AccessControl : TenantsTestBase
    {
        [TestMethod]
        [When(Context.RoleIs, UserRoles.TenantAdmin)]
        [When(Context.RequestFor, "Tenants list")]
        [It(Should.ReturnHttpCode, 403)]
        [It(Should.ReturnResponse, "With empty body")]
        public async Task ShouldReturnForbiddenWhenCalledByTenantAdmin()
        {
            var response = await BobTA.GetJsonAsync(TenantsUrl)
                .Response(HttpStatusCode.Forbidden,
                    "because only superadmins should have access to superadmin dashboard data");

            var responseContent = await response.Content.ReadAsStringAsync();

            responseContent.Should()
                .BeNullOrEmpty("because the access denial should happen on controller level and produce no response body");
        }

        [TestMethod]
        [When(Context.RoleIs, UserRoles.SeatUser)]
        [When(Context.RequestFor, "Tenants list")]
        [It(Should.ReturnHttpCode, 403)]
        [It(Should.ReturnResponse, "With empty body")]
        public async Task ShouldReturnForbiddenWhenCalledBySeatUser()
        {
            var response = await CecileSU.GetJsonAsync(TenantsUrl)
                .Response(HttpStatusCode.Forbidden,
                    "because only superadmins should have access to superadmin dashboard data");

            var responseContent = await response.Content.ReadAsStringAsync();

            responseContent.Should()
                .BeNullOrEmpty("because the access denial should happen on controller level and produce no response body");
        }
    }
}