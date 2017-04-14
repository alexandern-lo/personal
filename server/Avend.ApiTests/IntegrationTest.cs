using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API;
using Avend.API.Infrastructure;
using Avend.API.Model;
using Avend.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubscriptionMember = Avend.API.Services.Subscriptions.SubscriptionMember;

namespace Avend.ApiTests
{
    public class IntegrationTest : IDisposable
    {
        protected readonly Guid BobSubscriptionUid, MarcSubscriptionUid;
        public TestSystem System => TestSystem.Instance;
        private readonly List<HttpClient> _browsers = new List<HttpClient>();

        protected HttpClient BobTA, AlexSA, MarcTA, CecileSU;

        public IntegrationTest()
        {
            //
            //  Below are real users from dev ad:
            //
            //  Alex is Super Admin
            AddUserToFakeAD(TestUser.AlexTester, new Claim("extension_IsAdmin", "Yes"));

            //  Alice does have subscription but does NOT have subscription member record 
            AddUserToFakeAD(TestUser.AliceTester);
            
            //  Bob is subscription admin and has valid subscription, 
            //  Cecile is a valid seat user in the same subscription as Bob
            AddUserToFakeAD(TestUser.BobTester);
            AddUserToFakeAD(TestUser.CecileTester);

            //  Marc is a subscription admin in a separate individual subscription 
            AddUserToFakeAD(TestUser.MarcTester);

            //These are virtual users, id generated and not present in AD
            AddUserToFakeAD(TestUser.NoSubscription);
            AddUserToFakeAD(TestUser.JohnTester);
            AddUserToFakeAD(TestUser.MikeTester);

            //
            //  End users initialization
            //

            using (var scope = System.GetServices())
            {
                var db = scope.GetService<AvendDbContext>();

                ClearAllDbTables(db);

                Startup.EnsureDbHasRequiredData(db);

                //  Now setup real subscription for users that should have it

                //  Bob and Cecile use the same subscription
                BobSubscriptionUid = CreateSubscriptionAndUser(scope, TestUser.BobTester, "recurly_corporate_monthly_1000", "3b20fdd8e32cc0409e287c4864aa6954");
                AddUserToSubscriptionWithRole(scope, TestUser.CecileTester, BobSubscriptionUid, SubscriptionMemberRole.User);

                //  Marc uses a separate subscription.
                MarcSubscriptionUid = CreateSubscriptionAndUser(scope, TestUser.MarcTester, "individual", "3b6d12a93f0c3cad8b897140fb89e18c");

                db.SaveChanges();

                var email = scope.GetService<ISendGrid>() as TestSendGrid;
                email.Messages.Clear();
                email.EmulateError = false;
            }

            BobTA = Browser(TestUser.BobTester);
            AlexSA = Browser(TestUser.AlexTester);
            MarcTA = Browser(TestUser.MarcTester);
            CecileSU = Browser(TestUser.CecileTester);
        }

        private static void ClearAllDbTables(AvendDbContext db)
        {
            var conn = db.Database.GetDbConnection();
            conn.Open();
            try
            {
                var disableConstraints = conn.CreateCommand();
                disableConstraints.CommandText = "EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'";
                disableConstraints.ExecuteNonQuery();

                var cleanupDatabase = conn.CreateCommand();
                cleanupDatabase.CommandText =
                    "EXEC sp_MSForEachTable 'SET QUOTED_IDENTIFIER ON; IF ''?'' != ''[dbo].[__EFMigrationsHistory]'' DELETE FROM ?' ";
                cleanupDatabase.ExecuteNonQuery();

                var enableConstraints = conn.CreateCommand();
                enableConstraints.CommandText =
                    "EXEC sp_MSForEachTable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL' ";
                enableConstraints.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        protected void AddUserToFakeAD(TestUser user, params Claim[] claims)
        {
            var issuer = string.Format("https://sts.windows.net/{0}/", System.TenantId);
            var defaultClaims = new[]
            {
                new Claim("oid", user.Uid.ToString()),
                new Claim(SubscriptionMember.Givenname, user.FirstName),
                new Claim(SubscriptionMember.Surname, user.LastName),
                new Claim(SubscriptionMember.Emails, user.Email),
                new Claim("iss", issuer)
            };
            System.FakeAD.AddUser(user.Token, claims.Concat(defaultClaims).ToArray());
        }

        [TestInitialize]
#pragma warning disable 1998
        public virtual async Task Init()
#pragma warning restore 1998
        {
        }

        protected static Guid CreateSubscriptionAndUser(IServiceScope services, TestUser user, string plan, string recurlyId)
        {
            var db = services.GetService<AvendDbContext>();
            var subscription = new SubscriptionRecord()
            {
                Uid = Guid.NewGuid(),
                Service = SubscriptionServiceType.Recurly,
                ActiveUsersCount = 1,
                ExpiresAt = DateTime.MaxValue,
                ExternalUid = recurlyId,
                MaximumUsersCount = 1000,
                Status = SubscriptionStatus.Active,
                Type = plan,
                AdditionalData = "",
                CreatedAt = DateTime.Now,
                RecurlyAccountUid = user.Uid,
                Name = user.FirstName + " " + user.LastName
            };
            db.SubscriptionsTable.Add(subscription);
            db.SaveChanges();

            AddUserToSubscriptionWithRole(services, user, subscription.Uid, SubscriptionMemberRole.Admin);

            return subscription.Uid;
        }

        protected static void AddUserToSubscriptionWithRole(IServiceScope services, TestUser user, Guid subscriptionUid, SubscriptionMemberRole role)
        {
            var dbOptions = services.GetService<DbContextOptions<AvendDbContext>>();
            using (var member = new SubscriptionMember(dbOptions))
            {
                member.CreateAndSubscribe(subscriptionUid, user);
                member.Data.Role = role;
                member.SaveChangesAsync().Wait();
                user.SubscriptionUid = subscriptionUid;
            }
        }

        public virtual void Dispose()
        {
            System.FakeAD.Clear();
            _browsers.ForEach(x => x.Dispose());
        }
        
        protected HttpClient Browser(TestUser user)
        {
            var browser = System.CreateClient(user.Token);
            _browsers.Add(browser);
            return browser;
        }
    }

    public static class SubscriptionEditorTestExtensions
    {
        public static void CreateAndSubscribe(this SubscriptionMember member, Guid subscriptionUid, TestUser user)
        {
            member.Create(user.Uid, new List<Claim>());
            if (member.Data.Subscription == null)
                member.Subscribe(subscriptionUid);
            member.Data.FirstName = user.FirstName;
            member.Data.LastName = user.LastName;
            member.Data.Email = user.Email;
        }
    }
}