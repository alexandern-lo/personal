using System.Linq;

namespace Avend.API.Model
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> NotDeleted<T>(this IQueryable<T> colleciton) where T : IDeletable
        {
            return colleciton.Where(x => !x.Deleted);
        }
    }
}