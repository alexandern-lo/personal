using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.DashboardController
{
    public class BaseDashboardEndpointTest : BaseControllerTest
    {
        protected const string UrlApiV1Dashboard = "dashboard";

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
        }
    }
}