using System.Net.Http;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests
{
    public class BaseControllerTest : IntegrationTest
    {
        protected const string UrlApiV1UsersPrefix = "/api/v1/users";

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            ConferenceEventData = new EventData(TestUser.AlexTester, System);
        }

        public EventData ConferenceEventData { get; private set; }

        protected const string UrlApiV1CrmSettingsPrefix = "crm_configurations";
        protected const string UrlApiV1UserSettingsPrefix = "users/settings";
        protected const string UrlApiV1UserCrmConfigurationsPrefix = "users/crm_configurations";
        protected const string UrlApiV1CrmSystemsPrefix = "crm_systems";
    }
}