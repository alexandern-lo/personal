using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;

namespace Avend.API.Infrastructure.SearchExtensions
{
    /// <summary>
    /// Default search operation which generates query with appropriate sorting, filtering and pagination.
    /// </summary>
    /// <typeparam name="T">search target EF model</typeparam>
    public class DefaultSearch<T> where T : class
    {
        public SearchQueryParams QueryParams { get; }
        public IQueryable<T> Collection { get; }
        private IQueryable<T> _result;
        public string[] FilterProperties { get; set; }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public DefaultSearch(SearchQueryParams queryParams, IQueryable<T> collection)
        {
            QueryParams = Assert.Argument(queryParams, nameof(queryParams)).NotNull().Value;
            Collection = Assert.Argument(collection, nameof(collection)).NotNull().Value;
            QueryParams.Validate(ArgumentValidator.Instance);
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public IEnumerable<T> Filter(Func<IQueryable<T>, IQueryable<T>> filter)
        {
            var filtered = filter(Result);
            Assert.State(filtered, nameof(filter)).NotNull();
            _result = filtered;
            return _result;
        }

        public SearchResult<T> Paginate()
        {
            return Paginate(x => x);
        }

        /// <summary>
        /// Add Skip(x).Take(y) to query and return result.
        /// </summary>
        /// <typeparam name="TTarget">Target type</typeparam>
        /// <param name="converter">converter function to process results</param>
        /// <returns>Search result</returns>
        /// <remarks>
        /// Beware of Linq issue with subqueries and .Any() - https://github.com/aspnet/EntityFramework/issues/3317
        /// </remarks>
        public SearchResult<TTarget> Paginate<TTarget>(Func<T, TTarget> converter)
        {
            var enumerable = Result
                .Skip(QueryParams.PageNumber * QueryParams.RecordsPerPage)
                .Take(QueryParams.RecordsPerPage)
                .Select(converter);
            var count = Result.Count();

            return new SearchResult<TTarget>
            {
                Data = enumerable.ToList(),
                Total = count,
                QueryParams = QueryParams
            };
        }

        public async Task<SearchResult<TTarget>> PaginateAsync<TTarget>(Func<T, TTarget> converter)
        {
            var q = Result
                 .Skip(QueryParams.PageNumber * QueryParams.RecordsPerPage)
                 .Take(QueryParams.RecordsPerPage);
            var count = Result.CountAsync();
            var list = q.ToListAsync();
            await Task.WhenAll(count, list);

            return new SearchResult<TTarget>
            {
                Data = list.Result.Select(converter).ToList(),
                Total = count.Result,
                QueryParams = QueryParams
            };
        }

        public IQueryable<T> Result
        {
            get
            {
                if (_result == null)
                {
                    _result = Sort(Filter(Collection, QueryParams), QueryParams);
                }
                return _result;
            }
            protected set { _result = value; }
        }

        public static IQueryable<T> Filter(IQueryable<T> collection, IFilterQueryParams queryParams, string[] filterProperties = null)
        {
            if (queryParams.Filter != null)
            {
                if (filterProperties == null)
                {
                    var defaultFilter = typeof(T).GetTypeInfo().GetCustomAttribute<DefaultFilterAttribute>();
                    filterProperties = defaultFilter?.Properties;
                }
                if (filterProperties?.Length > 0)
                {
                    var queryable = collection.AsQueryable();
                    var x = Expression.Parameter(typeof(T), "x");

                    var checks = filterProperties
                        //x.Field
                        .Select(prop => Expression.Property(x, prop))
                        //x.Field.Contains(Filter)
                        .Select(
                            propValue =>
                                Expression.Call(propValue, typeof(string).GetMethod("Contains"),
                                    Expression.Constant(queryParams.Filter)))
                        //x.Field1.Contains(Filter) || x.Field2.Contains(Filter) || ...
                        .Aggregate<Expression>(Expression.OrElse);
                    //x => x.Field1.Contains(Filter) || x.Field2.Contains(Filter) || ...
                    var check = Expression.Lambda(checks, x);
                    //collection.Where(x => x.Field1.Contains(Filter) || x.Field2.Contains(Filter) || ...)
                    var where = Expression.Call(
                        typeof(Queryable),
                        "Where",
                        new[] { typeof(T) },
                        queryable.Expression,
                        check);
                    collection = queryable.Provider.CreateQuery<T>(@where);
                }
            }

            return collection;
        }

        public static PropertyInfo GetSortProperty(ISortingQueryParams query)
        {
            if (query?.SortField == null) return null;
            return (
                from p in typeof(T).GetProperties()
                from attr in p.GetCustomAttributes<ColumnAttribute>()
                where attr.Name == query.SortField
                select p
            ).FirstOrDefault();
        }

        public static IQueryable<T> Sort(IQueryable<T> collection, ISortingQueryParams sortingQueryParams)
        {
            if (sortingQueryParams.SortField == null)
                return collection;

            var sortFieldProperty = GetSortProperty(sortingQueryParams);

            if (sortFieldProperty == null)
                return collection;

            var x = Expression.Parameter(typeof(T), "x");

            //x => x.Property
            var getter = Expression.Lambda(Expression.Property(x, sortFieldProperty.Name), x);
            var queryable = collection.AsQueryable();
            var orderMethod = sortingQueryParams.SortOrder == "asc" ? "OrderBy" : "OrderByDescending";

            //OrderBy(x => x.Property)
            var orderBy = Expression.Call(
                typeof(Queryable),
                orderMethod,
                new[] { typeof(T), sortFieldProperty.PropertyType },
                queryable.Expression,
                getter);

            //collection.OrderBy(x => x.Property)
            collection = queryable.Provider.CreateQuery<T>(orderBy);

            return collection;
        }
    }

    public static class DefaultSearch
    {
        public static DefaultSearch<T> Start<T>(SearchQueryParams queryParams, IQueryable<T> db) where T : class
        {
            return new DefaultSearch<T>(queryParams, db);
        }
    }
}