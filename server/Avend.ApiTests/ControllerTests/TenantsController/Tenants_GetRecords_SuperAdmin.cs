using System.Linq;
using System.Threading.Tasks;

using Avend.ApiTests.Infrastructure;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Services.Subscriptions.NetworkDTO;
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
    public class Tenants_GetRecords_SuperAdmin : TenantsTestBase
    {
        [TestMethod]
        [When(Context.RoleIs, UserRoles.SuperAdmin)]
        [When(Context.RequestFor, "Tenants list")]
        [When(Context.QueryHasParameter, "sort_order=desc")]
        [It(Should.ReturnHttpCode, 200)]
        [It(Should.ReturnList, "With company names sorted by company name in descendent order")]
        public async Task TestSortOrder()
        {
            var response = await AlexSA.GetJsonAsync(TenantsUrl + "?sort_order=desc")
                .AvendListResponse<TenantDto>();

            response.Should()
                .Contain(record => record.Uid == BobSubscriptionUid, "we expect to get the Bob's subscription back in list")
                .And
                .Contain(record => record.Uid == MarcSubscriptionUid, "we expect to get the Marc's subscription back in list")
                ;

            var indexOfBob = response.IndexOf(response.FirstOrDefault(record => record.Uid == BobSubscriptionUid));
            var indexOfMarc = response.IndexOf(response.FirstOrDefault(record => record.Uid == MarcSubscriptionUid));

            indexOfBob.Should()
                .BeGreaterThan(indexOfMarc,
                "when sorting in descending order Marc Tester should precede Bob Tester"
                )
                ;
        }

        [TestMethod]
        [When(Context.RoleIs, UserRoles.SuperAdmin)]
        [When(Context.RequestFor, "Tenants list")]
        [When(Context.QueryHasParameter, "q=Bob")]
        [It(Should.ReturnHttpCode, 200)]
        [It(Should.ReturnList, "With company names satisfying filter condition")]
        public async Task TestFilter()
        {
            var response = await AlexSA.GetJsonAsync(TenantsUrl + "?q=Bob")
                .AvendListResponse<TenantDto>();

            response.Should()
                .Contain(record => record.Uid == BobSubscriptionUid, "we expect to get the Bob's subscription back in list")
                .And
                .NotContain(record => record.Uid == MarcSubscriptionUid, "we expect to not get the Marc's subscription back in list due to filter")
                ;
        }

        [TestMethod]
        [When(Context.RoleIs, UserRoles.SuperAdmin)]
        [When(Context.RequestFor, "Tenants list")]
        [When(Context.QueryHasParameter, "per_page=1")]
        [It(Should.ReturnHttpCode, 200)]
        [It(Should.ReturnList, "With single record that have alphabetically first company name")]
        public async Task TestPageSize()
        {
            var response = await AlexSA.GetJsonAsync(TenantsUrl + "?per_page=1")
                .AvendListResponse<TenantDto>();

            response.Should()
                .HaveCount(1, "we expect to get the list of the requested page size")
                .And
                .Contain(record => record.Uid == BobSubscriptionUid, "we expect to get the Bob's subscription back in list")
                .And
                .NotContain(record => record.Uid == MarcSubscriptionUid, "we expect to not get the Marc's subscription back in list as it belongs to the next page")
                ;
        }

        [TestMethod]
        [When(Context.RoleIs, UserRoles.SuperAdmin)]
        [When(Context.RequestFor, "Tenants list")]
        [When(Context.QueryHasParameter, "page=1&per_page=1")]
        [It(Should.ReturnHttpCode, 200)]
        [It(Should.ReturnList, "With single record that have alphabetically second company name")]
        public async Task TestPageOffset()
        {
            var response = await AlexSA.GetJsonAsync(TenantsUrl + "?page=1&per_page=1")
                .AvendListResponse<TenantDto>();

            response.Should()
                .HaveCount(1, "we expect to get the list of the requested page size")
                .And
                .Contain(record => record.Uid == MarcSubscriptionUid, "we expect to get the Marc's subscription back in list")
                .And
                .NotContain(record => record.Uid == BobSubscriptionUid, "we expect to not get the Bob's subscription back in list due to filter as it belongs to the previous page")
                ;
        }
    }
}