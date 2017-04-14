using System;
using System.Net;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Services.Leads.NetworkDTO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.LeadsController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("LeadsController")]
    [TestCategory("LeadsController.DeleteLead()")]
    // ReSharper disable once InconsistentNaming
    public class Leads_DeleteLead : BaseLeadsEndpointTest
    {
        [TestMethod]
        public async Task ShouldDeleteOwnActiveLeadSuccessfully()
        {
            var leadDto = new LeadDto()
            {
                EventUid = ConferenceEventData.Event.Uid,
                FirstName = "Konstantin",
                Notes = "Test",
            };

            var initialLeads = await BobTA.GetJsonAsync(UrlApiV1Leads).AvendListResponse<LeadDto>();
            var createdLead = await BobTA.PostJsonAsync(UrlApiV1Leads, leadDto).AvendResponse<LeadDto>();
            var deleteResult = await BobTA.DeleteJsonAsync(UrlApiV1Leads + "/" + createdLead.Uid, "").AvendEmptyResponse();

            deleteResult.Should()
                .BeTrue("because we have just added the lead with this GUID");

            var resultingLeads = await BobTA.GetJsonAsync(UrlApiV1Leads).AvendListResponse<LeadDto>();

            resultingLeads.Count.Should()
                .Be(initialLeads.Count, "because after deletion of the lead the list of leads should have the same length as the initial before adding this lead");

            for (var ind = 0; ind < resultingLeads.Count; ind++)
            {
                var resultingLead = resultingLeads[ind];

                resultingLead.Uid.Should()
                    .Be(initialLeads[ind].Uid, "because after deletion of the lead the list of leads should be equivalent to initial one");
            }
        }

        [TestMethod]
        public async Task ShouldDeleteSeatUserLeadByTenantAdmin()
        {
            var cecilLeadDto = await CecileLeadsData.Add();

            var deleteResult = await BobTA.DeleteJsonAsync(UrlApiV1Leads + "/" + cecilLeadDto.Uid, "").AvendEmptyResponse();

            deleteResult.Should()
                .BeTrue("because we have just added the lead with this GUID");
        }

        [TestMethod]
        public async Task ShouldReturnNotFoundWhenUserTriesToDeleteNonexistentLead()
        {
            await BobTA.DeleteJsonAsync(UrlApiV1Leads + "/" + Guid.Empty, "").Response(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task ShouldReturnBadRequestWhenLeadUidIsNotGuid()
        {
            await BobTA.DeleteJsonAsync(UrlApiV1Leads + "/aaa", "").Response(HttpStatusCode.BadRequest, "lead_uid is not a valid GUID");
        }

        [TestMethod]
        public async Task ShouldReturnNotFoundWhenSeatUserTriesToDeleteLeadOfTenantAdmin()
        {
            var cecileTesterClient = System.CreateClient(TestUser.CecileTester.Token);

            var bobLeadDto = await BobLeadsData.Add();

            await cecileTesterClient.DeleteJsonAsync(UrlApiV1Leads + "/" + bobLeadDto.Uid, "").Response(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task ShouldReturnNotFoundWhenUserTriesToDeleteOtherTenantLead()
        {
            var marcLeadData = LeadData.Init(TestUser.MarcTester, ConferenceEventData.Event.Uid.Value, System);

            var markLeadDto = await marcLeadData.Add();

            await BobTA.DeleteJsonAsync(UrlApiV1Leads + "/" + markLeadDto.Uid, "").Response(HttpStatusCode.NotFound);
        }
    }
}