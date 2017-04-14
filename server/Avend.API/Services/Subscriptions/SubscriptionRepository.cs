using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Avend.API.Infrastructure;
using Avend.API.Infrastructure.Logging;
using Avend.API.Infrastructure.SearchExtensions;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Microsoft.EntityFrameworkCore;
using Avend.API.Model;
using Microsoft.Extensions.Logging;
using Recurly.AspNetCore;
using Qoden.Validation;

namespace Avend.API.Services.Subscriptions
{
    public class SubscriptionRepository
    {
        private readonly ILogger _logger;

        public SubscriptionRepository(AvendDbContext db)
        {
            Assert.Argument(db, nameof(db)).NotNull();
            Db = db;
            _logger = AvendLog.CreateLogger(nameof(SubscriptionRepository));
        }

        public Expression<Func<SubscriptionMember, bool>> MemberScope { get; set; }

        public AvendDbContext Db { get; }

        public async Task<SubscriptionRecord> Create(Guid recurlyAccountUid)
        {
            _logger.LogDebug("Create subscription with recurly account id - {recurlyAccountUid}", recurlyAccountUid);
            //Create subscription and save it to DB, save fails if other subscription with same user id exists
            var subscription = new SubscriptionRecord
            {
                Uid = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                RecurlyAccountUid = recurlyAccountUid
            };
            if (await RefreshSubscriptionFromRecurly(subscription))
            {
                Db.SubscriptionsTable.Add(subscription);
                return subscription;
            }
            return null;
        }

        public SubscriptionRecord FindSubscriptionForUser(Guid userUid)
        {
            return Db.SubscriptionMembers
                .Include(x => x.Subscription)
                .Where(x => x.UserUid == userUid)
                .Select(x => x.Subscription)
                .FirstOrDefault();
        }

        /// <summary>
        /// Search for subscriptions.
        /// </summary>
        /// <param name="searchQueryParams">search parameters</param>
        /// <returns>Subscriptions satisfying search conditions and properly sorted</returns>
        public DefaultSearch<SubscriptionRecord> SearchSubscriptions(SearchQueryParams searchQueryParams)
        {
            searchQueryParams.ApplyDefaultSortOrder<SubscriptionRecord>();
            searchQueryParams.Validate().Throw();

            var dbSet = Db.SubscriptionsTable;

            var search = DefaultSearch.Start(searchQueryParams, dbSet);

            return search;
        }

        /// <summary>
        /// Search for subscription members
        /// </summary>
        /// <param name="searchQueryParams">search parameters</param>
        /// <param name="role">members must match this role</param>
        /// <param name="status">members must have this status</param>
        /// <param name="subscription">members must belong to give subscription</param>
        /// <returns>search result</returns>
        public DefaultSearch<Model.SubscriptionMember> Search(
            SearchQueryParams searchQueryParams,
            SubscriptionMemberRole? role,
            SubscriptionMemberStatus? status,
            Guid? subscription)
        {
            searchQueryParams.ApplyDefaultSortOrder<Model.SubscriptionMember>();
            searchQueryParams.Validate().Throw();

            var dbSet = Db.SubscriptionMembers
                .AsNoTracking()
                .FromSql("SELECT * FROM subscription_members_and_invites()");

            var search = DefaultSearch.Start(searchQueryParams, dbSet);
            search.Filter(collection =>
            {
                if (subscription != null)
                {
                    collection = collection.Where(x => x.Subscription.Uid == subscription);
                }
                if (status.HasValue)
                {
                    collection = collection.Where(x => x.Status == status.Value);
                }
                if (role.HasValue)
                {
                    collection = collection.Where(x => x.Role == role.Value);
                }
                return collection;
            });
            return search;
        }

        public SubscriptionRecord FindSubscriptionByRecurlyAccount(Guid userUid)
        {
            return Db.SubscriptionsTable
                .FirstOrDefault(x => x.RecurlyAccountUid == userUid);
        }

        /// <summary>
        /// Refresh subscription from Recurly
        /// </summary>
        /// <param name="subscription">Subscription to refresh</param>
        /// <returns></returns>
        public async Task<bool> RefreshSubscriptionFromRecurly(SubscriptionRecord subscription)
        {
            Assert.Argument(subscription, nameof(subscription)).NotNull();
            _logger.LogDebug("RefreshSubscriptionFromRecurly {subscription}", subscription);
            var recurlySubscription = await FetchRecurlySubscription(subscription);
            if (recurlySubscription == null)
            {
                _logger.LogDebug("Cannot find Recurly subscription for subscription {subscriptionUid}",
                    subscription.Uid);
                return false;
            }
            RefreshSubscriptionFromRecurly(subscription, recurlySubscription);
            _logger.LogDebug("Subscription refreshed from Recurly {recurlySubscription} {@subscription}",
                recurlySubscription, subscription);
            return true;
        }

