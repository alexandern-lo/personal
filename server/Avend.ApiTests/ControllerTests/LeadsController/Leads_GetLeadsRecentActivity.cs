using System;
using System.Collections.Generic;
using System.Linq;
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
    [TestCategory("LeadsController.GetLeadsRecentActivity()")]
    // ReSharper disable once InconsistentNaming
    public class Leads_GetLeadsRecentActivity : BaseLeadsEndpointTest
    {
        private const string UrlApiV1LeadsRecentActivity = "leads/recent_activity";

        [TestMethod]
        public async Task ShouldReturnSingleCreatedActionWhenOnlySingleLeadWasAdded()
        {
            await BobLeadsData.Add();

            var lead = BobLeadsData.Leads.First();

            var apiResponseJson = await BobTA.GetJsonAsync(UrlApiV1LeadsRecentActivity)
                .AvendResponse<List<LeadRecentActivityDto>>();

            apiResponseJson.Count.Should()
                .BeGreaterOrEqualTo(1, "because we have just added a single lead for Bob");

            apiResponseJson.First().Should()
                .Match(obj => (obj as LeadRecentActivityDto).LeadUid == lead.Uid, "because we expect to get activity for the lead we have just added for Bob")
                .And
                .Match(obj => (obj as LeadRecentActivityDto).PerformedAction == LeadPerformedAction.Created, "because we have just added a single Lead that was not changed")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnProperlySortedActionsWhenTwoLeadsWereAdded()
        {
            var lead1 = await BobLeadsData.Add();

            var lead2 = await BobLeadsData.Add();

            var apiResponseJson = await BobTA.GetJsonAsync(UrlApiV1LeadsRecentActivity)
                .AvendResponse<List<LeadRecentActivityDto>>();

            apiResponseJson.Count.Should()
                .Be(2, "because we have just added two leads for Bob");

            apiResponseJson.First().Should()
                .Match(obj => (obj as LeadRecentActivityDto).LeadUid == lead2.Uid, "because the first recent activity should be the last performed")
                .And
                .Match(obj => (obj as LeadRecentActivityDto).PerformedAction == LeadPerformedAction.Created, "because we have only added leads")
                ;

            apiResponseJson.Last().Should()
                .Match(obj => (obj as LeadRecentActivityDto).LeadUid == lead1.Uid, "because the last recent activity should be the first performed")
                .And
                .Match(obj => (obj as LeadRecentActivityDto).PerformedAction == LeadPerformedAction.Created, "because we have only added leads")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnSingleUpdatedActionWhenOnlySingleLeadWasAddedAndThenUpdated()
        {
            await BobLeadsData.Add();

            var lead = BobLeadsData.Leads.First();

            var leadDto = new LeadDto()
            {
                EventUid = lead.Event.Uid,
                Uid = lead.Uid,
                City = "Updated city",
            };

            var leadUpdateResponseJson = await BobTA.PutJsonAsync(UrlApiV1Leads + "/" + lead.Uid, leadDto)
                .AvendResponse<LeadDto>();

            leadUpdateResponseJson.Uid.Should()
                .Be(lead.Uid, "because we have updated lead with this Uid");

            var apiResponseJson = await BobTA.GetJsonAsync(UrlApiV1LeadsRecentActivity)
                .AvendResponse<List<LeadRecentActivityDto>>();

            apiResponseJson.Count.Should()
                .BeGreaterOrEqualTo(1, "because we have just a single lead in LeadData");

            apiResponseJson.First().Should()
                .Match(obj => (obj as LeadRecentActivityDto).LeadUid == lead.Uid, "because we expect to get activity for the lead we have just added for Bob")
                .And
                .Match(obj => (obj as LeadRecentActivityDto).PerformedAction == LeadPerformedAction.Updated, "because we have updated lead with this Uid")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnSingleUpdatedActionWhenOnlySingleLeadWasAddedAndThenDeleted()
        {
            await BobLeadsData.Add();

            var lead = BobLeadsData.Leads.First();

            var leadUpdateResponseJson = await BobTA.DeleteJsonAsync(UrlApiV1Leads + "/" + lead.Uid)
                .AvendEmptyResponse();

            leadUpdateResponseJson.Should()
                .BeTrue("because we have deleted lead with proper Uid");

            var apiResponseJson = await BobTA.GetJsonAsync(UrlApiV1LeadsRecentActivity)
                .AvendResponse<List<LeadRecentActivityDto>>();

            apiResponseJson.Count.Should()
                .BeGreaterOrEqualTo(1, "because we have just a single lead in LeadData");

            apiResponseJson.First().Should()
                .Match(obj => (obj as LeadRecentActivityDto).LeadUid == lead.Uid, "because we have want to ge the action for the lead present in LeadData")
                .And
                .Match(obj => (obj as LeadRecentActivityDto).PerformedAction == LeadPerformedAction.Deleted, "because we have updated lead with this Uid")
                ;
        }

        [TestMethod]
        public async Task ShouldNotReturnLeadsFromSeatUsers()
        {
            await CecileLeadsData.Add();

            await BobLeadsData.Add();

            var lead = BobLeadsData.Leads.First();

            var apiResponseJson = await BobTA.GetJsonAsync(UrlApiV1LeadsRecentActivity)
                .AvendResponse<List<LeadRecentActivityDto>>();

            apiResponseJson.Count.Should()
                .BeGreaterOrEqualTo(1, "because we have just a single lead for Bob");

            apiResponseJson.First().Should()
                .Match(obj => (obj as LeadRecentActivityDto).LeadUid == lead.Uid, "because we have want to ge the proper action for the Bob's lead")
                .And
                .Match(obj => (obj as LeadRecentActivityDto).PerformedAction == LeadPerformedAction.Created, "because we have just added a single Lead that was not changed yet")
                ;
        }

        [TestMethod]
        public async Task ShouldNotReturnLeadActionsFromAnotherTenant()
        {
            var marcLeadData = LeadData.Init(TestUser.MarcTester, ConferenceEventData.Event.Uid.Value, System);

            await marcLeadData.Add();

            await BobLeadsData.Add();

            var lead = BobLeadsData.Leads.First();

            var apiResponseJson = await BobTA.GetJsonAsync(UrlApiV1LeadsRecentActivity)
                .AvendResponse<List<LeadRecentActivityDto>>();

            apiResponseJson.Count.Should()
                .BeGreaterOrEqualTo(1, "because we have just a single lead for Bob");

            apiResponseJson.First().Should()
                .Match(obj => (obj as LeadRecentActivityDto).LeadUid == lead.Uid, "because we have want to ge the proper action for the Bob's lead")
                .And
                .Match(obj => (obj as LeadRecentActivityDto).PerformedAction == LeadPerformedAction.Created, "because we have just added a single Lead that was not changed yet")
                ;
        }
    }
}