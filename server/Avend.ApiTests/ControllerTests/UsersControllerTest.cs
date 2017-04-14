using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avend.API.Model;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Subscriptions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests
{
    [TestClass]
    public class UsersControllerTest : IntegrationTest
    {
        private SubscriptionMembers _members;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            _members = await SubscriptionMembers.Create(TestUser.BobTester, System);
            await _members.Add(TestUser.AliceTester);
            await _members.Add(TestUser.MikeTester);
            await _members.Add(TestUser.JohnTester, SubscriptionMemberStatus.Disabled);
        }

        [TestMethod]
        public async Task GetUsers()
        {
            var testCases = new[]
            {
                new
                {
                    Browser = BobTA,
                    Users = new[] {"Alice", "Bob", "Cecile", "John", "Mike"},
                    Name = "Bob"
                },
                new
                {
                    Browser = AlexSA,
                    Users = new[] {"Marc", "Alice", "Bob", "Cecile", "John", "Mike", "Alex"},
                    Name = "Alex"
                },
            };

            foreach (var testCase in testCases)
            {
                var users = await testCase.Browser.GetJsonAsync("users")
                    .AvendListResponse<SubscriptionMemberDto>(
                        testCase.Users.Length,
                        HttpStatusCode.OK,
                        "Admin {0} can see {1} users",
                        testCase.Name, testCase.Users.Length);

                users.Select(u => u.FirstName).Should()
                    .Contain(testCase.Users, "all records returned in order of their creation");
            }
        }

        [TestMethod]
        public async Task GetUsers_FilterWithQuery()
        {
            var request = "users?q=Alice&sort_field=first_name&sort_order=asc";
            var users = await BobTA.GetJsonAsync(request)
                .AvendListResponse<SubscriptionMemberDto>(1);
            users.Select(u => u.FirstName).Should()
                .Contain(new[] {"Alice"}, "only Alice match query");
        }

        [TestMethod]
        public async Task NonAdminCannotGetUsers()
        {
            using (var aliceBrowser = System.CreateClient(TestUser.AliceTester.Token))
            {
                await aliceBrowser.GetJsonAsync("users")
                    .Response(HttpStatusCode.Forbidden, "Alice is not admin and cannot list users");
            }
        }

        [TestMethod]
        public async Task GetUsers_SortOrderAndPagination()
        {
            var users = await BobTA.GetJsonAsync("users?per_page=2&page=1&sort_field=first_name")
                .AvendListResponse<SubscriptionMemberDto>();
            users.Select(u => u.FirstName).Should()
                .Contain(new[] {"Cecile", "John"},
                    "because second page (with index=1) sorted by first_name in ASC order should return only Cecile and John");

            users = await BobTA.GetJsonAsync("users?per_page=2&page=1&sort_field=first_name&sort_order=desc")
                .AvendListResponse<SubscriptionMemberDto>();
            users.Select(u => u.FirstName).Should()
                .ContainInOrder(new[] {"Cecile", "Bob"},
                    "because second page (with page index=1) sorted by first_name in DESC order should return only Bob and Cecile in reverse order");
        }

        [TestMethod]
        public async Task GetUsers_FilterRole()
        {
            var users = await BobTA.GetJsonAsync("users?role=user").AvendListResponse<SubscriptionMemberDto>();

            users.Select(u => u.FirstName).Should()
                .Contain(new[] {"Alice", "John", "Mike"}, "these users have 'users' role");
        }

        [TestMethod]
        public async Task GetUsers_FilterStatus()
        {
            var users = await BobTA.GetJsonAsync("users?status=enabled")
                .AvendListResponse<SubscriptionMemberDto>();
            users.Select(u => u.FirstName).Should()
                .Contain(new[] {"Bob", "Alice", "Mike"}, "these 3 users enabled");
        }

        [TestMethod]
        public async Task GrantRevokeAdmin()
        {
            foreach (var admin in new[] {BobTA, AlexSA})
            {
                var response = await admin.PutJsonAsync("users/grant_admin", TestUser.JohnTester.Uid)
                    .Response();
                response.IsSuccessStatusCode.Should()
                    .BeTrue("admin can grant admin rights");
                _members.Find(TestUser.JohnTester).Role.Should()
                    .Be(SubscriptionMemberRole.Admin, "John should have admin role");

                response = await admin.PutJsonAsync("users/revoke_admin", TestUser.JohnTester.Uid)
                    .Response();
                response.IsSuccessStatusCode.Should()
                    .BeTrue("admin can revoke admin rights");
                _members.Find(TestUser.JohnTester).Role.Should()
                    .Be(SubscriptionMemberRole.User, "John should have user role");
            }
        }

        [TestMethod]
        public async Task CannotUpdateSuperAdmin()
        {
            await AlexSA.PutJsonAsync("users/grant_admin", TestUser.AlexTester.Uid)
                .Response(HttpStatusCode.Forbidden);
            await AlexSA.PutJsonAsync("users/revoke_admin", TestUser.AlexTester.Uid)
                .Response(HttpStatusCode.Forbidden);
            await AlexSA.PutJsonAsync("users/enable", TestUser.AlexTester.Uid)
                .Response(HttpStatusCode.Forbidden);
            await AlexSA.PutJsonAsync("users/disable", TestUser.AlexTester.Uid)
                .Response(HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task RevokeLastAdmin()
        {
            var testCases = new[]
            {
                new
                {
                    Browser = BobTA,
                    HttpCode = HttpStatusCode.Forbidden,
                    Message = "cannot revoke last admin rights",
                    Role = SubscriptionMemberRole.Admin
                },
                new
                {
                    Browser = AlexSA,
                    HttpCode = HttpStatusCode.OK,
                    Message = "super admin can revoke last admin rights",
                    Role = SubscriptionMemberRole.User
                }
            };

            foreach (var testCase in testCases)
            {
                await testCase.Browser.PutJsonAsync("users/revoke_admin", TestUser.BobTester.Uid)
                    .Response(testCase.HttpCode, testCase.Message);
                _members.Find(TestUser.BobTester).Role.Should()
                    .Be(testCase.Role);
            }
        }

        [TestMethod]
        public async Task OnlyAdminsCanGrantAdmin()
        {
            using (var mikeBrowser = System.CreateClient(TestUser.MikeTester.Token))
            {
                await mikeBrowser.PutJsonAsync("users/grant_admin", TestUser.JohnTester.Uid)
                    .Response(HttpStatusCode.Forbidden, "only admins can grant admin right");
                _members.Find(TestUser.JohnTester).Role.Should()
                    .Be(SubscriptionMemberRole.User);
            }
        }

        [TestMethod]
        public async Task EnableDisable()
        {
            using (var johnBrowser = System.CreateClient(TestUser.JohnTester.Token))
            {
                foreach (var admin in new[] {BobTA, AlexSA})
                {
                    await admin.PutJsonAsync("users/disable", TestUser.JohnTester.Uid)
                        .Response(HttpStatusCode.OK, "admin can disable user");

                    await johnBrowser.GetJsonAsync("profile")
                        .Response(HttpStatusCode.Forbidden, "disabled user cannot login");

                    await admin.PutJsonAsync("users/enable", TestUser.JohnTester.Uid)
                        .Response(HttpStatusCode.OK, "admin can enable user");

                    await johnBrowser.GetJsonAsync("profile")
                        .Response(HttpStatusCode.OK, "enabled user can login");
                }
            }
        }

        [TestMethod]
        public async Task DisableLastAdmin()
        {
            var testCases = new[]
            {
                new
                {
                    Browser = BobTA,
                    HttpCode = HttpStatusCode.Forbidden,
                    Message = "cannot disable last admin",
                    MemberStatus = SubscriptionMemberStatus.Enabled
                },
                new
                {
                    Browser = AlexSA,
                    HttpCode = HttpStatusCode.OK,
                    Message = "super admin can disable last admin",
                    MemberStatus = SubscriptionMemberStatus.Disabled
                },
            };

            foreach (var testCase in testCases)
            {
                await testCase.Browser.PutJsonAsync("users/disable", TestUser.BobTester.Uid)
                    .Response(testCase.HttpCode, testCase.Message);
                _members.Find(TestUser.BobTester).Status.Should()
                    .Be(testCase.MemberStatus);
            }
        }

        [TestMethod]
        public async Task OnlyAdminCanEnableDisable()
        {
            using (var mikeBrowser = System.CreateClient(TestUser.MikeTester.Token))
            {
                await mikeBrowser.PutJsonAsync("users/enable", TestUser.BobTester)
                    .Response(HttpStatusCode.Forbidden, "only admin can enable user");
                await mikeBrowser.PutJsonAsync("users/disable", TestUser.BobTester)
                    .Response(HttpStatusCode.Forbidden, "only admin can disable user");
            }
        }

        [TestMethod]
        public async Task GetProfile()
        {
            var profile = await BobTA.GetJsonAsync("profile").AvendResponse<UserProfileDto>();
            profile.User.Role.Should().Be(UserRole.Admin);
            profile.CurrentSubscription.Name.Should().Be("Bob Tester");

            using (var mikeBrowser = System.CreateClient(TestUser.MikeTester.Token))
            {
                profile = await mikeBrowser.GetJsonAsync("profile").AvendResponse<UserProfileDto>();
                profile.User.Role.Should().Be(UserRole.SeatUser);
            }

            profile = await AlexSA.GetJsonAsync("profile").AvendResponse<UserProfileDto>();
            profile.User.Role.Should().Be(UserRole.SuperAdmin);
        }
    }
}