using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.Resources
{
    public class ResourcesTestBase : IntegrationTest
    {
        protected const string AzureCloudStorageUrl = "blob.core.windows.net";
        protected const string Resources = "users/resources";

        protected readonly string UserBearerToken = TestUser.BobTester.Token;
        protected ResourceData BobResources;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            BobResources = new ResourceData(TestUser.BobTester, System);
        }

        protected async Task<List<ResourceDto>> UserResources(HttpClient user)
        {
            return await user.GetJsonAsync($"{Resources}").AvendListResponse<ResourceDto>();
        }
    }
}