using System.Linq;
using System.Threading.Tasks;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.CrmConfiguration
{
    [TestClass]
    public class CrmConfigurationGetTest : CrmConfigurationTestBase
    {
        protected UserCrmDto C1, C2, C3;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            C1 = await BobTA.PostJsonAsync(
                    "crm/",
                    MakeConfigurationDto(Salesforce, x => x.Name = "AAA"))
                .AvendResponse<UserCrmDto>();
            C2 = await BobTA.PostJsonAsync(
                    "crm",
                    MakeConfigurationDto(Dynamics, x => x.Name = "BBB"))
                .AvendResponse<UserCrmDto>();
            C3 = await BobTA.PostJsonAsync(
                    "crm",
                    MakeConfigurationDto(Salesforce, x => x.Name = "CCC"))
                .AvendResponse<UserCrmDto>();
        }

        [TestMethod]
        public async Task Get()
        {
            var crms = await BobTA.GetJsonAsync("crm")
                .AvendListResponse<UserCrmDto>(3);
            crms.Select(x => x.Name).Should().Equal("AAA", "BBB", "CCC");
        }

        [TestMethod]
        public async Task Pagination()
        {
            var crms = await BobTA.GetJsonAsync("crm?page=1&per_page=2")
                .AvendListResponse<UserCrmDto>(3);
            crms.Select(x => x.Name).Should().Equal("CCC");
        }

        [TestMethod]
        public async Task Sort()
        {
            var crms = await BobTA.GetJsonAsync("crm?sort_field=name&sort_order=asc")
                .AvendListResponse<UserCrmDto>();
            crms.Select(x => x.Name).Should().Equal("AAA", "BBB", "CCC");

            crms = await BobTA.GetJsonAsync("crm?sort_field=type&sort_order=asc")
                .AvendListResponse<UserCrmDto>();
            crms.Select(x => x.Name).Should().Equal("AAA", "CCC", "BBB");
        }
    }
}