using System;
using CoreLocation;

namespace StudioMobile
{
	public class LocationManager : ILocationManager
	{
		readonly CLLocationManager manager = new CLLocationManager();

		public LocationManager() {
			manager.PausesLocationUpdatesAutomatically = true;
			manager.AuthorizationChanged += (sender, e) => {
				if (AuthorizationChanged != null) {
					AuthorizationChanged(this);
				}
			};
			manager.LocationsUpdated += (sender, e) => {
				if (LocationUpdated != null) {
					LocationUpdated(this);
				}
			};
		}

		public CLLocationManager Native { get { return manager; } }

		public void RequestForegroundPermissions ()
		{			
			manager.RequestWhenInUseAuthorization ();
		}

		public void RequestBackgroundPermissions ()
		{
			manager.RequestAlwaysAuthorization ();
		}

		public void Dispose ()
		{
			Dispose (true);
		}

		~LocationManager()
		{
			Dispose (false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing) {
				manager.Dispose();
			}
		}

		public MapLocation3D Location {
			get {
				if (manager.Location != null) {
					return new MapLocation3D (manager.Location);
				} else {
					return null;
				}
			}
		}

		public Action<ILocationManager> AuthorizationChanged { get; set; } 

		public LocationAuthorization AuthStatus { 
			get { 
				switch (CLLocationManager.Status) {
					case CLAuthorizationStatus.AuthorizedAlways:
					case CLAuthorizationStatus.AuthorizedWhenInUse:
						return LocationAuthorization.Granted;
					default:
						return LocationAuthorization.Revoked;
				}
			}
		}

		public Action<ILocationManager> LocationUpdated {
			get;
			set;
		}

		public void StartUpdatingLocation ()
		{					
			manager.StartUpdatingLocation ();
		}

		public void StopUpdatingLocation()
		{
			manager.StartUpdatingLocation ();
		}
	}
}

