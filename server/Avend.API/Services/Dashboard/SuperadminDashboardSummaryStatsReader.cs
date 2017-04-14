using System;
using System.Linq;

using Avend.API.Model;
using Avend.API.Services.Dashboard.NetworkDTO;

using Microsoft.EntityFrameworkCore;

namespace Avend.API.Services.Dashboard
{
    /// <summary>
    /// Helper class to incapsulate requests for global aggregated stats
    /// that are required for superadmin dashboard summary widget.
    /// </summary>
    public class SuperadminDashboardSummaryStatsReader
    {
        public AvendDbContext DbContext { get; }

        public SuperadminDashboardSummaryStatsReader(AvendDbContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <summary>
        /// Returns all-time member types data.
        /// </summary>
        /// 
        /// <param name="from">Start of the date & time interval to get data for.</param>
        /// 
        /// <returns><see cref="PaidVsTrialStatsDto"/> object containing all-time record for trial, paid and total number of users</returns>
        public PaidVsTrialStatsDto GetAggregatedMembersStats(DateTime? from = null)
        {
            IQueryable<SubscriptionMember> membersQuery = DbContext.SubscriptionMembers
                .Include(record => record.Subscription)
                .Where(record => record.CreatedAt < DateTime.UtcNow.Date);

            if (from.HasValue)
            {
                membersQuery = membersQuery
                    .Where(record => record.CreatedAt >= from.Value);
            }

            var aggregatedQuery = membersQuery
                .GroupBy(record => record.Subscription.Type)
                .Select(record => new CountIndexedByStatus
                {
                    Status = record.Key ?? "unknown",
                    Count = record.Count()
                });

            return AggregateFreeVsTrialStats(aggregatedQuery);
        }

        /// <summary>
        /// Returns all-time subscriptions data.
        /// </summary>
        /// 
        /// <param name="from">Start of the date & time interval to get data for.</param>
        /// 
        /// <returns><see cref="PaidVsTrialStatsDto"/> object containing all-time record for trial, paid and total number of subscriptions.</returns>
        public PaidVsTrialStatsDto GetAggregatedSubscriptionsStats(DateTime? from = null)
        {
            IQueryable<SubscriptionRecord> subscriptionsQuery = DbContext.SubscriptionsTable
                .Where(record => record.Status != SubscriptionStatus.Replaced)
                .Where(record => record.CreatedAt < DateTime.UtcNow.Date);

            if (from.HasValue)
            {
                subscriptionsQuery  = subscriptionsQuery 
                    .Where(record => record.CreatedAt >= from.Value);
            }

            var aggregatedQuery = subscriptionsQuery
                .GroupBy(record => record.Type)
                .Select(record => new CountIndexedByStatus
                {
                    Status = record.Key ?? "unknown",
                    Count = record.Count()
                });

            return AggregateFreeVsTrialStats(aggregatedQuery);
        }

        /// <summary>
        /// Returns all-time events data.
        /// </summary>
        /// 
        /// <returns><see cref="PaidVsTrialStatsDto"/> object containing all-time record for trial, paid and total number of subscriptions.</returns>
        public EventsStatsDto GetAllTimeEventsStats()
        {
            var eventsQuery = DbContext.EventsTable
                .Where(record => record.CreatedAt < DateTime.UtcNow.Date)
                .GroupBy(record => record.Type)
                .Select(record => new CountIndexedByStatus
                {
                    Status = record.Key ?? "unknown",
                    Count = record.Count()
                });

            return AggregateEventsStats(eventsQuery);
        }

        /// <summary>
        /// Returns all-time leads data.
        /// </summary>
        /// 
        /// <returns><see cref="PaidVsTrialStatsDto"/> object containing all-time record for trial, paid and total number of subscriptions.</returns>
        public PaidVsTrialStatsDto GetAllTimeLeadsStats()
        {
            var leadsTotal = DbContext.LeadsTable
                .Where(record => record.CreatedAt < DateTime.UtcNow.Date)
                .Count(record => !record.Deleted);

            return new PaidVsTrialStatsDto()
            {
                Total = leadsTotal
            };
        }

        /// <summary>
        /// Auxiliary method to perform actual construction of <see cref="PaidVsTrialStatsDto"/> from grouped query.
        /// </summary>
        /// 
        /// <param name="membersQuery">Query to get data from</param>
        /// <param name="initWithZeros">If set to true, the stats DTO will have all fields initialized with zeros</param>
        /// 
        /// <returns>DTO object with aggregated data retrieved from membersQuery parameter</returns>
        private static PaidVsTrialStatsDto AggregateFreeVsTrialStats(IQueryable<CountIndexedByStatus> membersQuery, bool initWithZeros = true)
        {
            PaidVsTrialStatsDto dto;

            dto = initWithZeros
                ? new PaidVsTrialStatsDto()
                {
                    Total = 0,
                    Paid = 0,
                    Trial = 0,
                }
                : new PaidVsTrialStatsDto();

            foreach (var record in membersQuery)
            {
                if (record.Status == "trial")
                    dto.Trial = record.Count;
                else
                    dto.Paid += record.Count;

                dto.Total += record.Count;
            }

            return dto;
        }

        /// <summary>
        /// Auxiliary method to perform actual construction of <see cref="EventsStatsDto"/> from grouped query.
        /// </summary>
        /// 
        /// <param name="aggregatedQuery">Query to get data from</param>
        /// <param name="initWithZeros">If set to true, the stats DTO will have all fields initialized with zeros</param>
        /// 
        /// <returns>DTO object with aggregated data retrieved from aggregatedQuery parameter</returns>
        protected static EventsStatsDto AggregateEventsStats(IQueryable<CountIndexedByStatus> aggregatedQuery, bool initWithZeros = true)
        {
            var dto = initWithZeros
                ? new EventsStatsDto()
                {
                    Total = 0,
                    Conference = 0,
                }
                : new EventsStatsDto();

            foreach (var record in aggregatedQuery)
            {
                if (record.Status == EventRecord.EventTypeConference)
                    dto.Conference = record.Count;

                dto.Total += record.Count;
            }

            return dto;
        }

        /// <summary>
        /// Auxiliary class for holding aggregated data records 
        /// with status name and count value.
        /// </summary>
        protected class CountIndexedByStatus
        {
            public string Status { get; set; }

            public int Count { get; set; }
        }
    }
}