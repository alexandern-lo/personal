using System.Net;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests
{
    [TestClass]
    public class UserProfileTest : IntegrationTest
    {
        [TestMethod]
        public async Task GetUserProfile()
        {
            var profile = await BobTA.GetJsonAsync("profile").AvendResponse<UserProfileDto>();
            profile.User.Uid.Should().Be(TestUser.BobTester.Uid);
            profile.User.Role.Should().Be(UserRole.Admin);
            profile.User.FirstName.Should().Be(TestUser.BobTester.FirstName);
            profile.User.LastName.Should().Be(TestUser.BobTester.LastName);
            profile.User.Email.Should().Be(TestUser.BobTester.Email);
            profile.Tenant.Uid.Should().Be(TestUser.BobTester.SubscriptionUid);
        }

        [TestMethod]
        public async Task GetUserProfileWithNoUser()
        {
            using (var browser = System.CreateClient("abc"))
            {
                await browser.GetJsonAsync("profile").Response(HttpStatusCode.Unauthorized);
            }
        }
    }
}