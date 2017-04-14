using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Leads.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.LeadsController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("LeadsController")]
    [TestCategory("LeadsController.GetLeads()")]
    public class Leads_GetLeads : BaseLeadsEndpointTest
    {

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            var marcLeadData = LeadData.Init(TestUser.MarcTester, EventUid, System);
            await marcLeadData.Add();

            await BobLeadsData.Add(x => x.FirstName = "A");
            await BobLeadsData.Add(x => x.FirstName = "B");
            await BobLeadsData.Add(x =>
            {
                x.EventUid = EventUid;
                x.FirstName = "C";
                x.Notes = "Test leads sorting";
            });
        }

        [TestMethod]
        public async Task GetLeads()
        {
            var questions = await EventQuestionData.Create(TestUser.AlexTester, System, ConferenceEventData.Event);
            await questions.AddQuestion(EventQuestionData.MakeSample());

            var dto = BobLeadsData.Leads[0];
            var leads = await BobTA.GetJsonAsync("leads")
                .AvendResponse<List<LeadDto>>();

            leads.Count.Should()
                .Be(3, "Bob can see only it own leads");
            var lead = leads[0];
            lead.Event.Uid.Should()
                .Be(EventUid, "because we have just added such lead record");
            lead.Event.Questions.Should().NotBeEmpty();
            lead.Event.Questions[0].Choices.Should().NotBeEmpty();
            lead.BusinessCardBackUrl.Should()
                .Be(dto.BusinessCardBackUrl, "because we have just added such lead record");
            lead.Owner.Uid.Should().Be(TestUser.BobTester.Uid);
            var bobProfile = await BobTA.GetJsonAsync("profile").AvendResponse<UserProfileDto>();
            lead.Tenant.Uid.Should().Be(bobProfile.CurrentSubscription.Uid);
        }

        [TestMethod]
        public async Task GetLeadsSorting()
        {
            var list = await BobTA.GetJsonAsync("leads?sort_field=first_name")
                .AvendListResponse<LeadDto>(3);

            list.Select(x => x.FirstName).Should().Equal("A", "B", "C");

            list = await BobTA.GetJsonAsync("leads?sort_field=first_name&sort_order=desc")
                    .AvendResponse<List<LeadDto>>();

            list.Select(x => x.FirstName).Should().Equal("C", "B", "A");
        }
    }
}