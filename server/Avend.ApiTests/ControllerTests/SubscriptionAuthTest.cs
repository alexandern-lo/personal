using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model;
using Avend.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests
{
    [TestClass]
    public class SubscriptionAuthTest : IntegrationTest
    {
        private HttpClient _aliceBrowser;
        private IServiceScope _services;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            _aliceBrowser = System.CreateClient(TestUser.AliceTester.Token);
            _services = System.GetServices();
        }

        public override void Dispose()
        {
            _aliceBrowser?.Dispose();
            _services?.Dispose();
            base.Dispose();
        }

        [TestMethod]
        public async Task SubscriptionAndAdminRecordsCreated()
        {
            await _aliceBrowser.GetJsonAsync("profile").Response();
            var db = _services.GetService<AvendDbContext>();
            var aliceMember = db.SubscriptionMembers
                .Include(x => x.Subscription)
                .First(x => x.UserUid == TestUser.AliceTester.Uid);

            aliceMember.Subscription.Should()
                .NotBeNull("subscription synchronized with Recurly");
            aliceMember.Role.Should()
                .Be(SubscriptionMemberRole.Admin, "user referenced in subscription became subscription admin");
            aliceMember.Status.Should()
                .Be(SubscriptionMemberStatus.Enabled, "user enabled");
        }

        [TestMethod]
        public async Task MemberNotCreatedSecondTime()
        {
            var db = _services.GetService<AvendDbContext>();
            var oldMembers = db.SubscriptionMembers.Count();
            var req1 = _aliceBrowser.GetJsonAsync("profile").Response();
            var req2 = _aliceBrowser.GetJsonAsync("profile").Response();
            await Task.WhenAll(req1, req2);
            var members = db.SubscriptionMembers.Count();
            (members - oldMembers).Should().Be(1, "subscription member created only once");
        }
    }
}