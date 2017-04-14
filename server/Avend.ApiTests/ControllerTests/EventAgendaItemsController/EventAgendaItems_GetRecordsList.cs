using System;
using System.Collections.Generic;
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
    [TestCategory("EventAgendaItemsController.GetRecordsList()")]
    // ReSharper disable once InconsistentNaming
    public class EventAgendaItems_GetRecordsList : BaseAgendaItemsEndpointTest
    {
        [TestMethod]
        public async Task ShouldReturnListWithBothMatchingAgendaItemsWhenSuperAdminSendsRequestAndStartTimeAndEndTimeAreValid()
        {
            var agendaItemDto = new EventAgendaItemDTO()
            {
                EventUid = ConferenceEventData.Event.Uid,
                Name = "Sample conference meeting",

                Date = DateTime.UtcNow.Date,
                StartTime = TimeSpan.FromHours(8),
                EndTime = TimeSpan.FromHours(18),
            };

            var newAgendaItemGuid1 = await AlexSA.PostJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/agenda_items", agendaItemDto).AvendResponse<Guid>();

            newAgendaItemGuid1.Should()
                .NotBeEmpty("because we have just added a valid agenda item");

            var newAgendaItemGuid2 = await AlexSA.PostJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/agenda_items", agendaItemDto).AvendResponse<Guid>();

            newAgendaItemGuid2.Should()
                .NotBeEmpty("because we have just added a valid agenda item");

            var newAgendaItems = await AlexSA.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/agenda_items").AvendResponse<List<EventAgendaItemDTO>>();

            newAgendaItems.Should()
                .NotBeNull("because a valid list of agenda items is expected")
                .And
                .HaveCount(2, "because we added exactly two agenda items")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnListWithSingleAgendaItemWhenSuperAdminSendsRequestAndDateFilterLimitsResults()
        {
            var agendaItemDto1 = new EventAgendaItemDTO()
            {
                EventUid = ConferenceEventData.Event.Uid,
                Name = "Sample conference meeting",

                Date = DateTime.UtcNow.Date,
                StartTime = TimeSpan.FromHours(8),
                EndTime = TimeSpan.FromHours(18),
            };

            var agendaItemDto2 = new EventAgendaItemDTO()
            {
                EventUid = ConferenceEventData.Event.Uid,
                Name = "Sample conference meeting",

                Date = DateTime.UtcNow.Date.AddDays(-1),
                StartTime = TimeSpan.FromHours(8),
                EndTime = TimeSpan.FromHours(18),
            };

            var newAgendaItemGuid1 = await AlexSA.PostJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/agenda_items", agendaItemDto1).AvendResponse<Guid>();

            newAgendaItemGuid1.Should()
                .NotBeEmpty("because we have just added a valid agenda item");

            var newAgendaItemGuid2 = await AlexSA.PostJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/agenda_items", agendaItemDto2).AvendResponse<Guid>();

            newAgendaItemGuid2.Should()
                .NotBeEmpty("because we have just added a valid agenda item");

            var newAgendaItems = await AlexSA.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/agenda_items?date=" + DateTime.UtcNow.Date).AvendResponse<List<EventAgendaItemDTO>>();

            newAgendaItems.Should()
                .NotBeNull("because a valid list of agenda items is expected")
                .And
                .HaveCount(1, "because only single agenda item is added for today")
                ;
        }
    }
}