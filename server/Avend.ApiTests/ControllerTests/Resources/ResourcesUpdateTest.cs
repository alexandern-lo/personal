using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.Resources
{
    [TestClass]
    public class ResourcesUpdateTest : ResourcesCreateUpdateTestBase
    {
        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            Resource = await BobResources.Add();
        }

        protected override HttpRequestMessage MakeRequest()
        {
            return new HttpRequestMessage(HttpMethod.Put, $"{Resources}/{Resource.Uid}");
        }
    }
}