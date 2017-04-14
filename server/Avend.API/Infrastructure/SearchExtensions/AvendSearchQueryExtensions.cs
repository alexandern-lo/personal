using System;
using Avend.API.Infrastructure.SearchExtensions.Data;

namespace Avend.API.Infrastructure.SearchExtensions
{
    public static class AvendSearchQueryExtensions
    {
        /// <summary>
        /// Apply default sorting directions if it is missing from search query.
        /// </summary>
        /// <remarks>By default date time fields are sorted by 'desc' and others sorted by 'asc'</remarks>
        /// <typeparam name="T">type of entity to be found</typeparam>
        /// <param name="query">search query</param>
        public static void ApplyDefaultSortOrder<T>(this SearchQueryParams query) where T : class
        {
            if (query.SortOrder != null) return;
            var sortProperty = DefaultSearch<T>.GetSortProperty(query);
            if (sortProperty == null) return;
            var type = sortProperty.PropertyType;
            query.SortOrder = type == typeof(DateTime) || type == typeof(DateTime?) ? "desc" : "asc";
        }
    }
}