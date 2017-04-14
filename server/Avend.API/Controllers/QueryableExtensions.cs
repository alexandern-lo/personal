using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Avend.API.Controllers
{
    public static class QueryableExtensions
    {
        public static string PropertyName<T>(Expression<Func<T, object>> expression)
        {
            var body = expression.Body as MemberExpression ??
                       ((UnaryExpression)expression.Body).Operand as MemberExpression;

            return body?.Member.Name;
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderBy, bool isAsc)
        {
            return source.OrderBy(new List<QueryOrderingParameters>() {new QueryOrderingParameters() {Column = orderBy, IsAscending = isAsc}});
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, IEnumerable<QueryOrderingParameters> sortModels)
        {
            var expression = source.Expression;

            var count = 0;

            foreach (var item in sortModels)
            {
                expression = ConstructFullyNotatedOrderByExpression(source, item, count, expression);

                count++;
            }

            return count > 0 ? source.Provider.CreateQuery<T>(expression) : source;
        }

        private static Expression ConstructFullyNotatedOrderByExpression<T>(IQueryable<T> source, QueryOrderingParameters item, int count, Expression expression)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var indexOfDot = item.Column.IndexOf(".", StringComparison.Ordinal);

            MemberExpression selector;

            if (indexOfDot < 0)
                selector = Expression.PropertyOrField(parameter, item.Column);
            else
            {
                var selectorPrep = Expression.PropertyOrField(parameter, item.Column.Substring(0, indexOfDot));

                selector = Expression.PropertyOrField(selectorPrep, item.Column.Substring(indexOfDot+1));
            }

            var method = item.IsAscending
                ? (count == 0 ? "OrderBy" : "ThenBy")
                : (count == 0 ? "OrderByDescending" : "ThenByDescending");

            var resultingExpression = Expression.Call(typeof(Queryable), method,
                new [] { source.ElementType, selector.Type },
                expression, Expression.Quote(Expression.Lambda(selector, parameter)));

            return resultingExpression;
        }

        private static Expression ConstructOrderByExpression<T>(IQueryable<T> source, QueryOrderingParameters item, Expression expression)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var selector = Expression.PropertyOrField(parameter, item.Column);
            var method = item.IsAscending ? "OrderBy" : "OrderByDescending";

            var orderByExpression = Expression.Call(typeof(Queryable), method,
                new [] { source.ElementType, selector.Type },
                expression, Expression.Quote(Expression.Lambda(selector, parameter)));

            return orderByExpression;
        }
    }
}