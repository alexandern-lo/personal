using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace StudioMobile
{
	public class LazyIList<T> : IList<T>, IList
	{
		IEnumerable<T> collection;
		List<T> list;

		public LazyIList (IEnumerable<T> collection)
		{
			if (collection == null)
				throw new ArgumentNullException ("collection");
			this.collection = collection;
		}

		IList<T> List { 
			get { 
				if (list == null)
					list = new List<T> (collection);
				return list;
			} 
		}

		public void Clear ()
		{
			collection = Enumerable.Empty<T> ();
			list = null;
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public IEnumerator<T> GetEnumerator ()
		{
			return collection.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		public int Count {
			get { return List.Count; }
		}

		public T this [int index] {
			get { return List [index]; }
			set { List [index] = value; }
		}

		public void RemoveAt (int index)
		{
			List.RemoveAt (index);
		}

		public int IndexOf (T item)
		{
			int idx = 0;
			foreach (var v in collection) {				
				if (Object.Equals (item, v))
					return idx;
				idx++;
			}
			return -1;
		}

		public void Insert (int index, T item)
		{
			List.Insert (index, item);
		}

		public void Add (T item)
		{
			List.Add (item);
		}

		public bool Contains (T item)
		{
			return IndexOf (item) != -1;
		}

		public void CopyTo (T[] array, int arrayIndex)
		{
			List.CopyTo (array, arrayIndex);
		}

		public bool Remove (T item)
		{
			return List.Remove (item);
		}

		#region IList

		int IList.Add (object value)
		{
			Add ((T)value);
			return List.Count;
		}

		bool IList.Contains (object value)
		{
			return Contains ((T)value);
		}

		int IList.IndexOf (object value)
		{
			return IndexOf ((T)value);
		}

		void IList.Insert (int index, object value)
		{
			Insert (index, (T)value);
		}

		void IList.Remove (object value)
		{
			Remove ((T)value);
		}

		bool IList.IsFixedSize {
			get { return true; }
		}

		void ICollection.CopyTo (Array array, int index)
		{
			CopyTo ((T[])array, index);
		}

		object ICollection.SyncRoot {
			get { return this; }
		}

		bool ICollection.IsSynchronized {
			get { return false; }
		}

		object IList.this[int idx] {
			get { return this [idx]; }
			set { this [idx] = (T)value; }
		}

		#endregion

	}
	
}
