using System;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventAttendee
{
    public class AttendeesControllerTestBase : IntegrationTest
    {
        protected EventData EventData;
        protected TestAttendeeCategory Prop1, Prop2;
        protected Guid EventUid;


        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            EventData = await EventData.InitWithSampleEvent(TestUser.BobTester, System);
            EventUid = EventData.Event.Uid.GetValueOrDefault();
            Prop1 = await EventData.Addcategory(EventUid, "Prop1", "Opt1.1", "Opt1.2", "Opt1.3");
            Prop2 = await EventData.Addcategory(EventUid, "Prop2", "Opt2.1", "Opt2.2", "Opt3.2");
        }
    }
}