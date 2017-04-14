using System;
using System.Threading.Tasks;

using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventAgendaItemsController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("EventAgendaItemsController")]
    [TestCategory("EventAgendaItemsController.CreateRecord()")]
    // ReSharper disable once InconsistentNaming
    public class EventAgendaItems_CreateRecord : BaseAgendaItemsEndpointTest
    {
        [TestMethod]
        public async Task ShouldCreateAgendaItemWhenSuperAdminSendsRequestAndStartTimeAndEndTimeAreValid()
        {
            var agendaItemDto = new EventAgendaItemDTO()
            {
                EventUid = ConferenceEventData.Event.Uid,
                Name = "Sample conference meeting",

                Date = DateTime.UtcNow.Date,
                StartTime = TimeSpan.FromHours(8),
                EndTime = TimeSpan.FromHours(18),
            };

            var newAgendaItemGuid = await AlexSA.PostJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/agenda_items", agendaItemDto).AvendResponse<Guid>();

            newAgendaItemGuid.Should()
                .NotBeEmpty("because we have just added a single agenda item");

            var newAgendaItemDto = await AlexSA.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/agenda_items/" + newAgendaItemGuid).AvendResponse<EventAgendaItemDTO>();

            newAgendaItemDto.Should()
                .NotBeNull("because we have just added a single agenda item")
                /*
                                .And
                                .Match<EventAgendaItemDTO>(obj => obj.UserUid == TestUser.BobTester.Uid, "because we have used Bob account to add new lead")
                                .And
                                .Match<EventAgendaItemDTO>(obj => obj.SubscriptionUid == BobSubscriptionUid, "because we have used Bob account to add new lead")
                */
                .And
                .Match<EventAgendaItemDTO>(obj => obj.EventUid == agendaItemDto.EventUid, "because we have explicitly set event uid for the new agenda item")
                .And
                .Match<EventAgendaItemDTO>(obj => obj.StartTime == agendaItemDto.StartTime, "because we have explicitly set start time for the new agenda item")
                .And
                .Match<EventAgendaItemDTO>(obj => obj.EndTime == agendaItemDto.EndTime, "because we have explicitly set end time for the new agenda item")
                .And
                .Match<EventAgendaItemDTO>(obj => obj.Date == agendaItemDto.Date, "because we have explicitly set date for the new agenda item")
                .And
                .Match<EventAgendaItemDTO>(obj => obj.Name == agendaItemDto.Name, "because we have explicitly set the name value for the new agenda item")
                ;
        }
    }
}