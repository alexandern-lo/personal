using System.Threading.Tasks;

using Avend.API.Services.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.LeadsController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("LeadsController")]
    [TestCategory("LeadsController.ExportLeadsToCrm()")]
    // ReSharper disable once InconsistentNaming
    public class Leads_ExportLeadsToSalesForce : BaseLeadsEndpointTest
    {
        [TestMethod]
        public async Task TestSalesForceConnection()
        {
            await CrmDefaultsHelper.RetrieveSalesForceLeadMetadata();
        }
    }
}