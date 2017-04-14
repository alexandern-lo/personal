using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.TenantsController
{
    public class TenantsTestBase : IntegrationTest
    {
        public const string TenantsUrl = "tenants";

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
        }
    }
}
