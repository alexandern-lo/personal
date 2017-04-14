using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.API.Model.NetworkDTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.Events
{
    public class EventsControllerTestBase : IntegrationTest
    {
        protected EventDto BobEvent, CecilEvent, AlexEvent, MarcEvent;
        protected EventDto CecileEventRequest;
        protected EventData AlexEvents;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();

            var bobEvents = new EventData(TestUser.BobTester, System);
            BobEvent = await bobEvents.AddFromDto(EventData.MakeSample(e =>
            {
                e.Name = "Bob Event (last week)";
                e.StartDate = new DateTime(2016, 12, 01);
                e.Type = "personal";
                e.Industry = "Nuclear Power Industry";
            }));
            var cecilEvents = new EventData(TestUser.CecileTester, System);
            CecileEventRequest = EventData.MakeSample(e =>
            {
                e.Name = "Cecil Event (next week)";
                e.StartDate = DateTime.Now + TimeSpan.FromDays(5);
                e.Type = "personal";
                e.Industry = "Transportation & Logistics";
                e.Recurring = true;
            });
            CecilEvent = await cecilEvents.AddFromDto(CecileEventRequest);

            AlexEvents = new EventData(TestUser.AlexTester, System);
            AlexEvent = await AlexEvents.AddFromDto(EventData.MakeSample(e =>
            {
                e.Name = "Alex Event (ongoing)";
                e.StartDate = DateTime.Now - TimeSpan.FromDays(1);
                e.EndDate = DateTime.Now + TimeSpan.FromDays(2);
                e.Type = "conference";
                e.Industry = "Nuclear Power Industry";
                e.Recurring = true; //this will be ignored since alex is super admin
            }));

            var marcEvents = new EventData(TestUser.MarcTester, System);
            MarcEvent = await marcEvents.AddFromDto(EventData.MakeSample(e =>
            {
                e.Name = "Mark Event";
                e.StartDate = new DateTime(2017, 01, 01);
            }));
        }
    }
}