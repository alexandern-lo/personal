using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Avend.API.Infrastructure;
using Avend.API.Infrastructure.Logging;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Qoden.Validation;

namespace Avend.API.Services.Subscriptions
{
    /// <summary>
    /// Provides operations on Subscription and Subscription members.
    /// </summary>
    public class SubscriptionMember : IDisposable
    {
        public const string Givenname = "given_name";
        public const string Surname = "family_name";
        public const string Emails = "emails";
        public const string Jobtitle = "job_title";
        public const string City = "city";
        public const string State = "state";

        private readonly DbContextOptions<AvendDbContext> _dbContextOptions;
        private readonly ILogger _logger;
        private SubscriptionRepository _repo;

        /// <summary>
        /// Subscriptions editor manages single subscription member subscription and related parameters. 
        /// Editor starts in invalid state since there is not valid Member. It transitions to valid state after call to Find or Create.
        /// </summary>
        /// <param name="dbContextOptions">context options</param>
        public SubscriptionMember(DbContextOptions<AvendDbContext> dbContextOptions)
        {
            Assert.Argument(dbContextOptions, nameof(dbContextOptions)).NotNull();

            _dbContextOptions = dbContextOptions;
            Dd = new AvendDbContext(_dbContextOptions);
            _logger = AvendLog.CreateLogger(nameof(SubscriptionMember));
            _repo = new SubscriptionRepository(Dd);
            Validator = new Validator();
        }

        public IValidator Validator { get; }
        public Model.SubscriptionMember Data { get; private set; }

        /// <summary>
        /// Find subscription member and populate Member property.
        /// </summary>
        /// <remarks>After call to this method Errors contains validation errors representing current Member state</remarks>
        /// <param name="userUid">user identifier</param>
        /// <returns>true if member found</returns>
        public bool Find(Guid userUid)
        {
            Data = _repo.FindMember(userUid);
            ValidateMember();
            return Data != null;
        }

        /// <summary>
        /// Creates new subscription member and updates Member property. If user owns subscription then she automatically subscribed as admin to it.
        /// </summary>
        /// <param name="userUid">user id</param>
        /// <remarks>After call to this method Errors contains validation errors representing current Member state</remarks>
        /// <exception cref="InvalidOperationException">Member not null</exception>
        public void Create(Guid userUid, List<Claim> claims)
        {
            Assert.Argument(userUid, nameof(userUid)).NotEqualsTo(Guid.Empty);
            _logger.LogDebug("Create subscription memeber for user {userUid}", userUid);
            Data = _repo.CreateMember(userUid);
            UpdateCachedMemberData(claims);
            if (Data.Role != SubscriptionMemberRole.SuperAdmin)
            {
                var subscription = _repo.FindSubscriptionByRecurlyAccount(userUid);
                if (subscription != null)
                {
                    Subscribe(subscription);
                }
            }
            ValidateMember();
        }

        /// <summary>
        /// Tries to find user subscription in Recurly and subscribe to it.
        /// </summary>
        /// <returns></returns>
        public async Task FetchSubscriptionFromRecurly()
        {
            Assert.State(Data).NotNull();

            var subscription = await _repo.Create(Data.UserUid);
            if (subscription != null)
            {
                subscription.Name = Data.FirstName + " " + Data.LastName;
                Subscribe(subscription);
            }
        }

