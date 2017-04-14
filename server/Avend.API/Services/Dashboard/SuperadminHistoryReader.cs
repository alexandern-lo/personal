using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Avend.API.Infrastructure.Logging;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Qoden.Validation;

namespace Avend.API.Services.Dashboard
{
    /// <summary>
    /// Helper class to construct different history data for superadmins' dashboard.
    /// </summary>
    public class SuperadminHistoryReader
    {
        public static string[] AllowedSubscriptionTypes = { "trial", "paid" };
        public static string[] AllowedEventTypes = { "all", "conference" };

        public ILogger Logger { get; }

        public IValidator Validator { get; }

        public AvendDbContext AvendDbContext { get; }

        /// <summary>
        /// The only constructor.
        /// </summary>
        /// 
        /// <param name="avendDb">Database context to operate on</param>
        public SuperadminHistoryReader(AvendDbContext avendDb)
        {
            Assert.Argument(avendDb, nameof(avendDb)).NotNull();

            AvendDbContext = avendDb;

            Logger = AvendLog.CreateLogger(GetType().Name);

            Validator = new Validator();
        }

        /// <summary>
        /// Constructs historical daily data on new users added into the system.
        /// The returned period ends by yesterday, inclusive.
        /// </summary>
        /// 
        /// <param name="days">Number of days to return data for.</param>
        /// 
        /// <returns>IEnumerable of DateIndexedTupleDto objects for total users added on each day.</returns>
        public async Task<IEnumerable<DateIndexedTupleDto<decimal>>> RetrieveNewUsersHistoryDaily(int days)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-days);
            var endDate = DateTime.UtcNow.Date;

            var aggregatedQuery = AvendDbContext.SubscriptionMembers
                .Where(record => record.CreatedAt >= startDate)
                .Where(record => record.CreatedAt < endDate)
                .GroupBy(record => new {record.CreatedAt.Year, record.CreatedAt.Month, record.CreatedAt.Day})
                .Select(record => new CountIndexedByDate
                {
                    Date = new DateTime(record.Key.Year, record.Key.Month, record.Key.Day),
                    Count = record.Count()
                });

