using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

using Avend.API.Infrastructure.Logging;
using Avend.API.Infrastructure.SearchExtensions;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Dashboard.NetworkDTO;
using Avend.API.Services.Events;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Qoden.Validation;

namespace Avend.API.Services.Dashboard
{
    public class DashboardReader
    {
        public ILogger Logger { get; }

        public IValidator Validator { get; }

        private UserContext UserContext { get; }
        public AvendDbContext AvendDbContext { get; }

        public DashboardReader(UserContext userContext, AvendDbContext avendDb)
        {
            Assert.Argument(userContext, nameof(userContext)).NotNull();
            Assert.Argument(avendDb, nameof(avendDb)).NotNull();

            UserContext = userContext;
            AvendDbContext = avendDb;

            Logger = AvendLog.CreateLogger(GetType().Name);

            Validator = new Validator();
        }

        /// <summary>
        /// Constructs sorted dashboard event DTOs.
        /// 
        /// DTOs contain leads goal, acquired leads and total expense for each.
        /// </summary>
        /// 
        /// <param name="userUid">Uid of the user for whom to get data</param>
        /// <param name="eventsCount">Maximum number of objects to retrieve</param>
        /// <param name="eventsSortField">Name of the field to sort events by</param>
        /// <param name="eventsSortOrder">Order of events sorting</param>
        /// 
        /// <returns>List of DashboardEventDTO</returns>
        public List<DashboardEventDTO> ConstructSortedDashboardEventDtos(Guid userUid, int eventsCount, string eventsSortField, string eventsSortOrder)
        {
            var sortingQueryParams = new SortingQueryParams()
            {
                SortField = eventsSortField,
                SortOrder = eventsSortOrder,
            };

            IQueryable<EventRecord> eventsTable = AvendDbContext.EventsTable.Where(UserContext.SelectableEvents());
            eventsTable = DefaultSearch<EventRecord>.Sort(eventsTable, sortingQueryParams);
            if (eventsCount > 0)
                eventsTable = eventsTable.Take(eventsCount);

            Dictionary<Guid, DashboardEventDTO> events = new Dictionary<Guid, DashboardEventDTO>();

            var eventIds = new List<long>();

            foreach (var eventRecord in eventsTable)
            {
                events[eventRecord.Uid] = DashboardEventDTO.From(eventRecord, CurrencyCode.USD, 0, 0);

                eventIds.Add(eventRecord.Id);
            }

            var eventUserExpensesTable = AvendDbContext.EventUserExpensesTable
                .Include(record => record.EventRecord)
                .Where(
                    record => record.UserUid == userUid
                              && eventIds.Contains(record.EventId)
                    )
                .GroupBy(record => record.EventRecord.Uid)
                .Select(record => new
                {
                    EventUid = record.Key,
                    TotalExpenses = record.Sum(amnt => amnt.Amount),
                    Currency = record.First().Currency,
                });

            foreach (var userExpense in eventUserExpensesTable)
            {
                events[userExpense.EventUid].TotalExpenses = new MoneyDto()
                {
                    Currency = userExpense.Currency,
                    Amount = userExpense.TotalExpenses,
                };
            }

            var eventUserGoalsTable = AvendDbContext.EventUserGoalsTable
                .Include(record => record.Event)
                .Where(record => record.UserUid == userUid
                                 && eventIds.Contains(record.EventId)
                );

            foreach (var userGoal in eventUserGoalsTable)
            {
                events[userGoal.Event.Uid].LeadsGoal = userGoal.LeadsGoal;
                events[userGoal.Event.Uid].LeadsCount = userGoal.LeadsAcquired;
            }

            return events.Values.ToList();
        }

