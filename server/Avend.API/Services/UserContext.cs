using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Avend.API.Helpers;
using Avend.API.Infrastructure;
using Avend.API.Infrastructure.Logging;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Subscriptions;
using Avend.API.Services.Subscriptions.NetworkDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Qoden.Validation;
using SubscriptionMember = Avend.API.Services.Subscriptions.SubscriptionMember;

namespace Avend.API.Services
{
    public interface IAvendPrincipal
    {
        long? UserId { get; }
        long? TenantId { get; }
        UserRole Role { get; }
    }

    public class AvendPrincipal : IAvendPrincipal
    {
        public AvendPrincipal(long? userId, long? tenantId, UserRole role)
        {
            UserId = userId;
            TenantId = tenantId;
            Role = role;
        }

        public long? UserId { get; }
        public long? TenantId { get; }
        public UserRole Role { get; }
    }

    public class UserContext : IAvendPrincipal
    {
        private readonly ILogger _logger;
        private readonly DbContextOptions<AvendDbContext> _dbContextOptions;

        public UserContext(DbContextOptions<AvendDbContext> dbContextOptions)
        {
            Assert.Argument(dbContextOptions, nameof(dbContextOptions)).NotNull();
            _dbContextOptions = dbContextOptions;
            _logger = AvendLog.CreateLogger(nameof(UserContext));
            _logger.LogDebug("Create user context");
            Role = UserRole.Anonymous;
            Errors = new Validator();
        }

        public async Task<IAvendPrincipal> GetSubordinate(Guid? userUid)
        {
            if (userUid == UserUid || userUid == null) return this;
            using (var db = new AvendDbContext(_dbContextOptions))
            {
                var r = new SubscriptionRepository(db);
                Model.SubscriptionMember user = null;
                switch (Role)
                {
                    case UserRole.SuperAdmin:
                        user = await r.FindMemberAsync(userUid.Value);
                        break;
                    case UserRole.Admin:
                        user = await r.FindMemberAsync(userUid.Value, Subscription?.Uid);
                        break;
                    default:
                        break;
                };
                return user != null ? new AvendPrincipal(user.Id, user.SubscriptionId, RoleHelper.FromSubscriptionRole(user.Role)) : null;
            }
        }

        public async Task LoadUser(Guid userUid, ClaimsPrincipal principal)
        {
            Assert.Argument(principal, nameof(principal)).NotNull();
            _logger.LogDebug("LoadUser {userUid}", userUid);

            Check.Value(userUid, onError: AvendErrors.Forbidden)
                .NotEqualsTo(Guid.Empty, "Invalid user id");

            using (var member = new SubscriptionMember(_dbContextOptions))
            {
                if (!member.Find(userUid))
                {
                    //Note 1 - about locking
                    //Since creating a member requires long calls to Recurly it is better
                    //to hold all other concurrent requests until this one is done.
                    //If statement above ensures it does not happen too often.
                    //Also this is bad idead to block thread which does async processing
                    //Better solution would be to pause this thread, do all calls and then resume it.
                    //(averbin)
                    //Note 2 - about two contexts
                    //SaveChanegs need to be called *before* leaving SqlAppLock section
                    //Unfortunately EF closes connection after SaveChange and SqlAppLock fails to unlock in Dispose.
                    //To avoid this I have to aquire another connection and lock user using it.
                    //(averbin)
                    using (var db = new AvendDbContext(_dbContextOptions))
                    using (new SqlAppLock(db.Database, userUid.ToString(), lockOwner: SqlAppLockOwner.Session))
                    {
                        if (member.Find(userUid))
                        {
                            member.Reload();
                        }
                        else
                        {
                            member.Create(userUid, principal.Claims.ToList());
                            if (member.NeedSubscriptionRefresh)
                            {
                                await member.FetchSubscriptionFromRecurly();
                            }
                            //save updated member to allow other locked request see updates
                            _logger.LogDebug(
                                "Subscription member created and saved {userUid} with subscription {subscriptionId}",
                                member.Data.UserUid, member.Data.Subscription?.Id);
                        }
                        await member.SaveChangesAsync();
                    }
                }
                member.UpdateCachedMemberData(principal.Claims.ToList());
                await member.SaveChangesAsync();

                //Init fields with defaults
                await LoadAnonymous();
                LoadUserInfo(userUid, member.Dd);
                Role = RoleHelper.FromSubscriptionRole(member.Data.Role);
                UserId = member.Data.Id;
                Errors = member.Validator;
                Member = SubscriptionMemberDto.From(member.Data);

                if (member.Data.Role == SubscriptionMemberRole.SuperAdmin)
                {
                    Subscription = new SubscriptionDto
                    {
                        Status = SubscriptionStatus.Active,
                        Service = SubscriptionServiceType.Free,
                        ExpiresAt = DateTime.MaxValue,
                        MaxUsers = int.MaxValue,
                        BillingPeriod = SubscriptionBillingPeriod.Yearly
                    };
                    Tenant = new TenantDto
                    {
                        Uid = null,
                        CompanyName = "Avend"
                    };
                    TenantId = null;
                }
                else if (member.Data.Subscription != null)
                {
                    Subscription = SubscriptionDto.From(member.Data.Subscription);                    
                    Tenant = TenantDto.From(member.Data.Subscription);
                    TenantId = member.Data.SubscriptionId;
                }
                else
                {
                    Subscription = null;
                    Tenant = null;
                    TenantId = null;
                }
            }
            IsLoaded = true;
        }