            return await ConstructDailyHistoryListFromAggregatedQuery(days, startDate, aggregatedQuery);
        }

        /// <summary>
        /// Constructs historical daily data on new subscriptions added into the system.
        /// 
        /// If 'trial' type is passed over, it returns trial subscriptions. 
        /// If 'paid' type is passed over, it returns all but trial subscriptions. 
        /// Otherwise it returns all subscriptions.
        /// 
        /// The returned period ends by yesterday, inclusive.
        /// </summary>
        /// <param name="type">Type of subscription to filter on.</param>
        /// <param name="days">Number of days to return data for.</param>
        /// 
        /// <returns>IEnumerable of DateIndexedTupleDto objects for total trial subscriptions added on each day.</returns>
        public async Task<IEnumerable<DateIndexedTupleDto<decimal>>> RetrieveNewSubscriptionsHistoryDaily(string type, int days)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-days);
            var endDate = DateTime.UtcNow.Date;

            Expression<Func<SubscriptionRecord, bool>> isTrialConditionExpression;
            if (type == "trial")
                isTrialConditionExpression = record => record.Type == "trial";
            else if (type == "paid")
                isTrialConditionExpression = record => record.Type != "trial";
            else 
                isTrialConditionExpression = record => true;

            var aggregatedQuery = AvendDbContext.SubscriptionsTable
                .Where(record => record.CreatedAt >= startDate)
                .Where(record => record.CreatedAt < endDate)
                .Where(isTrialConditionExpression)
                .GroupBy(record => new { record.CreatedAt.Year, record.CreatedAt.Month, record.CreatedAt.Day })
                .Select(record => new CountIndexedByDate
                {
                    Date = new DateTime(record.Key.Year, record.Key.Month, record.Key.Day),
                    Count = record.Count()
                });

            return await ConstructDailyHistoryListFromAggregatedQuery(days, startDate, aggregatedQuery);
        }

        /// <summary>
        /// Constructs historical monthly data on average leads added into the system.
        /// 
        /// The returned period ends by yesterday, inclusive.
        /// </summary>
        /// <param name="months">Number of days to return data for.</param>
        /// 
        /// <returns>IEnumerable of DateIndexedTupleDto objects for total trial subscriptions added on each day.</returns>
        public async Task<IEnumerable<DateIndexedTupleDto<decimal>>> RetrieveAverageLeadsHistoryMonthly(int months)
        {
            var todayStartDate = DateTime.UtcNow.Date;

            var endDate = todayStartDate.AddDays(-todayStartDate.Day + 1);
            var startDate = endDate.AddMonths(-months);
            
            var aggregatedLeadsQuery = AvendDbContext.LeadsTable
                .Where(record => record.CreatedAt >= startDate)
                .Where(record => record.CreatedAt < endDate)
                .GroupBy(record => new { record.CreatedAt.Year, record.CreatedAt.Month })
                .Select(record => new CountIndexedByDate
                {
                    Date = new DateTime(record.Key.Year, record.Key.Month, 1),
                    Count = record.Count()
                });

            var olderUsersCount = AvendDbContext.SubscriptionMembers
                .Count(record => record.CreatedAt < startDate);

            var aggregatedUsersQuery = AvendDbContext.SubscriptionMembers
                .Where(record => record.CreatedAt >= startDate)
                .Where(record => record.CreatedAt < endDate)
                .GroupBy(record => new { record.CreatedAt.Year, record.CreatedAt.Month })
                .Select(record => new CountIndexedByDate
                {
                    Date = new DateTime(record.Key.Year, record.Key.Month, 1),
                    Count = record.Count()
                });

            var totalLeads = (await ConstructMonthlyHistoryListFromAggregatedQuery(months, startDate, aggregatedLeadsQuery)).ToList();

            var incrementalUsers = (await ConstructMonthlyHistoryListFromAggregatedQuery(months, startDate, aggregatedUsersQuery)).ToList();

            Assert.State(totalLeads.Count).EqualsTo(incrementalUsers.Count);

            decimal aggregatedUsers = olderUsersCount;
            for (var i = 0; i < totalLeads.Count; i++)
            {
                Assert.State(totalLeads[i].Date).EqualsTo(incrementalUsers[i].Date);

                aggregatedUsers += incrementalUsers[i].Value;

                if (aggregatedUsers > 0)
                    totalLeads[i].Value /= aggregatedUsers;
            }

            return totalLeads;
        }

        /// <summary>
        /// Constructs historical monthly data on average events data added into the system.
        /// 
        /// If 'conference' type is passed over, it returns conference events. 
        /// If 'personal' type is passed over, it returns all personal events. 
        /// Otherwise it returns all events.
        /// 
        /// The returned period ends by yesterday, inclusive.
        /// </summary>
        /// <param name="type">Events type, see summary for possible values.</param>
        /// <param name="months">Number of days to return data for.</param>
        /// 
        /// <returns>IEnumerable of DateIndexedTupleDto objects for total trial subscriptions added on each day.</returns>
        public async Task<IEnumerable<DateIndexedTupleDto<decimal>>> RetrieveAverageEventsHistoryMonthly(string type, int months)
        {
            var todayStartDate = DateTime.UtcNow.Date;

            var endDate = todayStartDate.AddDays(-todayStartDate.Day + 1);
            var startDate = endDate.AddMonths(-months);

            Expression<Func<EventRecord, bool>> EventTypeCondition;
            if (type == "conference")
                EventTypeCondition = record => record.Type == EventRecord.EventTypeConference;
            else if (type == "personal")
                EventTypeCondition = record => record.Type == EventRecord.EventTypePersonal;
            else
                EventTypeCondition = record => true;

            var aggregatedLeadsQuery = AvendDbContext.EventsTable
                .Where(EventTypeCondition)
                .Where(record => record.CreatedAt >= startDate)
                .Where(record => record.CreatedAt < endDate)
                .GroupBy(record => new { record.CreatedAt.Year, record.CreatedAt.Month })
                .Select(record => new CountIndexedByDate
                {
                    Date = new DateTime(record.Key.Year, record.Key.Month, 1),
                    Count = record.Count()
                });

            var olderUsersCount = AvendDbContext.SubscriptionMembers
                .Count(record => record.CreatedAt < startDate);

            var aggregatedUsersQuery = AvendDbContext.SubscriptionMembers
                .Where(record => record.CreatedAt >= startDate)
                .Where(record => record.CreatedAt < endDate)
                .GroupBy(record => new { record.CreatedAt.Year, record.CreatedAt.Month })
                .Select(record => new CountIndexedByDate
                {
                    Date = new DateTime(record.Key.Year, record.Key.Month, 1),
                    Count = record.Count()
                });

            var totalLeads = (await ConstructMonthlyHistoryListFromAggregatedQuery(months, startDate, aggregatedLeadsQuery)).ToList();

            var incrementalUsers = (await ConstructMonthlyHistoryListFromAggregatedQuery(months, startDate, aggregatedUsersQuery)).ToList();

            Assert.State(totalLeads.Count).EqualsTo(incrementalUsers.Count);

            decimal aggregatedUsers = olderUsersCount;
            for (var i = 0; i < totalLeads.Count; i++)
            {
                Assert.State(totalLeads[i].Date).EqualsTo(incrementalUsers[i].Date);

                aggregatedUsers += incrementalUsers[i].Value;

                if (aggregatedUsers > 0)
                    totalLeads[i].Value /= aggregatedUsers;
            }

            return totalLeads;
        }

        /// <summary>
        /// Auxiliary method that actually constructs and returns the daily history 
        /// data based on the aggregated query that is passed to it.
        /// </summary>
        /// 
        /// <param name="days">Number of days to return data for.</param>
        /// <param name="startDate">Date to start with</param>
        /// <param name="aggregatedQuery">Aggregated query to retrieve stats from databaase</param>
        /// 
        /// <returns>IEnumerable of DateIndexedTupleDto objects with data from the query and with properly filled zero-data days.</returns>
        protected static async Task<IEnumerable<DateIndexedTupleDto<decimal>>> ConstructDailyHistoryListFromAggregatedQuery(int days, DateTime startDate, IQueryable<CountIndexedByDate> aggregatedQuery)
        {
            return await ConstructCustomHistoryListFromAggregatedQuery(
                aggregatedQuery,
                () => HistoryStatsHelper.ConstructDailyDecimalZerosList(startDate, days)
                );
        }

        /// <summary>
        /// Auxiliary method that actually constructs and returns the monthly history 
        /// data based on the aggregated query that is passed to it.
        /// </summary>
        /// 
        /// <param name="months">Number of months to return data for.</param>
        /// <param name="startDate">Date to start with</param>
        /// <param name="aggregatedQuery">Aggregated query to retrieve stats from databaase</param>
        /// 
        /// <returns>IEnumerable of DateIndexedTupleDto objects with data from the query and with properly filled zero-data months.</returns>
        protected static async Task<IEnumerable<DateIndexedTupleDto<decimal>>> ConstructMonthlyHistoryListFromAggregatedQuery(int months, DateTime startDate, IQueryable<CountIndexedByDate> aggregatedQuery)
        {
            return await ConstructCustomHistoryListFromAggregatedQuery(
                aggregatedQuery,
                () => HistoryStatsHelper.ConstructMonthlyDecimalZerosList(startDate, months)
                );
        }

        /// <summary>
        /// Auxiliary method that actually constructs and returns the arbitrary 
        /// history data based on the aggregated query that is passed to it.
        /// </summary>
        /// 
        /// <param name="aggregatedQuery">Aggregated query to retrieve stats from databaase</param>
        /// <param name="initializationFunc">Initializer for the history dictionary that should properly fill all possible data cells with zeros</param>
        /// 
        /// <returns>IEnumerable of DateIndexedTupleDto objects with data from the query and with properly filled zero-data cells.</returns>
        protected static async Task<IEnumerable<DateIndexedTupleDto<decimal>>> ConstructCustomHistoryListFromAggregatedQuery(IQueryable<CountIndexedByDate> aggregatedQuery, Func<Dictionary<DateTime, decimal>> initializationFunc)
        {
            var history = initializationFunc();

            foreach (var statRecord in await aggregatedQuery.ToListAsync())
            {
                if (statRecord.Date != DateTime.UtcNow.Date)
                    history[statRecord.Date] = statRecord.Count;
            }

            return history.Select(record => new DateIndexedTupleDto<decimal>()
            {
                Date = record.Key,
                Value = record.Value,
            });
        }

        /// <summary>
        /// Auxiliary class for holding aggregated data records 
        /// with date and count value.
        /// </summary>
        protected class CountIndexedByDate
        {
            public DateTime Date { get; set; }

            public int Count { get; set; }
        }
    }
}