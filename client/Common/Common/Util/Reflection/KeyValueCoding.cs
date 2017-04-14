using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Collections.Concurrent;

namespace StudioMobile
{
	/// <summary>
	/// Key Value Coding interface
	/// </summary>
	public interface IKeyValueCoding
	{
		object Get (object entry, string key);

		void Set (object entry, string key, object value);

		bool ContainsKey (object data, string key);

		bool Remove (object data, string key);

		void Clear (object data);

		IEnumerator<KeyValuePair<string, object>> GetEnumerator (object data);

		int Count (object data);

		bool IsReadonly (object data, string key);

		Type GetKeyType (object data, string key);
	}


	public static class KeyValueCoding
	{
		static readonly TypeMap<IKeyValueCoding> impls = new TypeMap<IKeyValueCoding>();

		static KeyValueCoding ()
		{
			impls.Add (typeof(object), ReflectionKeyValueCoding.Instance);
			impls.Add (typeof(IDictionary), DictionaryKeyValueCoding.Instance);
		}

		public static IKeyValueCoding Impl<T> ()
		{
			return impls.Implementation<T> ();
		}

		public static IKeyValueCoding Impl<T> (T data)
		{
			return impls.Implementation(data);
		}

		public static IKeyValueCoding Impl (Type type)
		{
			return impls.Implementation (type);
		}

		public static void Register (Type type, IKeyValueCoding impl)
		{
			impls.Add (type, impl);
		}
	}

	public class ReflectionKeyValueCoding : IKeyValueCoding
	{
		const BindingFlags ALL_PUBLIC = BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance;

		public static readonly ReflectionKeyValueCoding Instance = new ReflectionKeyValueCoding ();

		public object Get (object entry, string key)
		{
			try {
				var property = entry.GetType ().GetProperty (key, ALL_PUBLIC);
				if (property == null) {
					throw new KeyNotFoundException (key);
				}
				return GetValue (entry, property);
			} catch (TargetInvocationException e) {
				throw e.InnerException;
			}
		}

		public void Set (object entry, string key, object value)
		{
			var property = entry.GetType ().GetProperty (key, ALL_PUBLIC);
			if (property == null || !property.CanWrite) {
				throw new KeyNotFoundException (key);
			}
			try {
				property.SetValue (entry, value);
			} catch (TargetInvocationException e) {
				throw e.InnerException;
			}
		}

		public bool ContainsKey (object data, string key)
		{				
			var property = data.GetType ().GetProperty (key, ALL_PUBLIC);
			return property != null;
		}

		public bool Remove (object data, string key)
		{
			throw new NotImplementedException ();
		}

		public void Clear (object data)
		{
			throw new NotImplementedException ();
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator (object data)
		{				
			var properties = data.GetType ().GetProperties (ALL_PUBLIC);
			foreach (var property in properties) {
				yield return new KeyValuePair<string, object> (property.Name, GetValue (data, property));
			}
		}

		public int Count (object data)
		{				
			return data.GetType ().GetProperties (ALL_PUBLIC).Length;
		}

		public bool IsReadonly (object data, string key)
		{
			var property = data.GetType ().GetProperty (key, ALL_PUBLIC);
			if (property == null)
				return true;
			var method = property.SetMethod;
			if (method == null)
				return true;
			if (method.Attributes.HasFlag (MethodAttributes.Private))
				return true;
			return false;
		}

		public Type GetKeyType (object data, string key)
		{
			var property = data.GetType ().GetProperty (key, ALL_PUBLIC);
			if (property != null) {
				return property.PropertyType;
			} else {
				throw new KeyNotFoundException ();
			}
		}

		object GetValue (object data, PropertyInfo property)
		{
			try {
				return property.GetValue (data);
			} catch (TargetInvocationException e) {
				throw e.InnerException;
			}
		}
	}

	public class DictionaryKeyValueCoding : IKeyValueCoding
	{
		public static readonly IKeyValueCoding Instance = new DictionaryKeyValueCoding ();

		public object Get (object obj, string key)
		{
			var entry = (IDictionary)obj;
			if (entry.Contains (key)) {
				return entry [key];
			} else {
				throw new KeyNotFoundException (key);
			}
		}

		public void Set (object obj, string key, object value)
		{
			var entry = (IDictionary)obj;
			entry [key] = value;
		}

		public bool ContainsKey (object obj, string key)
		{
			var data = (IDictionary)obj;
			return data.Contains (key);
		}

		public bool Remove (object obj, string key)
		{
			var data = (IDictionary)obj;
			if (data.Contains (key)) {
				data.Remove (key);
				return true;
			} else {
				return false;
			}
		}

		public void Clear (object obj)
		{
			var data = (IDictionary)obj;
			data.Clear ();
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator (object obj)
		{
			var data = (IDictionary)obj;
			foreach (var kv in data) {
				var entry = (DictionaryEntry)kv;
				yield return new KeyValuePair<string, object> (entry.Key.ToString (), entry.Value);
			}
		}

		public int Count (object obj)
		{
			var data = (IDictionary)obj;
			return data.Count;
		}

		public bool IsReadonly (object obj, string key)
		{			
			var entry = (IDictionary)obj;
			return entry.IsReadOnly;
		}

		public Type GetKeyType (object obj, string key)
		{
			return typeof(object);
		}
	}
}

