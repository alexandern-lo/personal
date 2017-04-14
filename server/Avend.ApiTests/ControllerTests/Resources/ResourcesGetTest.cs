using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.Resources
{
    [TestClass]
    public class ResourcesGetTest : ResourcesTestBase
    {
        protected ResourceDto R1, R2, R3;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            R1 = await BobResources.Add(r =>
            {
                r.Name = "AAA";
                r.MimeType = "text/html";
            });
            var marcResources = new ResourceData(TestUser.MarcTester, System);
            R2 = await marcResources.Add(r =>
            {
                r.Name = "BBB";
                r.MimeType = "application/pdf";
            });
            var cecileResources = new ResourceData(TestUser.CecileTester, System);
            R3 = await cecileResources.Add(r =>
            {
                r.Name = "CCC";
                r.MimeType = "text/xml";
            });
        }

        [TestMethod]
        public async Task GetByUid()
        {
            await BobTA.GetJsonAsync($"{Resources}/{R1.Uid}").Response();
        }

        [TestMethod]
        public async Task Roles()
        {
            await CecileSU.GetJsonAsync($"{Resources}/{R1.Uid}").Response(HttpStatusCode.NotFound);

            await MarcTA.GetJsonAsync($"{Resources}/{R1.Uid}").Response(HttpStatusCode.NotFound);

            var cecileResources = await CecileSU.GetJsonAsync($"{Resources}")
                .AvendListResponse<ResourceDto>(1);
            cecileResources.Select(x => x.Name).Should().Equal("CCC");

            await MarcTA.GetJsonAsync($"{Resources}?tenant={TestUser.CecileTester.SubscriptionUid}")
                .AvendListResponse<ResourceDto>(0);
        }

        [TestMethod]
        public async Task GetList()
        {
            var response = await BobTA.GetJsonAsync($"{Resources}")
                .AvendListResponse<ResourceDto>(2);
            response.Count.Should().Be(2);
            response[0].Uid.Should().NotBeNull();
            response[0].User.Uid.Should().Be(TestUser.BobTester.Uid);
            response[0].Tenant.Uid.Should().Be(TestUser.BobTester.SubscriptionUid);
        }

        [TestMethod]
        public async Task Pagination()
        {
            var query = $"{Resources}?sort_field=name&per_page=2&page=1";
            var response = await AlexSA.GetJsonAsync(query)
                .AvendListResponse<ResourceDto>(3);
            response.Select(x => x.Name).Should().Equal("CCC");
        }

        [TestMethod]
        public async Task FilterByTenant()
        {
            var bob = TestUser.BobTester.SubscriptionUid;
            var query = $"{Resources}?tenant={bob}&sort_field=name";
            var response = await AlexSA.GetJsonAsync(query).AvendListResponse<ResourceDto>(2);
            response.Select(x => x.Name).Should().Equal("AAA", "CCC");
        }

        [TestMethod]
        public async Task FilterByUser()
        {
            var cecile = TestUser.CecileTester.Uid;
            var query = $"{Resources}?user={cecile}";
            var response = await BobTA.GetJsonAsync(query).AvendListResponse<ResourceDto>(1);
            response[0].Name.Should().Be("CCC");
        }

        [TestMethod]
        public async Task Search()
        {
            var response = await BobTA.GetJsonAsync($"{Resources}?q=AA").AvendListResponse<ResourceDto>(1);
            response.Select(x => x.Name).Should().Equal("AAA");
        }

        [TestMethod]
        public async Task Sort()
        {
            var response = await AlexSA.GetJsonAsync($"{Resources}?sort_field=name&sort_order=desc")
                .AvendListResponse<ResourceDto>();
            response.Select(x => x.Name).Should().Equal("CCC", "BBB", "AAA");

            response = await AlexSA.GetJsonAsync($"{Resources}?sort_field=created_at&sort_order=desc")
                .AvendListResponse<ResourceDto>();
            response.Select(x => x.Name).Should().Equal("CCC", "BBB", "AAA");

            response = await AlexSA.GetJsonAsync($"{Resources}?sort_field=type&sort_order=asc")
                .AvendListResponse<ResourceDto>();
            response.Select(x => x.Name).Should().Equal("BBB", "AAA", "CCC");

            response = await AlexSA.GetJsonAsync($"{Resources}?sort_field=type&sort_order=desc")
                .AvendListResponse<ResourceDto>();
            response.Select(x => x.Name).Should().Equal("CCC", "AAA", "BBB");
        }
    }
}