using System;
using UIKit;
using Foundation;
using System.Collections.Generic;

namespace StudioMobile
{

	public class KVCBindingStrategy : IPropertyBindingStrategy 
	{
		public static readonly KVCBindingStrategy Instance = new KVCBindingStrategy();
		public IDisposable SubscribeToPropertyChange (IProperty property, Action<IProperty> action)
		{
			var tf = property.Owner as NSObject;
			var propertyName = property.Key;
			var key = Char.IsUpper(propertyName[0]) ? Char.ToLower(propertyName [0]) + propertyName.Substring (1) : propertyName;
			return tf.AddObserver (key, NSKeyValueObservingOptions.OldNew, change => {
				action(property);
			});
		}
	}
	
}