        private async Task<Recurly.AspNetCore.Subscription> FetchRecurlySubscription(SubscriptionRecord subscription)
        {
            Account account;
            try
            {
                _logger.LogDebug("FetchRecurlySubscription {recurlyAccountUid}",
                    subscription.RecurlyAccountUid);
                account = await Accounts.Get(subscription.RecurlyAccountUid.ToString());
            }
            catch (NotFoundException e)
            {
                _logger.LogError(e,
                    "Cannot find Recurly account {RecurlyAccountUid} for local subscription {subscriptionUid}",
                    subscription.RecurlyAccountUid, subscription.Uid);
                return null;
            }
            _logger.LogDebug("Loading Recurly subscriptions {recurlyAccountUid}",
                subscription.RecurlyAccountUid);
            var subscriptions = account.GetSubscriptions(Recurly.AspNetCore.Subscription.SubscriptionState.Active);
            await subscriptions.RetrievalTask;
            var list = subscriptions.All;
            _logger.LogDebug("Found {Amount} subscriptions in {RecurlyAccountUid}",
                list.Count, subscription.RecurlyAccountUid);
            if (list.Count > 1)
            {
                _logger.LogWarning(
                    "Recurly has multiple active subscriptions in account {RecurlyAccountUid}",
                    subscription.RecurlyAccountUid);
            }

            return list.Count == 0 ? null : list[0];
        }

        private static void RefreshSubscriptionFromRecurly(SubscriptionRecord subscription,
            Recurly.AspNetCore.Subscription recurlySubscription)
        {
            subscription.Status = SubscriptionStatus.Active;
            subscription.Service = SubscriptionServiceType.Recurly;
            subscription.ExternalUid = recurlySubscription.Uuid;
            subscription.Type = recurlySubscription.PlanCode;
            subscription.ExpiresAt = recurlySubscription.ExpiresAt ??
                                     recurlySubscription.CurrentPeriodEndsAt ?? DateTime.MinValue;
            subscription.MaximumUsersCount = recurlySubscription.Quantity;
        }

        public Model.SubscriptionMember CreateMember(Guid userUid)
        {
            var member = new Model.SubscriptionMember
            {
                UserUid = userUid,
                Status = SubscriptionMemberStatus.Disabled,
                CreatedAt = DateTime.Now
            };
            Db.SubscriptionMembers.Add(member);
            return member;
        }

        public async Task<Model.SubscriptionMember> FindMemberAsync(Guid userUid, Guid? subscriptionUid = null)
        {
            var query = Db.SubscriptionMembers
                .Include(m => m.Subscription)
                .Where(m => m.UserUid == userUid);

            if (subscriptionUid.HasValue)
            {
                query = query.Where(x => x.Subscription.Uid == subscriptionUid);
            }
            return await query.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Find subscription member by userUid and optional subscription.
        /// </summary>
        /// <param name="userUid">user uid</param>
        /// <param name="subscriptionUid">susbcription uid</param>
        /// <returns>subscription member</returns>
        public Model.SubscriptionMember FindMember(Guid userUid, Guid? subscriptionUid = null)
        {
            return FindMemberAsync(userUid, subscriptionUid).Result;
        }

        public IEnumerable<Model.SubscriptionMember> SubscriptionAdmins(long? subscriptionid)
        {
            return Db.SubscriptionMembers
                .Where(s => s.SubscriptionId == subscriptionid && s.Role == SubscriptionMemberRole.Admin);
        }

        public SubscriptionRecord FindSubscriptionByUid(Guid subscriptionUid)
        {
            return Db.SubscriptionsTable
                .FirstOrDefault(x => x.Uid == subscriptionUid);
        }

        public SubscriptionInvite FindInviteByEmail(string email, bool? accepted = null)
        {
            Expression<Func<SubscriptionInvite, bool>> emailAndAcceptedFilter =
                x => x.Email == email && x.Accepted == accepted;
            Expression<Func<SubscriptionInvite, bool>> emailFilter = x => x.Email == email;
            var filter = accepted.HasValue ? emailAndAcceptedFilter : emailFilter;
            return Db.SubscriptionInvitesTable
                .Include(x => x.Subscription)
                .FirstOrDefault(filter);
        }

        public SubscriptionInvite FindInviteById(string inviteId)
        {
            return Db.SubscriptionInvitesTable
                .Include(x => x.Subscription)
                .FirstOrDefault(x => x.InviteCode == inviteId);
        }

        public SubscriptionInvite FindInviteByUId(Guid inviteId, Guid? subscriptionUid)
        {
            var query = Db.SubscriptionInvitesTable
                .Include(x => x.Subscription)
                .Where(x => x.Uid == inviteId);
            if (subscriptionUid.HasValue)
            {
                query = query.Where(x => x.Subscription.Uid == subscriptionUid);
            }
            return query.FirstOrDefault();
        }
    }
}
