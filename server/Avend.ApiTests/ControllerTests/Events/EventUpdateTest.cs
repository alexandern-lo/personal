using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.Events
{
    [TestClass]
    public class EventUpdateTest : EventsControllerTestBase
    {
        [TestMethod]
        public async Task UpdateEvent()
        {
            CecilEvent.Type = EventRecord.EventTypeConference;
            CecilEvent.Recurring = true;
            CecilEvent.EndDate = DateTime.Now + TimeSpan.FromDays(10);
            var @event = await BobTA.PutJsonAsync($"events/{CecilEvent.Uid}", CecilEvent)
                .AvendResponse<EventDto>(HttpStatusCode.OK, "admin can update users event");
            @event.Uid.Should().NotBeEmpty("returned event's Uid must be valid");
            @event.Type.Should().Be(EventRecord.EventTypePersonal, "admins create personal events");
            @event.Recurring.Should().BeTrue();
            @event.EndDate.Should().NotHaveValue("end date ignored if recurring is true");
            @event.Owner?.Uid.Should().Be(TestUser.CecileTester.Uid);

            BobEvent.EndDate = DateTime.Now + TimeSpan.FromDays(10);
            BobEvent.Recurring = false;
            @event = await BobTA.PutJsonAsync($"events/{BobEvent.Uid}", BobEvent).AvendResponse<EventDto>();
            var expectedEndDate = new DateTime(2016, 12, 01, 23, 59, 59);
            @event.EndDate.Should().Be(expectedEndDate, "personal events always have equal start and end dates");
        }

        [TestMethod]
        public async Task UpdateEvent_Roles()
        {
            await BobTA.PutJsonAsync($"events/{MarcEvent.Uid}", MarcEvent)
                .Response(HttpStatusCode.NotFound, "admin cannot update event which does not belong to this tenant");

            await CecileSU.PutJsonAsync($"events/{BobEvent.Uid}", BobEvent)
                .Response(HttpStatusCode.NotFound, "user cannot update admin event");
        }

        [TestMethod]
        public async Task CreateUpdate_Superadmin()
        {
            BobEvent.Type = EventRecord.EventTypePersonal;
            var @event = await AlexSA.PostJsonAsync("events", BobEvent).AvendResponse<EventDto>();
            @event.Type.Should()
                .Be(EventRecord.EventTypeConference, "super admins create conference events");
            @event.Owner.Should().NotBeNull();

            BobEvent.Name = "Updated Bob Event";
            @event = await AlexSA.PutJsonAsync($"events/{BobEvent.Uid}", BobEvent).AvendResponse<EventDto>();
            @event.Type.Should()
                .Be(EventRecord.EventTypePersonal, "Event type not changed when event updated by super admin");
        }
    }
}
