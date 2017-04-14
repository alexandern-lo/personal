using System;
using System.Collections.Concurrent;

namespace StudioMobile
{
	public class TypeMap<Methods>
	{		
		readonly ConcurrentDictionary<Type, Methods> implementations = new ConcurrentDictionary<Type, Methods>();

		public Methods Implementation<T> ()
		{
			var type = typeof(T);
			return Implementation (type);
		}

		public Methods Implementation<T> (T data)
		{
			if (ReferenceEquals (null, data)) {
				throw new ArgumentNullException ("data");
			}
			return Implementation (data.GetType ());
		}

		public Methods Implementation (Type type)
		{
			Methods impl = default(Methods);
			while (type != null) {
				if (implementations.TryGetValue (type, out impl)) {
					return impl;
				}
				foreach (var intf in type.GetInterfaces ()) {
					if (implementations.TryGetValue (intf, out impl)) {
						return impl;
					}	
				}
				type = type.BaseType;
			}
			return impl;
		}

		public void Add (Type type, Methods impl)
		{
			implementations.AddOrUpdate (type, impl, (t, old) => impl);
		}
	}
}
