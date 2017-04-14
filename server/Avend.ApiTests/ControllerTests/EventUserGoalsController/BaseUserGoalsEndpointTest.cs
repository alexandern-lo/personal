using System;
using System.Net.Http;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;
using Avend.API.Services.Leads.NetworkDTO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventUserGoalsController
{
    public class BaseUserGoalsEndpointTest : BaseCrudControllerTest<LeadDto>
    {
        public HttpClient AlexTesterClient { get; private set; }
        public HttpClient CecileTesterClient { get; private set; }

        public EventData BobEventData { get; private set; }
        public EventData CecileEventData { get; private set; }

        public EventGoalsDataFixtures BobGoalsDataFixtures { get; private set; }
        public EventGoalsDataFixtures CecileGoalsDataFixtures { get; private set; }

        protected const string UrlApiV1Events = "events";

        /// <summary>
        /// 
        /// 
        /// </summary>
        public BaseUserGoalsEndpointTest() : base(UrlApiV1Events)
        {
        }

        /// <exception cref="InvalidOperationException">If event in EventFixtures from base class is not instantiated.</exception>
        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();

            AlexTesterClient = System.CreateClient(TestUser.AlexTester.Token);
            CecileTesterClient = System.CreateClient(TestUser.CecileTester.Token);

            if (ConferenceEventData?.Event == null || !ConferenceEventData.Event.Uid.HasValue)
            {
                throw new InvalidOperationException();
            }

            BobEventData = await EventData.InitWithSampleEvent(TestUser.BobTester, System);
            BobGoalsDataFixtures = new EventGoalsDataFixtures(TestUser.BobTester, System, ConferenceEventData.Event);

            CecileEventData = await EventData.InitWithSampleEvent(TestUser.CecileTester, System);
            CecileGoalsDataFixtures = new EventGoalsDataFixtures(TestUser.CecileTester, System, ConferenceEventData.Event);
        }

        public override void Dispose()
        {
            AlexTesterClient?.Dispose();
            CecileTesterClient?.Dispose();

            base.Dispose();
        }
    }
}