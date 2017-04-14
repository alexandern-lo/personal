using System;

namespace StudioMobile
{
	public class MapLocation3D
	{
		public MapLocation Location;
		public double HAccuracy, VAccuracy, Altitude, Course, Speed;
		public DateTime Timestamp;

        public MapLocation3D()
		{
			Location = new MapLocation (0, 0);
			HAccuracy = VAccuracy = Altitude = Course = Speed = 0;
			Timestamp = DateTime.MinValue;
		}

#if __ANDROID__
        public MapLocation3D(Android.Locations.Location location)
        {
            Location = new MapLocation(location);
            HAccuracy = location.Accuracy;
            VAccuracy = location.Accuracy;
            Altitude = location.Altitude;
            Course = 0;
            Speed = location.Speed;
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(location.Time).UtcDateTime;
        }
        public Android.Locations.Location Native
        {
            get
            {
                var location = Location.Native;
                location.Accuracy = (float)HAccuracy;
                location.Altitude = Altitude;
                location.Speed = (float)Speed;
                location.Time = new DateTimeOffset(Timestamp).ToUnixTimeMilliseconds();
                return location;
            }
        }
        public static implicit operator Android.Locations.Location(MapLocation3D location)
        {
            return location != null ? location.Native : null;
        }
#endif

#if __IOS__
		public MapLocation3D(CoreLocation.CLLocation location)
		{
			Location = new MapLocation (location.Coordinate);
			HAccuracy = location.HorizontalAccuracy;
			VAccuracy = location.VerticalAccuracy;
			Altitude = location.Altitude;
			Course = location.Course;
			Speed = location.Speed;
			Timestamp = location.Timestamp.ToDateTime();
		}
		public CoreLocation.CLLocation Native {
			get { 
				return new CoreLocation.CLLocation (
					Location.Native,
					Altitude, HAccuracy, VAccuracy, Course, Speed, Timestamp.ToNSDate()
				); 
			}
		}
		public static implicit operator CoreLocation.CLLocation(MapLocation3D location)
		{
			return location != null ? location.Native : null;
		}
#endif
    }

	public enum LocationAuthorization { Granted, Revoked }

	public interface ILocationManager : IDisposable
	{
		void RequestForegroundPermissions();
		void RequestBackgroundPermissions();
		MapLocation3D Location { get; }
		Action<ILocationManager> AuthorizationChanged { get; set; } 

		Action<ILocationManager>  LocationUpdated {
			get;
			set;
		}

		LocationAuthorization AuthStatus { get; }

		void StartUpdatingLocation ();
		void StopUpdatingLocation();
	}
}

