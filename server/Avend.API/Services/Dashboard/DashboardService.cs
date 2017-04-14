using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avend.API.Infrastructure.Logging;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Dashboard.NetworkDTO;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Qoden.Validation;

namespace Avend.API.Services.Dashboard
{
    public class DashboardService
    {
        private readonly ILogger logger;

        public UserContext UserContext { get; }
        public DbContextOptions<AvendDbContext> DbContextOptions { get; }

        /// <summary>
        /// Service constructor ready for dependency injection.
        /// </summary>
        /// 
        /// <param name="userContext">User context containing user & subscrption data for current user</param>
        /// <param name="dbOptions">Configuration for connection to database</param>
        public DashboardService(
            UserContext userContext,
            DbContextOptions<AvendDbContext> dbOptions
        )
        {
            Assert.Argument(userContext, nameof(userContext)).NotNull();
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();

            UserContext = userContext;
            DbContextOptions = dbOptions;

            logger = AvendLog.CreateLogger(nameof(DashboardService));
        }

        public DashboardDTO ProduceDashboardForUser(
            int eventsCount, string eventsSortField, string eventsSortOrder,
            int resourcesCount, string resourcesSortField, string resourcesSortOrder
            )
        {
            Check.Value(UserContext, nameof(UserContext)).NotNull();
            Check.Value(UserContext.Role, nameof(UserContext), AvendErrors.Forbidden).NotEqualsTo(UserRole.SuperAdmin, "User's dashboard is not available for superadmins");

            using (var db = new AvendDbContext(DbContextOptions))
            {
                var dashboardReader = new DashboardReader(UserContext, db);

                var resources = dashboardReader.ConstructDashboardResourceDtos(UserContext.UserUid, resourcesCount, resourcesSortField, resourcesSortOrder);

                List<DashboardEventDTO> events;
                if (!string.IsNullOrWhiteSpace(eventsSortField))
                    events = dashboardReader.ConstructSortedDashboardEventDtos(UserContext.UserUid, eventsCount, eventsSortField, eventsSortOrder);
                else
                    events = dashboardReader.ConstructMobileStyledDashboardEventDtos(UserContext.UserUid, eventsCount);

                var leadsStats = dashboardReader.GetLeadsStatisticsForUser(UserContext.UserUid);

                var responseObj = new DashboardDTO()
                {
                    Events = events,
                    Resources = resources.ToList(),

                    LeadsStatistics = leadsStats,

                    CreatedAt = DateTime.UtcNow,
                };

                return responseObj;
            }
        }

        /// <summary>
        /// Returns enumerable with daily history leads count data for the user with given Uid.
        /// </summary>
        /// 
        /// <param name="count">Number of days in the past to get data for</param>
        /// 
        /// <returns>Enumerable of DateDecimalTupleDTO holding the leads count data in ascending order</returns>
        public IEnumerable<DateIndexedTupleDto<decimal>> ProduceLeadsCountDailyHistoryForUser(int count)
        {
            Check.Value(UserContext, nameof(UserContext)).NotNull();
            Check.Value(UserContext.UserUid, nameof(UserContext), AvendErrors.Forbidden).NotNull();
            Check.Value(count, nameof(count)).GreaterOrEqualTo(1);

            using (var db = new AvendDbContext(DbContextOptions))
            {
                var dashboardReader = new DashboardReader(UserContext, db);

                return dashboardReader.ProduceLeadsCountDailyHistoryForUser(UserContext.UserUid, count);
            }
        }

