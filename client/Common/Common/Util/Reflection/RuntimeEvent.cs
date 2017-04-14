using System;
using System.Reflection;
using System.Collections.Generic;

namespace StudioMobile
{
	/// <summary>
	/// Reflection based event object you can use to subscribe/unsubscribe from events unknown at runtime.
	/// </summary>
	public class RuntimeEvent
	{
		public RuntimeEvent (Type owner, string name)
		{
			if (owner == null)
				throw new ArgumentNullException (nameof(owner));
			if (name == null)
				throw new ArgumentNullException (nameof(name));
			Owner = owner;
			Name = name;
			Info = FindEvent (Owner, Name);
			if (Info == null)
				throw new InvalidOperationException (string.Format ("Event {0} not found in {1}", Name, Owner));
		}

		public static EventInfo FindEvent(Type type, string eventName)
		{
			foreach (var t in type.ParentHierarchy()) {
				var info = t.GetEvent (eventName, BindingFlags.Instance | BindingFlags.Public);
				if (info != null)
					return info;
			}
			return null;
		}

		public Type Owner  { get; private set; }

		public string Name { get; private set; }

		public EventInfo Info { get; private set; }

		public void AddEventHandler (object sender, Delegate d)
		{
            if (Owner.IsInstanceOfType(sender)) {
				Info.AddEventHandler (sender, d);
                return;
            }
            var senderType = sender.GetType();
            if (IsAssignableToGenericType(senderType, Owner))
            {
                var currentInfo = FindEvent(senderType, Name);
                currentInfo.AddEventHandler(sender, d);
                return;
            }
            throw new InvalidOperationException();
		}

		public void RemoveEventHandler (object sender, Delegate d)
		{
            if (Owner.IsInstanceOfType(sender)) {
				Info.RemoveEventHandler(sender, d);
                return;
            }
            var senderType = sender.GetType();
            if (IsAssignableToGenericType(senderType, Owner))
            {
                var currentInfo = FindEvent(senderType, Name);
                currentInfo.RemoveEventHandler(sender, d);
                return;
            }
            throw new InvalidOperationException();
		}

		public Delegate CreateDelegate(object owner, MethodInfo method)
		{
			return Delegate.CreateDelegate (Info.EventHandlerType, owner, method);
		}

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }
    }	
}
