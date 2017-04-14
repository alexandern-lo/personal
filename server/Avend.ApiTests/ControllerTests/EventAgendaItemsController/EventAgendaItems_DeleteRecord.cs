using System;
using System.Net;
using System.Threading.Tasks;

using Avend.ApiTests.Infrastructure.Extensions;
using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventAgendaItemsController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("EventAgendaItemsController")]
    [TestCategory("EventAgendaItemsController.DeleteRecord()")]
    // ReSharper disable once InconsistentNaming
    public class EventAgendaItems_DeleteRecord : BaseAgendaItemsEndpointTest
    {
        protected Guid AgendaItemGuid;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            AgendaItemGuid = await CreateAgendaItem();
        }

        [TestMethod]
        public async Task NormalUsersCannotDelete()
        {
            await BobTA.DeleteJsonAsync($"events/{EventUid}/agenda_items/{AgendaItemGuid}")
                .Response(HttpStatusCode.Forbidden);
            await CecileSU.DeleteJsonAsync($"events/{EventUid}/agenda_items/{AgendaItemGuid}")
                .Response(HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task SuperAdminCanDelete()
        {
            var uid = await AlexSA.DeleteJsonAsync($"events/{EventUid}/agenda_items/{AgendaItemGuid}")
                .AvendResponse<Guid>();

            uid.Should().Be(AgendaItemGuid);
        }
    }
}