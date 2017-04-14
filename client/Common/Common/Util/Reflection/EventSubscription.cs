using System;
using System.Reflection;

namespace StudioMobile
{
	public class EventSubscription : IDisposable
	{
		public EventSubscription (RuntimeEvent @event, Delegate action, object sender)
		{
			Check.Argument(@event, "event").NotNull();
			Check.Argument(action, "action").NotNull();
			Check.Argument(sender, "sender").NotNull();
			Event = @event;
			Action = action;
			Sender = sender;
			@event.AddEventHandler(sender, action);
		}	

		public RuntimeEvent Event { get; private set; }
		public Delegate Action { get; private set; }
		public object Sender { get; private set; }

		public void Dispose ()
		{
			if (Event != null) {
				Event.RemoveEventHandler (Sender, Action);
				Event = null;
			}
		}
	}
	
}