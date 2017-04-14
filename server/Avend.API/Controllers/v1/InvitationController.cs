using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avend.API.Infrastructure;
using Avend.API.Services;
using Avend.API.Services.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qoden.Validation;

namespace Avend.API.Controllers.v1
{
    [Route("/api/v1/invite")]
    public class InvitationController : Controller
    {
        private readonly InviteService _inviteService;
        private readonly UserContext _userContext;

        public InvitationController(InviteService inviteService, UserContext userContext)
        {
            Assert.Argument(inviteService, nameof(inviteService)).NotNull();
            _inviteService = inviteService;
            _userContext = userContext;
        }

        [HttpPost]        
        [Authorize(Policy = Startup.SubscriptionAdminPolicy)]
        public async Task<List<string>> Invite([FromBody] InviteRequestDto inviteRequest)
        {
            var host = HttpContext.Request.Host;
            Func<string, string> linkGenerator = s => string.Format("https://{0}/invite/accept/{1}", host, s);
            return await _inviteService.Invite(User.AzureOid(), inviteRequest, linkGenerator);
        }

        [HttpPost]
        [Route("accept/{invite_code}")]
        public async Task AcceptInvite([FromRoute(Name = "invite_code")] string inviteCode)
        {
            var userUid = User.AzureOid();
            //ensure subscription member exists in DB before accepting invite
            await _userContext.LoadUser(userUid, User);
            await _inviteService.AcceptInvite(userUid, inviteCode);
        }

        [HttpDelete]
        [Route("{id}")]        
        [Authorize(Policy = Startup.SubscriptionAdminPolicy)]
        public async Task DeleteInvite([FromRoute(Name = "id")] Guid userUid)
        {
            await _inviteService.DeleteInvite(User.AzureOid(), userUid);
        }
    }
}