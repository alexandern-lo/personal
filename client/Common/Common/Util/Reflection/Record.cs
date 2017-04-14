using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace StudioMobile
{	
	/// <summary>
	/// Expose objects as IDictionary[String, Object] implementations.
	/// Might be useful of you want code which works for Dictionary, Json and Data Objects.
	/// </summary>
	public struct Record : IDictionary<string, object>, ICloneable
	{
		object data;
		IKeyValueCoding kvc;

		public Record (object data)
		{			
			kvc = KeyValueCoding.Impl (data);
			this.data = data;
		}

		public Record Clone()
		{
			var copy = new Dictionary<string, object> ();
			foreach (var kv in this) {
				var value = kv.Value;
				var cloneable = value as ICloneable;
				if (cloneable != null) {
					copy [kv.Key] = cloneable.Clone ();
				} else {
					copy [kv.Key] = value;
				}
			}
			return new Record (copy);
		}

		object ICloneable.Clone()
		{
			return Clone ();
		}

		public object Data { 
			get { return data; }
			set { 
				if (ReferenceEquals (value, null)) {
					throw new ArgumentNullException ("value");
				}
				data = value;
			}
		}

		public void Set (string key, object value)
		{
			kvc.Set (data, key, value);
		}

		public bool CanWrite(string key)
		{
			return !kvc.IsReadonly (data, key);
		}

		public Type GetType(string key)
		{
			return kvc.GetKeyType (data, key);
		}

		public object Get (string index)
		{
			return kvc.Get (data, index);
		}

		public void Add (string key, object value)
		{
			Set (key, value);
		}

		public bool ContainsKey (string key)
		{			
			return kvc.ContainsKey (data, key);
		}

		public bool Remove (string key)
		{
			return kvc.Remove (data, key);
		}

		public bool TryGetValue (string key, out object value)
		{
			value = null;
			if (!ContainsKey (key)) {
				return false;
			}
			value = this [key];
			return true;
		}

		public void Add (KeyValuePair<string, object> item)
		{
			Add (item.Key, item.Value);
		}

		public void Clear ()
		{
			kvc.Clear (data);
		}

		public bool Contains (KeyValuePair<string, object> item)
		{
			if (!ContainsKey (item.Key)) {
				return false;
			}
			var value = this [item.Key];
			return object.Equals (item.Value, value);
		}

		public void CopyTo (KeyValuePair<string, object>[] array, int arrayIndex)
		{
			throw new NotImplementedException ();
		}

		public bool Remove (KeyValuePair<string, object> item)
		{
			if (kvc.ContainsKey (data, item.Key)) {
				var value = kvc.Get (data, item.Key);
				if (object.Equals (value, item.Value)) {
					kvc.Remove (data, item.Key);
					return true;
				}
			}
			return false;
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator ()
		{
			return kvc.GetEnumerator (data);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		public object this [string index] {
			get {
				return Get (index);
			}
			set {
				Set (index, value);
			}
		}

		public ICollection<string> Keys {
			get {
				return this.Select (kv => kv.Key).ToList ();
			}
		}

		public ICollection<object> Values {
			get {
				return this.Select (kv => kv.Value).ToList ();
			}
		}

		public int Count {
			get {
				return kvc.Count (data);
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}
	}
}