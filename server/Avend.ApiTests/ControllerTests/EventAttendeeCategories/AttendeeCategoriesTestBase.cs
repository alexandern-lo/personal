using System;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventAttendeeCategories
{
    public class AttendeeCategoriesTestBase : IntegrationTest
    {
        protected Guid EventUid;
        protected EventData AlexEvents;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            AlexEvents = await EventData.InitWithSampleEvent(TestUser.AlexTester, System);
            EventUid = AlexEvents.Event.Uid.GetValueOrDefault();
        }
        
    }
}
