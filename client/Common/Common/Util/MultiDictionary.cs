using System;
using System.Collections.Generic;

namespace StudioMobile
{

	public class MultiDictionary<K, V> : IEnumerable<KeyValuePair<K, IEnumerable<V>>>
	{
		readonly Dictionary<K, List<V>> content = new Dictionary<K, List<V>>();

		public IEnumerator<KeyValuePair<K, IEnumerable<V>>> GetEnumerator ()
		{			
			foreach (var kv in content) {
				yield return new KeyValuePair<K, IEnumerable<V>> (kv.Key, kv.Value);
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		public IEnumerable<V> this[K key]
		{
			get { return content [key]; }
			set { Add (key, value);	}
		}

		public bool TryGetValue(K key, out IEnumerable<V> values)
		{
			List<V> list;
			if (content.TryGetValue (key, out list)) {
				values = list;
				return true;
			} else {
				values = null;
				return false;
			}
		}

		public bool ContainsKey(K key) 
		{
			return content.ContainsKey (key);
		}

		List<V> GetBucket(K key, bool create = true)
		{
			List<V> list;
			if (!content.TryGetValue (key, out list) && create) {
				list = new List<V> ();
				content.Add (key, list);
			}
			return list;
		}

		public void Add(K key, V value)
		{
			GetBucket(key).Add (value);
		}

		public void Add(K key, IEnumerable<V> value)
		{
			content[key] = new List<V>(value); 
		}

		public void AddRange(K key, IEnumerable<V> value)
		{
			GetBucket(key).AddRange (value);
		}

		public bool Remove(K key, V value)
		{			
			var bucket = GetBucket (key, false);
			if (bucket != null) {
				var removed = bucket.Remove (value);
				if (bucket.Count == 0) {
					content.Remove (key);
				}
				return removed;
			} else {
				return false;
			}
		}

		public int RemoveAll(K key, Predicate<V> value)
		{
			var bucket = GetBucket (key, false);
			if (bucket != null) {
				var removed = bucket.RemoveAll (value);
				if (bucket.Count == 0) {
					content.Remove (key);
				}
				return removed;
			} else {
				return 0;
			}
		}

		public bool Remove(K key)
		{
			return content.Remove (key);
		}
	}
	
}
