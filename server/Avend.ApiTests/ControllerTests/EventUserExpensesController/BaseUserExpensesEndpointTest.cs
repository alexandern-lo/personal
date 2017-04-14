using System;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;
using Avend.API.Services.Leads.NetworkDTO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventUserExpensesController
{
    public class BaseUserExpensesEndpointTest : BaseCrudControllerTest<LeadDto>
    {
        public EventData BobEventData { get; private set; }
        public EventData CecileEventData { get; private set; }

        public EventExpenseDataFixtures BobExpensesDataFixtures { get; private set; }
        public EventExpenseDataFixtures CecileExpensesDataFixtures { get; private set; }

        protected const string UrlApiV1Events = "events";

        /// <summary>
        /// 
        /// 
        /// </summary>
        public BaseUserExpensesEndpointTest() : base(UrlApiV1Events)
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

            BobEventData = await EventData.InitWithSampleEvent(TestUser.BobTester, System);
            BobExpensesDataFixtures = new EventExpenseDataFixtures(TestUser.BobTester, System, ConferenceEventData.Event);

            CecileEventData = await EventData.InitWithSampleEvent(TestUser.CecileTester, System);
            CecileExpensesDataFixtures = new EventExpenseDataFixtures(TestUser.CecileTester, System, ConferenceEventData.Event);
        }
    }
}