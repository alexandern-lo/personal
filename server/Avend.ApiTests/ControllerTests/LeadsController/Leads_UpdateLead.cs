using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
    public class Leads_UpdateLead : BaseLeadsEndpointTest
    {
        protected LeadDto Lead;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            Lead = await BobLeadsData.Add();
            Lead.EventUid = Lead.Event.Uid;
        }

        [TestMethod]
        public async Task Update()
        {
            Lead.FirstName = Lead.FirstName + "[Updated]";
            Lead.Notes = "Completely new notes";

            var updated = await UpdatedLead();

            updated.FirstName.Should().Be(Lead.FirstName);
            updated.Notes.Should().Be(Lead.Notes);
        }

        [TestMethod]
        public async Task UpdateHonorsClientSideUpdatedAt()
        {
            Lead.ClientsideUpdatedAt = DateTime.Now.AddHours(-1);
            Lead.FirstName = Lead.FirstName + " [Updated]";
            var updatedLead = await UpdatedLead();
            updatedLead.FirstName.Should().Be(Lead.FirstName);

            Lead.FirstName = Lead.FirstName + " [Old Update]";
            Lead.ClientsideUpdatedAt = DateTime.Now.AddHours(-2);
            await BobTA.PutJsonAsync(UrlApiV1Leads + "/" + Lead.Uid, Lead)
                .AvendErrorResponse(HttpStatusCode.Conflict, "cannot update lead with outdated data");
        }

        [TestMethod]
        public async Task InvalidEventUid()
        {
            Lead.EventUid = Guid.NewGuid();
            await BobTA.PutJsonAsync(UrlApiV1Leads + "/" + Lead.Uid, Lead)
                .AvendErrorResponse(HttpStatusCode.BadRequest, "cannot set event which does not exists");

            Lead.EventUid = null;
            await BobTA.PutJsonAsync(UrlApiV1Leads + "/" + Lead.Uid, Lead)
                .AvendErrorResponse(HttpStatusCode.BadRequest, "cannot set null event");

            var cecileEventDto = await CecileEventData.AddFromSample();
            Lead.EventUid = cecileEventDto.Uid;
            await BobTA.PutJsonAsync(UrlApiV1Leads + "/" + Lead.Uid, Lead)
                .AvendErrorResponse(HttpStatusCode.BadRequest, "cannot set other user event");
        }

        [TestMethod]
        public async Task InvalidLeadNotFound()
        {
            await BobTA.PutJsonAsync(UrlApiV1Leads + "/" + Guid.NewGuid(), Lead)
                .AvendErrorResponse();
        }

        private async Task<LeadDto> UpdatedLead()
        {
            return await BobTA.PutJsonAsync(UrlApiV1Leads + "/" + Lead.Uid, Lead).AvendResponse<LeadDto>();
        }
    }
}