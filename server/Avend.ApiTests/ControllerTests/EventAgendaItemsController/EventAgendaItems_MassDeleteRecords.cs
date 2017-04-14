using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Services.Events.NetworkDTO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventAgendaItemsController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("EventAgendaItemsController")]
    [TestCategory("EventAgendaItemsController.MassDeleteRecords()")]
    // ReSharper disable once InconsistentNaming
    public class EventAgendaItems_MassDeleteRecords : BaseAgendaItemsEndpointTest
    {
        protected Guid A1, A2;
        protected EventAgendaItemMassDeleteRequestDto DeleteRequest;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            A1 = await CreateAgendaItem();
            A2 = await CreateAgendaItem();
            DeleteRequest = new EventAgendaItemMassDeleteRequestDto
            {
                Uids = new List<Guid?> {A1, A2}
            };
        }

        [TestMethod]
        public async Task SuperAdminCanDelete()
        {
            var nrItems = await AlexSA.PostJsonAsync($"events/{EventUid}/agenda_items/delete", DeleteRequest)
                .AvendResponse<long?>();
            nrItems.Should().Be(2, "2 items removed");
        }

        [TestMethod]
        public async Task NormalUsersCannotMassDelete()
        {
            await BobTA.PostJsonAsync($"events/{EventUid}/agenda_items/delete", DeleteRequest)
                .Response(HttpStatusCode.Forbidden);
            await CecileSU.PostJsonAsync($"events/{EventUid}/agenda_items/delete", DeleteRequest)
                .Response(HttpStatusCode.Forbidden);
        }
    }
}