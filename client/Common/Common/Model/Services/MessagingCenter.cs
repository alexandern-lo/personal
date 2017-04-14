using System;
using System.Collections.Generic;
using System.Collections;

namespace StudioMobile
{
	public class Notification : EventArgs
	{
		public Notification (string name) : this(name, null, null)
		{
		}

		public Notification (string name, object sender): this(name, sender, null)
		{
		}

		public Notification (string name, object sender, IDictionary userInfo)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			Name = name;
			Sender = sender;
			UserInfo = userInfo ?? new Hashtable();
		}
		
		public string Name { get; private set; }
		public object Sender { get; private set; }
		public IDictionary UserInfo { get; private set; }
	}

	public class MessagingCenter
	{
		public static readonly MessagingCenter Default = new MessagingCenter();

		readonly MultiDictionary<string, Handler> listeners = new MultiDictionary<string, Handler>();

		class Handler : IDisposable {
			public string Name;
			public EventHandler<Notification> Action;
			public object Sender;
			public MessagingCenter Center;

			public void Dispose ()
			{
				Center.Unsubscribe (Name, Sender, Action);
			}
		}

		public void Post(Notification notification)
		{
			IEnumerable<Handler> handlers;
			if (listeners.TryGetValue (notification.Name, out handlers)) {
				foreach (var handler in handlers) {
					if (notification.Sender == handler.Sender || handler.Sender == null)
						handler.Action (notification.Sender, notification);
				}
			}
		}

		public void Post(string name, object sender)
		{
			Post (new Notification(name, sender));
		}

		public void Post(string name, IDictionary userInfo)
		{
			Post (new Notification(name, null, userInfo));
		}

		public void Post(string name, object sender, IDictionary userInfo)
		{
			Post (new Notification(name, sender, userInfo));
		}

		public IDisposable Subscribe(string name, object sender, EventHandler<Notification> action)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			if (action == null)
				throw new ArgumentNullException ("action");
			var handler = new Handler { 
				Action = action,
				Sender = sender,
				Center = this,
				Name = name
			};
			listeners.Add (name, handler);
			return handler;
		}

		public IDisposable Subscribe(string name, EventHandler<Notification> action)
		{
			return Subscribe (name, null, action);
		}

		public void Unsubscribe(EventHandler<Notification> action)
		{
			foreach (var kv in listeners) {
				Unsubscribe (kv.Key, action);
			}
		}

		public void Unsubscribe(string name, EventHandler<Notification> action)
		{
			listeners.RemoveAll (name, h => h.Action.Equals(action));
		}

		public void Unsubscribe(string name, object sender, EventHandler<Notification> action)
		{
			listeners.RemoveAll (name, h => h.Action.Equals(action) && h.Sender == sender);
		}
	}
}

