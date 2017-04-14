using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StudioMobile
{
    public static class CollectionExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
        {
            if (enumerableList != null)
            {
                return new ObservableCollection<T>(enumerableList);
            }
            return null;
        }

        public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> self)
        {
            return new ReadOnlyCollection<T>(self);
        }

		public static T Last<T>(this IList<T> self)
		{
			return self [self.Count - 1];
		}

		public static T First<T>(this IList<T> self)
		{
			return self [0];
		}

		public static T Pop<T>(this IList<T> self)
		{
			var result = self.Last();
			self.RemoveAt (self.Count - 1);
			return result;
		}

        public static bool IsNullOrEmpty<T>(this IReadOnlyList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static void AddIfNotNull<T>(this List<T> list, T value) where T : class
        {
            if (value != null)
                list.Add(value);
        }

        // from MoreLINQ - https://code.google.com/p/morelinq/
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static void Each<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            int index = 0;
            foreach (var element in collection)
                action(element, index++);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue defaultVal = default(TValue))
        {
            TValue result;
            bool found = dic.TryGetValue(key, out result);
            return found ? result : defaultVal;
        }

        public static bool HasSameObjects<T>(this IEnumerable<T> list, IEnumerable<T> other)
        {
            if (list == other) return true;
            if (list == null || other == null) return false;
            var listSet = new HashSet<T>(list);
            var otherSet = new HashSet<T>(other);
            return listSet.SetEquals(otherSet);
        }
    }
}