        /// <summary>
        /// Updates user related data cached in subscription member for future display
        /// </summary>
        /// <param name="claims">User data</param>
        public void UpdateCachedMemberData(List<Claim> claims)
        {
            Assert.State(Data, nameof(Data)).NotNull();
            Data.FirstName = claims.FirstOrDefault(c => c.Type == Givenname)?.Value;
            Data.LastName = claims.FirstOrDefault(c => c.Type == Surname)?.Value;
            if (Data.FirstName == null && Data.LastName == null)
            {
                var name = claims.FirstOrDefault(c => c.Type == "name");
                if (name != null)
                {
                    var parts = name.Value.Split(' ');
                    if (parts.Length > 1)
                    {
                        Data.LastName = parts[parts.Length - 1];
                        Data.FirstName = String.Join(" ", parts, 0, parts.Length - 1);
                    }
                    else
                    {
                        Data.FirstName = name.Value;
                    }
                    
                }
            } 
            Data.Email = claims.FirstOrDefault(c => c.Type == Emails)?.Value;
            Data.JobTitle = claims.FirstOrDefault(c => c.Type == Jobtitle)?.Value;
            Data.City = claims.FirstOrDefault(c => c.Type == City)?.Value;
            Data.State = claims.FirstOrDefault(c => c.Type == State)?.Value;
            Data.Role = claims.Exists(c => c.Type == "extension_IsAdmin")
                ? SubscriptionMemberRole.SuperAdmin
                : Data.Role;
            _logger.LogDebug("UpdateCachedMemberData {@subscriptionMember}", Data);
            ValidateMember();
        }

        public void Subscribe(Guid subscriptionUid)
        {
            Assert.State(Data, nameof(Data)).NotNull();
            Assert.State(Data.Subscription, "Member.Subscription").IsNull();
            Subscribe(_repo.FindSubscriptionByUid(subscriptionUid));
        }

        private void Subscribe(SubscriptionRecord subscription)
        {
            _logger.LogDebug("Subscribe {userUid} {subscription}", Data.UserUid, subscription);
            Data.Subscription = subscription;
            Data.Status = SubscriptionMemberStatus.Enabled;
            Data.Role = Data.Subscription?.RecurlyAccountUid == Data.UserUid
                ? SubscriptionMemberRole.Admin
                : SubscriptionMemberRole.User;

            ValidateMember();
        }

        public bool NeedSubscriptionRefresh
            => Data.Subscription?.ExternalUid == null && Data.Role != SubscriptionMemberRole.SuperAdmin;

        public AvendDbContext Dd { get; private set; }

        private void ValidateMember()
        {
            Validator.CheckValue(Data, "member", AvendErrors.NotFound)
                .NotNull("user not found");

            if (Data != null)
            {
                Validator.CheckValue(Data.Subscription, "subscription", AvendErrors.Forbidden)
                    .NotNull("user does not have active subscription membership");

                if (Data.Subscription != null)
                {
                    var subscription = Data.Subscription;

                    Validator.CheckValue(Data.Status, "status", AvendErrors.Forbidden)
                        .EqualsTo(SubscriptionMemberStatus.Enabled, "membership disabled");

                    Validator.CheckValue(
                            subscription.ActiveUsersCount,
                            "subscription_members_count",
                            AvendErrors.Forbidden)
                        .LessOrEqualTo(subscription.MaximumUsersCount,
                            "Subscription has more members ({Value}) than allowed ({Max})");

                    Validator.CheckValue(subscription.ExpiresAt, "subscription_expires_at", AvendErrors.Forbidden)
                        .GreaterOrEqualTo(DateTime.Now, "Subscription expired");
                }
            }
        }

        public void Dispose()
        {
            Dd?.Dispose();
        }

        public Task SaveChangesAsync()
        {
            return Dd.SaveChangesAsync();
        }

        public void Reload()
        {
            _logger.LogDebug("Reload member {userUid}", Data.UserUid);
            Dd.Dispose();
            Dd = new AvendDbContext(_dbContextOptions);
            _repo = new SubscriptionRepository(Dd);
            Find(Data.UserUid);
        }

        public void AcceptInvite(string inviteId)
        {
            Assert.State(Data, "Data").NotNull();

            Check.Value(Data.Subscription)
                .IsNull("Cannot accept invite, user already has active subscription");

            var invite = _repo.FindInviteById(inviteId);
            if (invite != null)
            {
                Check.Value(invite.Accepted)
                    .IsFalse("Cannot accept same invite twice");

                Subscribe(invite.Subscription);
                invite.Accepted = true;
                invite.AcceptedAt = DateTime.Now;
            }
        }
    }
}