        public TimeZoneInfo TimeZone { get; private set; }

        public Task LoadAnonymous()
        {
            Role = UserRole.Anonymous;
            Errors = new Validator();
            UserUid = Guid.Empty;
            Subscription = null;
            Tenant = null;
            TenantId = null;
            TimeZone = TimeZoneInfo.Utc;
            IsLoaded = true;
            return Task.FromResult(0);
        }


        private UserRole _userRole;

        public UserRole Role
        {
            get
            {
                Assert.State(IsLoaded, "loaded").IsTrue();
                return _userRole;
            }
            private set { _userRole = value; }
        }

        private IValidator _errors;

        public IValidator Errors
        {
            get
            {
                Assert.State(IsLoaded, "loaded").IsTrue();
                return _errors;
            }
            private set { _errors = Assert.Property(value).NotNull().Value; }
        }

        private SubscriptionMemberDto _memberData;

        public SubscriptionMemberDto Member
        {
            get
            {
                Assert.State(IsLoaded, "loaded").IsTrue();
                return _memberData;
            }
            private set { _memberData = value; }
        }

        private SubscriptionDto _subscription;

        /// <summary>
        /// Current user susbcription. *Warning* - Super Admins does not have subscription and Subscription.Uid is null.
        /// </summary>
        public SubscriptionDto Subscription
        {
            get
            {
                Assert.State(IsLoaded, "loaded").IsTrue();
                return _subscription;
            }
            private set { _subscription = value; }
        }

        private TenantDto _tenant;
        /// <summary>
        /// Current user susbcription. *Warning* - Super Admins does not have subscription and Subscription.Uid is null.
        /// </summary>
        public TenantDto Tenant
        {
            get
            {
                Assert.State(IsLoaded, "loaded").IsTrue();
                return _tenant;
            }
            private set { _tenant = value; }
        }

        public bool IsLoaded { get; private set; }
        public Guid UserUid { get; private set; }
        public long? UserId { get; private set; }
        public long? TenantId { get; private set; }

        private void LoadUserInfo(Guid userUid, AvendDbContext db)
        {
            UserUid = userUid;
            TimeZone = TimeZoneInfo.Utc;
            var settings = db.Settings.FirstOrDefault(x => x.UserUid == userUid);
            if (settings?.TimeZone == null) return;
            try
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(settings.TimeZone);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot find timezone {timeZone}", settings.TimeZone);
            }
        }
    }
}