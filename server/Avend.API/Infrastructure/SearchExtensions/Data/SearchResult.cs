using System.Collections.Generic;
using System.Linq;

namespace Avend.API.Infrastructure.SearchExtensions.Data
{
    /// <summary>
    /// Search operation result, contains result list and total number of entries
    /// </summary>
    /// <typeparam name="T">type of result element</typeparam>
    public class SearchResult<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int Total { get; set; }

        public static SearchResult<T> Empty => new SearchResult<T>
        {
            Data = Enumerable.Empty<T>(),
            Total = 0
        };

        public SearchQueryParams QueryParams { get; set; }

        public Dictionary<string, object> Filters { get; set; }

        public SearchResult<T> WithFilter(string filter, object value)
        {
            if (value != null)
            {
                if (Filters == null) Filters = new Dictionary<string, object>();
                Filters.Add(filter, value);
            }
            return this;
        }
    }
}