        /// <summary>
        /// Returns summary for each user's expenses filtered by event uids for the given tenant.
        /// </summary>
        /// 
        /// <param name="eventsFilter">Events filter object containing event uids</param>
        /// <param name="userContext">Member profile needed to provide data on current subscription</param>
        /// 
        /// <returns>List of constructed statistical records, throws exception on any error</returns>
        public Task<UserTotalExpensesListDto> GetUserExpensesFilteredByEvents(FilterByEventsRequestDTO eventsFilter)
        {
            logger.LogDebug("GetUserExpensesFilteredByEvents {tenantUid} {eventUids}", UserContext?.Subscription?.Uid, eventsFilter);

            Check.Value(UserContext, nameof(UserContext)).NotNull();
            Check.Value(UserContext.UserUid, nameof(UserContext), AvendErrors.Forbidden).NotNull();
            Check.Value(UserContext.Role, nameof(UserContext), AvendErrors.Forbidden).EqualsTo(UserRole.Admin, "Only tenant admin can access those data");
            Check.Value(UserContext.Subscription, nameof(UserContext) + "." + nameof(UserContext.Subscription)).NotNull();
            Check.Value(UserContext.Subscription.Uid,
                nameof(UserContext) + "." + nameof(UserContext.Subscription) + "." +
                nameof(UserContext.Subscription.Uid)).NotNull();

            var allTimeExpenses = new MoneyDto();

            var result = new Dictionary<Guid, UserTotalExpensesDto>();

            using (var db = new AvendDbContext(DbContextOptions))
            {
                List<long> eventIds = null;

                if (eventsFilter.EventUids != null)
                    eventIds = db.EventsTable
                        .Where(record => eventsFilter.EventUids.Contains(record.Uid)).Select(record => record.Id).ToList();

                var userExpensesQuery = db.EventUserExpensesTable
                    .Where(record => record.TenantUid == UserContext.Subscription.Uid.Value);

                if (eventsFilter.EventUids != null)
                    userExpensesQuery = userExpensesQuery
                        .Where(record => eventIds.Contains(record.EventId));

                var firstUserExpense = userExpensesQuery.FirstOrDefault();

                allTimeExpenses.Currency = firstUserExpense?.Currency ?? CurrencyCode.USD;
                allTimeExpenses.Amount = firstUserExpense != null ? userExpensesQuery.Sum(record => record.Amount) : 0;

                var leadCountsQuery = GetLeadsCountQuery(UserContext.Subscription.Uid.Value, db.LeadsTable);
                leadCountsQuery = leadCountsQuery.OrderByDescending(record => record.LeadsAcquired);

                if (eventsFilter.Limit.HasValue && eventsFilter.Limit.Value > 0)
                {
                    leadCountsQuery = leadCountsQuery.Take(eventsFilter.Limit.Value);
                }

                List<Guid> userUids = new List<Guid>();

                foreach (var leadCount in leadCountsQuery)
                {
                    if (!leadCount.UserUid.HasValue)
                        continue;

                    var userUid = leadCount.UserUid.Value;

                    var expensesDto = new UserTotalExpensesDto()
                    {
                        UserUid = userUid,
                        Amount = new MoneyDto()
                        {
                            Amount = 0,
                            Currency = CurrencyCode.USD,
                        },
                    };

                    result[userUid] = expensesDto;
                    userUids.Add(userUid);
                }

                var usersWithLeads = db.SubscriptionMembers
                    .Where(recUser => userUids.Contains(recUser.UserUid))
                    .OrderBy(record => record.FirstName + record.LastName);

                foreach (var memberData in usersWithLeads)
                {
                    var userUid = memberData.UserUid;

                    if (!result.ContainsKey(userUid))
                        continue;

                    if (memberData.Status != SubscriptionMemberStatus.Enabled)
                    {
                        result.Remove(userUid);

                        continue;
                    }

                    result[userUid].FirstName = memberData.FirstName;
                    result[userUid].LastName = memberData.LastName;
                }

                if (!eventsFilter.Limit.HasValue || eventsFilter.Limit.Value <= 0 || eventsFilter.Limit.Value > result.Count)
                {
                    IQueryable<SubscriptionMember> usersNoLeads = db.SubscriptionMembers.Include(user => user.Subscription)
                        .Where(recUser => recUser.Subscription.Uid == UserContext.Subscription.Uid)
                        .Where(recUser => !userUids.Contains(recUser.UserUid))
                        .Where(recUser => recUser.Status == SubscriptionMemberStatus.Enabled)
                        .OrderBy(record => record.FirstName + record.LastName);

                    if (eventsFilter.Limit.HasValue && eventsFilter.Limit.Value > result.Count)
                    {
                        usersNoLeads = usersNoLeads.Take(eventsFilter.Limit.Value - result.Count);
                    }

                    foreach (var memberData in usersNoLeads)
                    {
                        var userUid = memberData.UserUid;

                        var expensesDto = new UserTotalExpensesDto()
                        {
                            UserUid = memberData.UserUid,
                            Amount = new MoneyDto()
                            {
                                Amount = 0,
                                Currency = CurrencyCode.USD,
                            },
                            FirstName = memberData.FirstName,
                            LastName = memberData.LastName,
                        };

                        result[userUid] = expensesDto;
                        userUids.Add(userUid);
                    }
                }

                var eventUserExpenses = db.EventUserExpensesTable
                        .Where(recExpense => recExpense.TenantUid == UserContext.Subscription.Uid)
                        .Where(recExpense => recExpense.UserUid.HasValue && userUids.Contains(recExpense.UserUid.Value))
                    ;

                if (eventsFilter?.EventUids != null)
                    eventUserExpenses = eventUserExpenses.Where(recExpense => eventIds.Contains(recExpense.EventId));

                var totalExpensesQuery = eventUserExpenses
                    .GroupBy(recExpense => recExpense.UserUid)
                    .Select(recAggregated => new
                    {
                        UserUid = recAggregated.Key,
                        TotalAmount = recAggregated.Sum(record => record.Amount),
                        Currency = recAggregated.Select(record => record.Currency).FirstOrDefault(),
                    });

                foreach (var expensesData in totalExpensesQuery)
                {
                    if (!expensesData.UserUid.HasValue)
                        continue;

                    var userUid = expensesData.UserUid.Value;

                    if (!result.ContainsKey(userUid))
                        continue;

                    result[userUid].Amount = new MoneyDto()
                    {
                        Amount = expensesData.TotalAmount,
                        Currency = expensesData.Currency,
                    };
                }
            }

            return Task.FromResult(new UserTotalExpensesListDto()
            {
                EventUids = eventsFilter.EventUids?.Where(record => record.HasValue).Select(record => record.Value),
                TotalExpenses = allTimeExpenses,
                UserExpenses = result.Values,
            });
        }

