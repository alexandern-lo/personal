using System;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;
using Avend.API.Services.Leads.NetworkDTO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.LeadsController
{
    public class BaseLeadsEndpointTest : BaseCrudControllerTest<LeadDto>
    {
        public EventData BobEventData { get; private set; }
        public EventData CecileEventData { get; private set; }

        public LeadData BobLeadsData { get; private set; }
        public LeadData CecileLeadsData { get; private set; }

        protected const string UrlApiV1Leads = "leads";

        protected Guid EventUid;

        public BaseLeadsEndpointTest() : base(UrlApiV1Leads)
        {
        }

        /// <exception cref="InvalidOperationException">If event in EventFixtures from base class is not instantiated.</exception>
        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();

            if (ConferenceEventData?.Event == null || !ConferenceEventData.Event.Uid.HasValue)
            {
                throw new InvalidOperationException();
            }

            EventUid = ConferenceEventData.Event.Uid.GetValueOrDefault();

            BobEventData = await EventData.InitWithSampleEvent(TestUser.BobTester, System);
            BobLeadsData = LeadData.Init(TestUser.BobTester, ConferenceEventData.Event.Uid.Value, System);

            CecileEventData = await EventData.InitWithSampleEvent(TestUser.CecileTester, System);
            CecileLeadsData = LeadData.Init(TestUser.CecileTester, ConferenceEventData.Event.Uid.Value, System);
        }
    }
}