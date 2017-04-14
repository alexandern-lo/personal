using System;
using System.Net;
using System.Threading.Tasks;
using Avend.ApiTests.Infrastructure.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.Events
{
    [TestClass]
    public class EventDeleteTest : EventsControllerTestBase
    {
        protected async Task VerifyNoEvent(Guid? uid, string message = "Event deleted")
        {
            await AlexSA.GetJsonAsync($"events/{uid}")
                .Response(HttpStatusCode.NotFound, message);
        }

        protected async Task VerifyEventPresent(Guid? uid, string message = "Event present")
        {
            await AlexSA.GetJsonAsync($"events/{uid}")
                .Response(HttpStatusCode.OK, message);
        }

        [TestMethod]
        public async Task SuperAdminCanDelete()
        {
            await AlexSA.DeleteJsonAsync($"events/{BobEvent.Uid}")
                .Response(HttpStatusCode.OK, "Super admin can remove other people events");
            await VerifyNoEvent(BobEvent.Uid);
        }

        [TestMethod]
        public async Task UserCanDeleteOwnEvents()
        {
            await CecileSU.DeleteJsonAsync($"events/{CecilEvent.Uid}")
                .Response(HttpStatusCode.OK, "Users can remove their own events");
            await VerifyNoEvent(CecilEvent.Uid);
        }

        [TestMethod]
        public async Task UserCannotDeleteOthersEvents()
        {
            await CecileSU.DeleteJsonAsync($"events/{BobEvent.Uid}")
                .Response(HttpStatusCode.NotFound, "Users can not remove admin events");
            await VerifyEventPresent(BobEvent.Uid);
        }

        [TestMethod]
        public async Task NobodyCanDeleteSuperAdminEvents()
        {
            foreach (var user in new[] {BobTA, CecileSU})
            {
                await user.DeleteJsonAsync($"events/{AlexEvent.Uid}")
                    .Response(HttpStatusCode.NotFound, "Users cannot remove super admin event");
                await VerifyEventPresent(AlexEvent.Uid);
            }
        }

        [TestMethod]
        public async Task MassDelete()
        {
            await BobTA.PostJsonAsync($"events/delete", new[] {BobEvent.Uid, CecilEvent.Uid, AlexEvent.Uid})
                .Response(HttpStatusCode.OK, "User can mass delete events he has access too");
            await VerifyNoEvent(BobEvent.Uid);
            await VerifyNoEvent(CecilEvent.Uid);
            await VerifyEventPresent(AlexEvent.Uid, "Super admin events ignored in mass delete");
        }

        [TestMethod]
        public async Task SuperAdminMassDelete()
        {
            await AlexSA.PostJsonAsync($"events/delete", new[] {BobEvent.Uid, CecilEvent.Uid, AlexEvent.Uid})
                .Response(HttpStatusCode.OK, "User can mass delete events he has access too");
            await VerifyNoEvent(BobEvent.Uid);
            await VerifyNoEvent(CecilEvent.Uid);
            await VerifyNoEvent(AlexEvent.Uid);
        }
    }
}