        /// <summary>
        /// Constructs mobile-styled dashboard event DTOs list.
        /// Conference events show up first, sorted by date of recent conference events, then by total leads taken at the event.
        /// Everyday personal events show up after those (if limit allows) sorted first by total leads taken to date, then by most recent date modified.
        /// 
        /// DTOs contain leads goal, acquired leads and total expense for each.
        /// </summary>
        /// 
        /// <param name="userUid">Uid of the user for whom to get data</param>
        /// <param name="eventsCount">Maximum number of objects to retrieve</param>
        /// 
        /// <returns>List of DashboardEventDTO</returns>
        public List<DashboardEventDTO> ConstructMobileStyledDashboardEventDtos(Guid userUid, int eventsCount)
        {
            List<EventRecord> eventsTable = new List<EventRecord>();

            var columnsList = typeof(EventRecord).GetProperties()
                .Where(record => record.GetCustomAttributes<ColumnAttribute>().Any())
                .Select(record => record.GetCustomAttributes<ColumnAttribute>().FirstOrDefault())
                ;

            var allEventColumnNamesAggregatedAndEventIdDisambiguated =
                "e." + columnsList.Select(record => record.Name).Aggregate((aggregated, columnName) =>
                    new [] { "recurring", "deleted" }.Contains(columnName)
                        ? $"{aggregated}, CAST (MIN(e.{columnName}+0) AS bit) as {columnName}"
                        : $"{aggregated}, MIN(e.{columnName}) as {columnName}"
                );

            IEnumerable<EventRecord> conferenceEventsTable = AvendDbContext.EventsTable.FromSql(
                "SELECT " + allEventColumnNamesAggregatedAndEventIdDisambiguated +
                " FROM events AS e LEFT JOIN leads AS l" +
                " ON (e.event_id = l.event_id AND l.user_uid=@UserUid)" +
                " WHERE e.deleted=0 AND e.event_type='conference' AND e.start_date<GETUTCDATE()" +
                " GROUP BY e.event_id" +
                " ORDER BY MIN(e.start_date) DESC, count(l.lead_id) DESC",
                new object[]
                    {
                        new SqlParameter("UserUid", UserContext.UserUid),
                        new SqlParameter("UserId", UserContext.UserId),
                    }
                );

            if (eventsCount > 0)
                conferenceEventsTable = conferenceEventsTable.Take(eventsCount);

            eventsTable.AddRange(conferenceEventsTable);

            if (conferenceEventsTable.Count() < eventsCount)
            {
                IEnumerable<EventRecord> personalEventsTable = AvendDbContext.EventsTable.FromSql(
                    "SELECT " + allEventColumnNamesAggregatedAndEventIdDisambiguated +
                    " FROM events AS e LEFT JOIN leads AS l" +
                    " ON (e.event_id = l.event_id AND l.user_uid=@UserUid)" +
                    " WHERE e.deleted=0 AND e.event_type='personal' AND e.recurring=1" +
                    " AND e.owner_id=@UserId" +
                    //                "GROUP BY e.event_id, e.event_uid, e.event_type, e.address, e.owner_id, e.address, e.address, e.address, e.address, e.address " +
                    " GROUP BY e.event_id" +
                    " ORDER BY count(l.lead_id) DESC, MIN(e.updated_at) DESC",
                    new object[]
                        {
                        new SqlParameter("UserUid", UserContext.UserUid),
                        new SqlParameter("UserId", UserContext.UserId),
                        }
                    );

                personalEventsTable = personalEventsTable.Take(eventsCount - conferenceEventsTable.Count());

                eventsTable.AddRange(personalEventsTable);
            }

            Dictionary<Guid, DashboardEventDTO> events = new Dictionary<Guid, DashboardEventDTO>();

            var eventIds = new List<long>();

            foreach (var eventRecord in eventsTable)
            {
                events[eventRecord.Uid] = DashboardEventDTO.From(eventRecord, CurrencyCode.USD, 0, 0);

                eventIds.Add(eventRecord.Id);
            }

            var eventUserExpensesTable = AvendDbContext.EventUserExpensesTable
                .Include(record => record.EventRecord)
                .Where(
                    record => record.UserUid == userUid
                              && eventIds.Contains(record.EventId)
                    )
                .GroupBy(record => record.EventRecord.Uid)
                .Select(record => new
                {
                    EventUid = record.Key,
                    TotalExpenses = record.Sum(amnt => amnt.Amount),
                    Currency = record.First().Currency,
                });

            foreach (var userExpense in eventUserExpensesTable)
            {
                events[userExpense.EventUid].TotalExpenses = new MoneyDto()
                {
                    Currency = userExpense.Currency,
                    Amount = userExpense.TotalExpenses,
                };
            }

            var eventUserGoalsTable = AvendDbContext.EventUserGoalsTable
                .Include(record => record.Event)
                .Where(record => record.UserUid == userUid
                                 && eventIds.Contains(record.EventId)
                );

            foreach (var userGoal in eventUserGoalsTable)
            {
                events[userGoal.Event.Uid].LeadsGoal = userGoal.LeadsGoal;
                events[userGoal.Event.Uid].LeadsCount = userGoal.LeadsAcquired;
            }

            return events.Values.ToList();
        }

