using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avend.API.Controllers.v1;
using Avend.API.Infrastructure.Logging;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Subscriptions.NetworkDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Qoden.Validation;

namespace Avend.API.Services.Subscriptions
{
    public class SubscriptionsService
    {
        private readonly ILogger _logger;
        private readonly UserContext _userContext;

        private Dictionary<string, string> SubscriptionSortFieldMapping { get; } = new Dictionary<string, string>()
        {
            {"company_name", "name"}
        };

        public SubscriptionsService(DbContextOptions<AvendDbContext> dbOptions, UserContext userContext)
        {
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();
            Assert.Argument(userContext, nameof(userContext)).NotNull();
            DbContextOptions = dbOptions;
            _logger = AvendLog.CreateLogger(nameof(SubscriptionsService));
            _userContext = userContext;
        }

        public DbContextOptions<AvendDbContext> DbContextOptions { get; }

        private SubscriptionRepository CreateRepo(AvendDbContext db)
        {
            return new SubscriptionRepository(db);
        }

        public SearchResult<TenantDto> FindSubscriptions(SearchQueryParams searchQueryParams)
        {
            Assert.Argument(searchQueryParams, nameof(searchQueryParams)).NotNull();
            Assert.Argument(SubscriptionSortFieldMapping.ContainsKey(searchQueryParams.SortField), nameof(searchQueryParams)).IsTrue("sort_field is not valid");

            searchQueryParams.SortField = SubscriptionSortFieldMapping[searchQueryParams.SortField];

            using (var db = new AvendDbContext(DbContextOptions))
            {
                var repo = CreateRepo(db);

                var search = repo.SearchSubscriptions(searchQueryParams);

                return search.Paginate(TenantDto.From);
            }
        }

        public SearchResult<SubscriptionMemberDto> FindUsers(SearchQueryParams searchQueryParams,
            SubscriptionMemberRole? role, SubscriptionMemberStatus? status, Guid? tenantUid)
        {
            Assert.Argument(searchQueryParams, nameof(searchQueryParams)).NotNull();

            using (var db = new AvendDbContext(DbContextOptions))
            {
                var repo = CreateRepo(db);
                switch (_userContext.Role)
                {
                    case UserRole.Admin:
                        tenantUid = repo.FindSubscriptionForUser(_userContext.UserUid).Uid;
                        break;
                    case UserRole.SuperAdmin:
                        break;
                    default:
                        Check.Value(false, onError: AvendErrors.Forbidden)
                            .IsTrue();
                        break;
                }
                var search = repo.Search(searchQueryParams, role, status, tenantUid);
                return search.Paginate(SubscriptionMemberDto.From)
                    .WithFilter("tenant", tenantUid)
                    .WithFilter("role", role)
                    .WithFilter("status", status);
            }
        }

        public SubscriptionDto FindSubscriptionForUser()
        {
            if (_userContext.Role == UserRole.SuperAdmin)
            {
                return _userContext.Subscription;
            }

            using (var db = new AvendDbContext(DbContextOptions))
            {
                var repo = CreateRepo(db);
                var subscription = repo.FindSubscriptionForUser(_userContext.UserUid);
                return subscription != null ? SubscriptionDto.From(subscription) : null;
            }
        }

        /// <summary>
        /// Updates subscription member admin status. Method does not revoke admin rights from last admin.
        /// </summary>
        /// <param name="userUid">user uid to grant/revoke admin rights</param>
        /// <param name="isAdmin">admin status</param>
        /// <returns>true if status updated</returns>
        public async Task<SubscriptionMemberDto> UpdateMemberRole(Guid userUid, bool isAdmin)
        {
            _logger.LogDebug("UpdateMemberRole {userUid} {admin}", userUid, isAdmin);
            using (var db = new AvendDbContext(DbContextOptions))
            {
                var admin = new SubscriptionAdmin(_userContext, CreateRepo(db));
                var member = admin.UpdateRole(userUid, isAdmin);
                await db.SaveChangesAsync();
                return SubscriptionMemberDto.From(member);
            }
        }

        /// <summary>
        /// Subscription member status 
        /// </summary>
        /// <param name="userUid">user id</param>
        /// <param name="status">new status</param>
        /// <returns>true if update successfull</returns>
        public async Task<SubscriptionMemberDto> UpdateMemberStatus(Guid userUid, bool status)
        {
            _logger.LogDebug("UpdateMemberStatus {userUid} {status}", userUid, status);
            using (var db = new AvendDbContext(DbContextOptions))
            {
                var admin = new SubscriptionAdmin(_userContext, CreateRepo(db));
                var member = admin.UpdateStatus(userUid, status);
                await db.SaveChangesAsync();
                return SubscriptionMemberDto.From(member);
            }
        }
    }
}