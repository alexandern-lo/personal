using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.CrmConfiguration
{
    [TestClass]
    public class CrmConfigurationCreateTest : CrmConfigurationCreateUpdateTestBase
    {
        protected override HttpRequestMessage MakeRequest()
        {
            return new HttpRequestMessage(HttpMethod.Post, "crm");
        }
    }
}