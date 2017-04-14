using System;
using System.Collections.Generic;
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
    public class SuperadminDashboardService
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
        public SuperadminDashboardService(
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

        /// <summary>
        /// Returns summary data for superadmins' dashboard.
        /// </summary>
        /// 
        /// <returns><see cref="SuperadminDashboardDto"/> object containing the requested data</returns>
        public Task <SuperadminDashboardDto> GetSummary()
        {
            Check.Value(UserContext, nameof(UserContext)).NotNull();
            Check.Value(UserContext.Role, nameof(UserContext), AvendErrors.Forbidden).EqualsTo(UserRole.SuperAdmin, "This dashboard is only available for superadmins");

            SuperadminDashboardDto dto = new SuperadminDashboardDto()
            {
                CreatedAt = DateTime.UtcNow,
            };

            using (var db = new AvendDbContext(DbContextOptions))
            {
                var superadminStatsReader = new SuperadminDashboardSummaryStatsReader(db);

                dto.UsersStats.AllTime = superadminStatsReader.GetAggregatedMembersStats();
                dto.UsersStats.Yesterday = superadminStatsReader.GetAggregatedMembersStats(DateTime.UtcNow.Date.AddDays(-1));

                dto.SubscriptionStats.AllTime = superadminStatsReader.GetAggregatedSubscriptionsStats();
                dto.SubscriptionStats.LastPeriod = superadminStatsReader.GetAggregatedSubscriptionsStats(DateTime.UtcNow.Date.AddDays(-30));

                dto.LeadsStats.AllTime = superadminStatsReader.GetAllTimeLeadsStats();
                dto.EventsStats.AllTime = superadminStatsReader.GetAllTimeEventsStats();
            }

            return Task.FromResult(dto);
        }

        public async Task<IEnumerable<DateIndexedTupleDto<decimal>>> GetNewUsersHistoryDaily(int days)
        {
            Check.Value(UserContext, nameof(UserContext)).NotNull();
            Check.Value(UserContext.Role, nameof(UserContext), AvendErrors.Forbidden).EqualsTo(UserRole.SuperAdmin, "This dashboard is only available for superadmins");
            Check.Value(days, "limit").Greater(0);

            using (var db = new AvendDbContext(DbContextOptions))
            {
                var superadminHistoryReader = new SuperadminHistoryReader(db);

                return await superadminHistoryReader.RetrieveNewUsersHistoryDaily(days);
            }
        }

        public async Task<IEnumerable<DateIndexedTupleDto<decimal>>> GetNewSubscriptionsHistoryDaily(string type, int days)
        {
            Check.Value(UserContext, nameof(UserContext)).NotNull();
            Check.Value(UserContext.Role, nameof(UserContext), AvendErrors.Forbidden).EqualsTo(UserRole.SuperAdmin, "This dashboard is only available for superadmins");
            Check.Value(days, "limit").Greater(0);
            Check.Value(type, "type").In(SuperadminHistoryReader.AllowedSubscriptionTypes);

            using (var db = new AvendDbContext(DbContextOptions))
            {
                var superadminHistoryReader = new SuperadminHistoryReader(db);

                return await superadminHistoryReader.RetrieveNewSubscriptionsHistoryDaily(type, days);
            }
        }

        public async Task<IEnumerable<DateIndexedTupleDto<decimal>>> GetAverageLeadsHistoryMonthly(int months)
        {
            Check.Value(UserContext, nameof(UserContext)).NotNull();
            Check.Value(UserContext.Role, nameof(UserContext), AvendErrors.Forbidden).EqualsTo(UserRole.SuperAdmin, "This dashboard is only available for superadmins");
            Check.Value(months, "limit").Greater(0);

            using (var db = new AvendDbContext(DbContextOptions))
            {
                var superadminHistoryReader = new SuperadminHistoryReader(db);

                return await superadminHistoryReader.RetrieveAverageLeadsHistoryMonthly(months);
            }
        }

        public async Task<IEnumerable<DateIndexedTupleDto<decimal>>> GetAverageEventsHistoryMonthly(string type, int months)
        {
            Check.Value(UserContext, nameof(UserContext)).NotNull();
            Check.Value(UserContext.Role, nameof(UserContext), AvendErrors.Forbidden).EqualsTo(UserRole.SuperAdmin, "This dashboard is only available for superadmins");
            Check.Value(months, "limit").Greater(0);
            Check.Value(type, "type").In(SuperadminHistoryReader.AllowedEventTypes);

            using (var db = new AvendDbContext(DbContextOptions))
            {
                var superadminHistoryReader = new SuperadminHistoryReader(db);

                return await superadminHistoryReader.RetrieveAverageEventsHistoryMonthly(type, months);
            }
        }
    }
}