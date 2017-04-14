using Avend.API.Model;
using Avend.API.Services.Helpers;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

namespace Avend.ApiTests.ServiceTests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class CrmDefaultsHelperTest
    {
        [DataTestMethod]
        [DataRow(CrmSystemAbbreviation.Salesforce)]
        [DataRow(CrmSystemAbbreviation.Dynamics365)]
        public void TestDefaultCrmCreation(CrmSystemAbbreviation abbreviation)
        {
            var crmSystem = new CrmSystem()
            {
                Abbreviation = abbreviation,
                Name = CrmDefaultsHelper.CrmNames[abbreviation],

                DefaultFieldMappings = JsonConvert.SerializeObject(CrmDefaultsHelper.DefaultCrmMappings[abbreviation], Formatting.Indented),

                AuthorizationParams = JsonConvert.SerializeObject(CrmDefaultsHelper.CrmAuthorizationParams[abbreviation], Formatting.Indented),

                TokenRequestUrl = CrmDefaultsHelper.CrmTokenRequestUrls[abbreviation],
                TokenRequestParams = JsonConvert.SerializeObject(CrmDefaultsHelper.CrmTokenRequestParams[abbreviation], Formatting.Indented),
            };

            crmSystem.Abbreviation.Should()
                .Be(abbreviation, "because we just create a sample CRM system using default parameters to ensure that CrmDefaultsHelper is a correct class");
        }
    }
}