        /// <summary>
        /// Returns summary for each user's leads goal filtered by event uids for the given tenant.
        /// </summary>
        /// 
        /// <param name="eventsFilter">Events filter object containing event uids</param>
        /// <param name="userContext">Member profile needed to provide data on current subscription</param>
        /// <param name="count">Maximum number of records to return</param>
        /// 
        /// <returns>List of constructed statistical records, throws exception on any error</returns>
        public Task<IEnumerable<UserTotalLeadsGoalDto>> GetUserGoalsFilteredByEvents(FilterByEventsRequestDTO eventsFilter)
        {
            logger.LogDebug("GetUserGoalsFilteredByEvents {tenantUid} {eventUids}", UserContext?.Subscription?.Uid, eventsFilter);

            Check.Value(UserContext, nameof(UserContext)).NotNull();
            Check.Value(UserContext.UserUid, nameof(UserContext), AvendErrors.Forbidden).NotNull();
            Check.Value(UserContext.Role, nameof(UserContext), AvendErrors.Forbidden).EqualsTo(UserRole.Admin, "Only tenant admin can access those data");
            Check.Value(UserContext.Subscription,
                nameof(UserContext) + "." + nameof(UserContext.Subscription)).NotNull();
            Check.Value(UserContext.Subscription.Uid,
                nameof(UserContext) + "." + nameof(UserContext.Subscription) + "." +
                nameof(UserContext.Subscription.Uid)).NotNull();

            var result = new Dictionary<Guid, UserTotalLeadsGoalDto>();

            using (var db = new AvendDbContext(DbContextOptions))
            {
                var leadCountsQuery = GetLeadsCountQuery(UserContext.Subscription.Uid.Value, db.LeadsTable);

                leadCountsQuery = leadCountsQuery.OrderByDescending(record => record.LeadsAcquired);

                if (eventsFilter.Limit.HasValue && eventsFilter.Limit.Value > 0)
                {
                    leadCountsQuery = leadCountsQuery.Take(eventsFilter.Limit.Value);
                }

                List<Guid> userUids = new List<Guid>();

                foreach (var leadCount in leadCountsQuery)
                {
                    if (!leadCount.UserUid.HasValue)
                        continue;

                    var userUid = leadCount.UserUid.Value;

                    var goalsDto = new UserTotalLeadsGoalDto()
                    {
                        UserUid = userUid,
                        LeadsCount = leadCount.LeadsAcquired,
                        LeadsGoal = leadCount.LeadsGoal,
                    };

                    result[userUid] = goalsDto;

                    userUids.Add(userUid);
                }

                var usersWithLeads = db.SubscriptionMembers.Include(user => user.Subscription)
                    .Where(recUser => recUser.Subscription.Uid == UserContext.Subscription.Uid)
                    .Where(recUser => userUids.Contains(recUser.UserUid));

                foreach (var memberData in usersWithLeads)
                {
                    var userUid = memberData.UserUid;

                    if (memberData.Status != SubscriptionMemberStatus.Enabled)
                    {
                        result.Remove(userUid);

                        continue;
                    }

                    result[userUid].FirstName = memberData.FirstName;
                    result[userUid].LastName = memberData.LastName;
                }

                if (!eventsFilter.Limit.HasValue || eventsFilter.Limit.Value <= 0 ||
                    eventsFilter.Limit.Value > result.Count)
                {
                    IQueryable<SubscriptionMember> usersNoLeads = db.SubscriptionMembers.Include(user => user.Subscription)
                        .Where(recUser => recUser.Subscription.Uid == UserContext.Subscription.Uid)
                        .Where(recUser => !userUids.Contains(recUser.UserUid))
                        .Where(recUser => recUser.Status == SubscriptionMemberStatus.Enabled)
                        .OrderBy(record => record.FirstName + record.LastName);

                    if (eventsFilter.Limit.HasValue && eventsFilter.Limit.Value > result.Count)
                    {
                        usersNoLeads = usersNoLeads.Take(eventsFilter.Limit.Value - result.Count);
                    }

                    foreach (var memberData in usersNoLeads)
                    {
                        var userUid = memberData.UserUid;

                        var goalDto = new UserTotalLeadsGoalDto()
                        {
                            UserUid = memberData.UserUid,

                            FirstName = memberData.FirstName,
                            LastName = memberData.LastName,

                            LeadsCount = 0,
                            LeadsGoal = 0,
                        };

                        result[userUid] = goalDto;
                    }
                }
                var eventUserGoals = db.EventUserGoalsTable
                    .Where(recGoal => recGoal.TenantUid == UserContext.Subscription.Uid);

                if (eventsFilter?.EventUids != null)
                    eventUserGoals = eventUserGoals.Where(recExpense => eventsFilter.EventUids.Contains(recExpense.Event.Uid));

                var totalGoalsQuery = eventUserGoals
                    .GroupBy(regGoal => regGoal.UserUid)
                    .Select(recAggregated => new UserLeadsStats
                    {
                        UserUid = recAggregated.Key,
                        LeadsAcquired = recAggregated.Sum(record => record.LeadsAcquired),
                        LeadsGoal = recAggregated.Sum(record => record.LeadsGoal),
                    });

                foreach (var goalData in totalGoalsQuery)
                {
                    if (!goalData.UserUid.HasValue)
                        continue;

                    var userUid = goalData.UserUid.Value;

                    if (!result.ContainsKey(userUid))
                        continue;

                    result[userUid].LeadsCount = Math.Max(result[userUid].LeadsCount, goalData.LeadsAcquired);
                    result[userUid].LeadsGoal = goalData.LeadsGoal;
                }
            }

            var usersSorted = result.Values.OrderByDescending(record => record.LeadsCount).ThenBy(record => record.FirstName + record.LastName);

            return Task.FromResult((IEnumerable<UserTotalLeadsGoalDto>)usersSorted);
        }

        private IQueryable<UserLeadsStats> GetLeadsCountQuery(Guid tenantUid, IQueryable<LeadRecord> leadsTable)
        {
            leadsTable = leadsTable
                .Where(recLead => recLead.Subscription.Uid == tenantUid)
                .NotDeleted();

            var leadCountsByUser = leadsTable
                .GroupBy(recLead => recLead.UserUid)
                .Select(recAggregated => new UserLeadsStats
                {
                    UserUid = recAggregated.Key,
                    LeadsAcquired = recAggregated.Count(),
                });

            return leadCountsByUser;
        }
    }

    public class SubscriptionMemberWithLeadsCount : SubscriptionMember
    {
        public int LeadsCount { get; set; }
    }

    public class UserLeadsStats
    {
        public Guid? UserUid { get; set; }

        public int LeadsAcquired { get; set; }
        public int LeadsGoal { get; set; }
    }
}