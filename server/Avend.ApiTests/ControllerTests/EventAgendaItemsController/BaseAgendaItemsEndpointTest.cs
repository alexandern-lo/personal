using System;
using System.Net.Http;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Leads.NetworkDTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventAgendaItemsController
{
    public class BaseAgendaItemsEndpointTest : BaseCrudControllerTest<LeadDto>
    {
        public EventData BobEventData { get; private set; }
        public EventData CecileEventData { get; private set; }

        public LeadData BobLeadsData { get; private set; }
        public LeadData CecileLeadsData { get; private set; }

        protected Guid EventUid;

        protected const string UrlApiV1Events = "events";

        public BaseAgendaItemsEndpointTest() : base(UrlApiV1Events)
        {
        }

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();

            EventUid = ConferenceEventData.Event.Uid.GetValueOrDefault();

            if (ConferenceEventData?.Event == null || !ConferenceEventData.Event.Uid.HasValue)
            {
                throw new InvalidOperationException();
            }

            BobEventData = await EventData.InitWithSampleEvent(TestUser.BobTester, System);
            BobLeadsData = LeadData.Init(TestUser.BobTester, ConferenceEventData.Event.Uid.Value, System);

            CecileEventData = await EventData.InitWithSampleEvent(TestUser.CecileTester, System);
            CecileLeadsData = LeadData.Init(TestUser.CecileTester, ConferenceEventData.Event.Uid.Value, System);
        }

        protected async Task<Guid> CreateAgendaItem()
        {
            var eventAgendaItemDto = new EventAgendaItemDTO
            {
                EventUid = EventUid,
                Name = "Sample conference meeting",
                Date = DateTime.UtcNow.Date,
                StartTime = TimeSpan.FromHours(8),
                EndTime = TimeSpan.FromHours(18),
            };

            return await AlexSA.PostJsonAsync($"events/{EventUid}/agenda_items", eventAgendaItemDto)
                .AvendResponse<Guid>();
        }
    }
}