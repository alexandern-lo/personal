using System;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;
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
    public class SalesForceConnectorTest : IntegrationTest
    {
        private CrmRecord _crmConfig;

        [TestInitialize]
        public override async Task Init()
        {
            CrmData = CrmData.Init(TestUser.BobTester, System);
            _crmConfig = await CrmData.AuthorizeSalesForce();
            LeadData = await LeadData.Init(TestUser.BobTester, System);
            await LeadData.Add();
            Connector = new SalesForceConnector(CrmData.SalesForceConfig);
        }

        public SalesForceConnector Connector { get; set; }

        public LeadData LeadData { get; set; }

        public CrmData CrmData { get; set; }

        [TestMethod]
        public async Task Login()
        {
            var oauth = new SalesForceOAuthApi(CrmData.SalesForceConfig);
            try
            {
                var username = "kkb_ru@hotmail.com";
                var password = "09ebU-nv8vdu=vcs";
                var securityToken = "tUfUXjihMJoyMrcP45qqjDuO";
                var response = await oauth.LoginWithUsernamePassword(username, password + securityToken);
                Console.WriteLine(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [TestMethod]
        public void TestAuthUrlGeneration()
        {
            var authorizationUrl = Connector.ConstructAuthorizationUrl(_crmConfig);

            Console.Out.WriteLine(authorizationUrl);
            Console.Error.WriteLine(authorizationUrl);
        }

        [TestMethod]
        public async Task TestGetResourcesList()
        {
            var userCrmConfiguration = _crmConfig;
            var resources = await Connector.GetResourcesList(userCrmConfiguration);

            resources.Should()
                .HaveCount(record => record > 0,
                    "Because access_token is expected to be returned for valid refresh_token");

            resources.Should()
                .ContainKey("sobjects", "Because we need access to managing leads");

            resources.Should()
                .ContainKey("query", "Because we need access to querying leads");

            resources.Should()
                .ContainKey("search", "Because we need access to searching leads");
        }

        [TestMethod]
        public async Task TestGetLeadStructure()
        {
            var userCrmConfiguration = _crmConfig;
            var leadStructure = await Connector.GetLeadStructure(userCrmConfiguration);

            leadStructure.Should()
                .HaveCount(record => record > 0, "Because result is expected to be not empty");

            leadStructure.Should()
                .HaveCount(46, "Because we expect to have precisely 46 fields in SalesForce Lead object");

            leadStructure.Should()
                .Contain("FirstName", "Because we expect to export first name");

            leadStructure.Should()
                .Contain("LastName", "Because we expect to export last name");
        }

        [TestMethod]
        public async Task TestUploadLeadSuccess()
        {
            var lead = LeadData.GetDbRecord(0);
            lead.FirstName = "Short FirstName";
            var result = await Connector.UploadLead(lead, _crmConfig);

            result.Should()
                .NotBeNull("because we always expect some export result, even negative");
            result.Status.Should()
                .Be(LeadExportResultStatus.Created, "because we expect success for this lead");
            result.Message.Should()
                .BeEmpty("because we do not expect any message when succesfully exporting lead");

            var leadRetrieval = await Connector.RetrieveLead(result.LeadRecord, _crmConfig);

            leadRetrieval.Should()
                .NotBeNull("because we always expect some export result, even negative");
            leadRetrieval.Should()
                .ContainKey("Id", "because we expect success for this lead");
            (leadRetrieval["Id"] as string).Should()
                .NotBeNullOrEmpty("because we do not expect any message when succesfully exporting lead");
        }

        [TestMethod]
        public async Task TestUploadLeadFailure()
        {
            var lead = LeadData.GetDbRecord(0);
            lead.FirstName = "";
            lead.CompanyName = null;
            var result = await Connector.UploadLead(lead, _crmConfig);

            result.Should()
                .NotBeNull("because we always expect some export result, even negative");
            result.Status.Should()
                .Be(LeadExportResultStatus.Failed, "because we expect failure for this lead");
        }
    }
}