        public IEnumerable<DashboardResourceDTO> ConstructDashboardResourceDtos(Guid userUid, int resourcesCount, string resourcesSortField, string resourcesSortOrder)
        {
            var sortingQueryParams = new SortingQueryParams()
            {
                SortField = resourcesSortField,
                SortOrder = resourcesSortOrder,
            };

            var resourceRecords = AvendDbContext.ResourcesTable.Where(record => record.UserUid == userUid);

            resourceRecords = DefaultSearch<Resource>.Sort(resourceRecords, sortingQueryParams);

            if (resourcesCount > 0)
                resourceRecords = resourceRecords.Take(resourcesCount);

            var resources = resourceRecords.Select(DashboardResourceDTO.From);

            return resources;
        }

        /// <summary>
        /// Produces enumerable with daily history leads count data for the user with given Uid.
        /// </summary>
        /// 
        /// <param name="userUid">Uid of the user to retrieve data for</param>
        /// <param name="count">Number of days in the past to get data for</param>
        /// 
        /// <returns>Enumerable of DateDecimalTupleDTO holding the leads count data in ascending order</returns>
        public IEnumerable<DateIndexedTupleDto<decimal>> ProduceLeadsCountDailyHistoryForUser(Guid userUid, int count)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-count);

            var history = HistoryStatsHelper.ConstructDailyDecimalZerosList(startDate, count);

            var leadCounts = AvendDbContext.LeadsTable.Where(record => record.UserUid == userUid && record.CreatedAt >= startDate)
                .GroupBy(record => new {record.CreatedAt.Year, record.CreatedAt.Month, record.CreatedAt.Day})
                .Select(record => new
                {
                    Date = new DateTime(record.Key.Year, record.Key.Month, record.Key.Day),
                    Value = record.Count()
                });

            foreach (var leadCount in leadCounts)
            {
                if (leadCount.Date != DateTime.UtcNow.Date)
                    history[leadCount.Date] = leadCount.Value;
            }

            return history.Select(record => new DateIndexedTupleDto<decimal>()
            {
                Date = record.Key,
                Value = record.Value,
            });
        }

        /// <summary>
        /// Auxiliary method to produce statistics for insertion into DashboardDTO.
        /// </summary>
        /// 
        /// <param name="userUid">UID of the user to get statistics for</param>
        /// 
        /// <returns>DTO containing user statistics</returns>
        public LeadsStatisticsDTO GetLeadsStatisticsForUser(Guid userUid)
        {
            var lastPeriodStart = DateTime.UtcNow.AddDays(-30);
            var thisYearStart = new DateTime(DateTime.UtcNow.Year, 1, 1).ToUniversalTime();

            var thisYearExpenses = AvendDbContext.EventUserExpensesTable.Where(record => record.UserUid == userUid && record.CreatedAt >= thisYearStart).Sum(record => record.Amount);
            var thisYearExpensesCurrency = AvendDbContext.EventUserExpensesTable.Where(record => record.UserUid == userUid && record.CreatedAt >= thisYearStart).Select(record => record.Currency).FirstOrDefault();
            if (thisYearExpensesCurrency == CurrencyCode.Unknown)
                thisYearExpensesCurrency = CurrencyCode.USD;

            var thisYearLeads = AvendDbContext.LeadsTable.Count(record => record.UserUid == userUid && record.CreatedAt >= lastPeriodStart);

            var thisYearAverageLeadCost = thisYearExpenses;
            if (thisYearLeads > 0)
                thisYearAverageLeadCost = thisYearExpenses / thisYearLeads;

            var leadsStats = new LeadsStatisticsDTO()
            {
                AllTimeCount = AvendDbContext.LeadsTable.Count(record => record.UserUid == userUid),
                AllTimeGoal = AvendDbContext.EventUserGoalsTable.Where(record => record.UserUid == userUid).Sum(record => record.LeadsGoal),

                LastPeriodCount = AvendDbContext.LeadsTable.Count(record => record.UserUid == userUid && record.CreatedAt >= lastPeriodStart),
                LastPeriodGoal = AvendDbContext.EventUserGoalsTable.Where(record => record.UserUid == userUid && record.CreatedAt >= lastPeriodStart).Sum(record => record.LeadsGoal),

                ThisYearCostPerLead = new MoneyDto()
                {
                    Currency = thisYearExpensesCurrency,
                    Amount = thisYearAverageLeadCost,
                },
                ThisYearExpenses = new MoneyDto()
                {
                    Currency = thisYearExpensesCurrency,
                    Amount = thisYearExpenses,
                },
            };

            return leadsStats;
        }
    }
}
