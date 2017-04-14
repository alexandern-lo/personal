using System;
using UIKit;
using Foundation;

namespace StudioMobile
{
	public class NSNotificationCenterSubscription : IDisposable
	{
		NSObject subscription;
		readonly NSNotificationCenter center;

		public NSNotificationCenterSubscription (NSNotificationCenter center, NSObject subscription)
		{
			Check.Argument (center, "center").NotNull ();
			Check.Argument (subscription, "subscription").NotNull ();
			this.subscription = subscription;
			this.center = center;
		}

		public void Dispose ()
		{
			if (subscription != null) {
				center.RemoveObserver (subscription);
				subscription = null;
			}
		}
	}
	
}
