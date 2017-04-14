using System;
using System.Collections.Generic;

using Qoden.Validation;

namespace Avend.API.Services.Dashboard
{
    public static class HistoryStatsHelper
    {
        /// <summary>
        /// Produces dictionary indiced by dates and containing decimal zero values for each day.
        /// </summary>
        /// 
        /// <param name="date">Date to start dictionary with</param>
        /// <param name="count">Number of dates to generate</param>
        /// 
        /// <returns>Populated dictionary with zero values</returns>
        public static Dictionary<DateTime, decimal> ConstructDailyDecimalZerosList(DateTime date, int count)
        {
            return ConstructPeriodicZerosListIndexedByDateTime<decimal>(date, count, datetime => datetime.AddDays(1));
        }

        /// <summary>
        /// Produces dictionary indiced by month start dates and containing decimal zero values for each month.
        /// </summary>
        /// 
        /// <param name="date">Date to start dictionary with</param>
        /// <param name="count">Number of dates to generate</param>
        /// 
        /// <returns>Populated dictionary with zero values</returns>
        public static Dictionary<DateTime, decimal> ConstructMonthlyDecimalZerosList(DateTime date, int count)
        {
            return ConstructPeriodicZerosListIndexedByDateTime<decimal>(date, count, datetime => datetime.AddMonths(1));
        }

        /// <summary>
        /// Produces dictionary indexed by <see cref="DateTime"/> values and containing decimal zero values for each of those.
        /// </summary>
        /// 
        /// <param name="startDate">Date to start dictionary with</param>
        /// <param name="count">Number of dates to generate</param>
        /// <param name="incrementFunc">Function to increment datetime index on each step</param>
        /// 
        /// <returns>Populated dictionary with zero values</returns>
        public static Dictionary<DateTime, T> ConstructPeriodicZerosListIndexedByDateTime<T>(DateTime startDate, int count, Func<DateTime, DateTime> incrementFunc)
        {
            Assert.Argument(incrementFunc, "increment_function").NotNull("Increment function should be defined in " + nameof(ConstructPeriodicZerosListIndexedByDateTime));

            var history = new Dictionary<DateTime, T>();

            while (count > 0)
            {
                history[startDate] = default(T);

                startDate = incrementFunc(startDate);

                count--;
            }

            return history;
        }
    }
}