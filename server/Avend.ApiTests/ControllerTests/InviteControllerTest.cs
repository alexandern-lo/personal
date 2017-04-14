using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Infrastructure;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Subscriptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests
{
    [TestClass]
    public class InviteControllerTest : IntegrationTest
    {
        private HttpClient _bobBrowser, _johnBrowser;
        private IServiceScope _services;
        private InviteRequestDto _qwe1Qwe2Invite;
        private string[] _qwe1Qwe2Emails;
        private InviteRequestDto _johnInviteRequest;
        private string _johnInviteEmail = "john@studiomobile.ru";
        private TestSendGrid _inviteMailer;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            _bobBrowser = System.CreateClient(TestUser.BobTester.Token);
            _johnBrowser = System.CreateClient(TestUser.JohnTester.Token);
            _services = System.GetServices();

            _qwe1Qwe2Emails = new[] { "qwe1@studiomobile.ru", "qwe2@mailinator.com" };
            _qwe1Qwe2Invite = new InviteRequestDto
            {
                Subject = "Avend Invite",
                Message = "Hello, please accept my invite to Avend.",
                Emails = _qwe1Qwe2Emails,
            };

            _johnInviteRequest = new InviteRequestDto
            {
                Subject = "Avend Invite",
                Message = "Hello, please accept my invite to Avend.",
                Emails = new[] { _johnInviteEmail },
            };

            _inviteMailer = _services.GetService<ISendGrid>() as TestSendGrid;
        }

        public override void Dispose()
        {
            _services?.Dispose();
            _bobBrowser?.Dispose();
            _johnBrowser?.Dispose();
            base.Dispose();
        }

        [TestMethod]
        public async Task InviteUser()
        {
            await _bobBrowser.PostJsonAsync("invite", _qwe1Qwe2Invite).Response();

            var members = await _bobBrowser.GetJsonAsync("users").AvendListResponse<SubscriptionMemberDto>();
            members.Count.Should().Be(4, "Subscription should now have Bob, Cecil, Qwe1 and Qwe2");
            
            //filter out Bob and Cecile from the list to leave only freshly invited users
            members = members.Where(x =>
                x.Uid != TestUser.BobTester.Uid
                && x.Uid != TestUser.CecileTester.Uid
                ).ToList();

            foreach (var member in members)
            {
                member.Status.Should()
                    .Be(SubscriptionMemberStatus.Invited, "because invited members should have 'Invited' status");
                member.Email.Should()
                    .BeOneOf(_qwe1Qwe2Emails, "because returned emails should be the same as in invites.");
            }
        }

        [TestMethod]
        public async Task AcceptInvite()
        {
            await _bobBrowser.PostJsonAsync("invite", _johnInviteRequest).Response();
            
            var johnInviteMessage = _inviteMailer.MessageFor(_johnInviteEmail);
            await _johnBrowser.PostJsonAsync("invite/accept/" + johnInviteMessage.InviteCode, "").Response();

            var members = await _bobBrowser.GetJsonAsync("users").AvendListResponse<SubscriptionMemberDto>();
            var johnMember = members.First(x => x.Uid == TestUser.JohnTester.Uid);
            johnMember.Role.Should()
                .Be(UserRole.SeatUser, "Fresh members receive 'User' role");
            johnMember.Status.Should()
                .Be(SubscriptionMemberStatus.Enabled, "Fresh members receive status 'Invited'");

            await _johnBrowser.PostJsonAsync("invite/accept/" + johnInviteMessage.InviteCode, "")
                .Response(HttpStatusCode.BadRequest, "Cannot accept same invite twice");
        }

        [TestMethod]
        public async Task DeleteInvite()
        {
            await _bobBrowser.PostJsonAsync("invite", _johnInviteRequest).Response();
            var members = await _bobBrowser.GetJsonAsync("users").AvendListResponse<SubscriptionMemberDto>();
            var johnMember = members.First(x => x.Email == _johnInviteEmail);
            await _bobBrowser.DeleteJsonAsync("invite/" + johnMember.Uid, "").Response();

            members = await _bobBrowser.GetJsonAsync("users").AvendListResponse<SubscriptionMemberDto>();
            members.Count.Should().Be(2, "Only Bob and Cecil left in the list");
        }

        [TestMethod]
        public async Task CannotInviteMoreThanAllowedBySubscription()
        {
            var db = _services.GetService<AvendDbContext>();
            var repo = new SubscriptionRepository(db);
            var subscription = repo.FindSubscriptionForUser(TestUser.BobTester.Uid);
            subscription.MaximumUsersCount = 2;
            await db.SaveChangesAsync();

            await _bobBrowser.PostJsonAsync("invite", _qwe1Qwe2Invite)
                .Response(HttpStatusCode.BadRequest, $"Cannot invite more than {subscription.MaximumUsersCount} users");
        }

        [TestMethod]
        public async Task OnlyAdminsCanInvite()
        {
            await _johnBrowser.PostJsonAsync("invite", _qwe1Qwe2Invite)
                .Response(HttpStatusCode.Forbidden, $"Only admins can invite users");
        }
    }
}