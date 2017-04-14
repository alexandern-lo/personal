using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avend.ApiTests.Infrastructure;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model;
using Avend.API.Services;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;
using SubscriptionMember = Avend.API.Model.SubscriptionMember;

namespace Avend.ApiTests.DataSamples
{
    public class SubscriptionMembers
    {
        public static async Task<SubscriptionMembers> Create(TestUser adminUser, TestSystem system)
        {
            var members = new SubscriptionMembers
            {
                User = adminUser,
                System = system
            };
            using (var services = system.GetServices())
            {
                var dbOptions = services.GetService<DbContextOptions<AvendDbContext>>();
                using (var member = new API.Services.Subscriptions.SubscriptionMember(dbOptions))
                {
                    member.Find(adminUser.Uid);
                    member.Validator.Throw();
                    members.Admin = member.Data;
                    members.Admin.Role = SubscriptionMemberRole.Admin;
                    await member.SaveChangesAsync();
                    members.Members.Add(members.Admin);
                }
            }

            return members;
        }

        private SubscriptionMembers()
        {
            Members = new List<SubscriptionMember>();
        }

        public List<SubscriptionMember> Members { get; set; }

        public SubscriptionMember Admin { get; set; }

        public TestSystem System { get; set; }

        public TestUser User { get; set; }

        public async Task<SubscriptionMember> Add(TestUser user,
            SubscriptionMemberStatus status = SubscriptionMemberStatus.Enabled,
            SubscriptionMemberRole role = SubscriptionMemberRole.User)
        {
            using (var services = System.GetServices())
            {
                var dbOptions = services.GetService<DbContextOptions<AvendDbContext>>();
                using (var member = new API.Services.Subscriptions.SubscriptionMember(dbOptions))
                {
                    member.CreateAndSubscribe(Admin.Subscription.Uid, user);
                    member.Data.Status = status;
                    member.Data.Role = role;
                    await member.SaveChangesAsync();
                    return member.Data;
                }
            }
        }

        public SubscriptionMember Find(TestUser user)
        {
            using (var services = System.GetServices())
            {
                var db = services.GetService<AvendDbContext>();
                return db.SubscriptionMembers.First(m => m.UserUid == user.Uid);
            }
        }

        public SubscriptionMember FindOrDefault(TestUser user)
        {
            using (var services = System.GetServices())
            {
                var db = services.GetService<AvendDbContext>();
                return db.SubscriptionMembers.FirstOrDefault(m => m.UserUid == user.Uid);
            }
        }
    }
}