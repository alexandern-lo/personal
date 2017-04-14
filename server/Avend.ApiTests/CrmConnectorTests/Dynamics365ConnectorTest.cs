using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model;
using Avend.API.Services.Crm;
using Avend.OAuth;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.CrmConnectorTests
{
    [TestClass]
    [TestCategory("CrmConnectors")]
    [TestCategory("Integrational")]
    public class Dynamics365ConnectorTest : IntegrationTest
    {
        Dynamics365Connector _connector;
        CrmRecord _config;

        [TestInitialize]
        public async Task Initialize()
        {
            var crmData = CrmData.Init(TestUser.BobTester, System);
            _connector = new Dynamics365Connector(crmData.DynamicsConfig);
            _config = await crmData.AuthorizeDynamics();
        }

        [TestMethod]
        public async Task GetAccessCodeUsingRefreshToken()
        {
            var response = await _connector.GetAccessCodeUsingRefreshToken(_config);
            Assert.IsNotNull(response[OAuthApi.AccessToken]);
            Assert.IsNotNull(response[OAuthApi.RefreshToken]);
        }

        [TestMethod]
        //See this thread - https://basecamp.com/2173487/projects/12065966/messages/66322224
        //Ignore this test if trial expires, Enable when we get proper Dynamics 365 user
        public async Task UploadLead()
        {
            var eventData = await EventData.InitWithSampleEvent(TestUser.BobTester, System);
            var leadData = LeadData.Init(TestUser.BobTester, eventData.Event.Uid.GetValueOrDefault(), System);
            var leadDto = await leadData.Add();

            using (var services = System.GetServices())
            {
                var db = services.GetService<AvendDbContext>();
                var lead = (from l in db.LeadsTable where l.Uid == leadDto.Uid.GetValueOrDefault() select l).First();
                var result = await _connector.UploadLead(lead, _config);

                result.Should()
                    .NotBeNull("because we always expect some export result, even negative");
                result.Status.Should()
                    .Be(LeadExportResultStatus.Created, "because we expect success for this lead");
                result.Message.Should()
                    .BeEmpty("because we do not expect any message when succesfully exporting lead");

                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                        _config.AccessToken);
                    http.BaseAddress = new Uri(_config.Url);
                    var responseMessage =
                        await http.GetAsync("/api/data/v8.2/leads?$filter=firstname eq '" + lead.FirstName + "'");
                    responseMessage.IsSuccessStatusCode.Should().BeTrue();
                }
            }
        }
    }
}