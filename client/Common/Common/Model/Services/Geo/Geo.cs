using System;
using System.Collections.Generic;

namespace StudioMobile
{
	public struct MapLocation
	{
		public double Latitude;
		public double Longitude;
        public string provider;

        public MapLocation(double lat, double lon, string prov = null)
		{
			Latitude = lat;
			Longitude = lon;
            provider = prov ?? string.Empty;
		}

#if __ANDROID__
        public MapLocation(Android.Locations.Location location)
        {
            Latitude = location.Latitude;
            Longitude = location.Longitude;
            provider = location.Provider;
        }
        public Android.Locations.Location Native
        {
            get
            {
                var location = new Android.Locations.Location(provider);
                location.Latitude = Latitude;
                location.Longitude = Longitude;
                return location;
            }
        }
        public static implicit operator Android.Locations.Location(MapLocation location)
        {
            return location.Native;
        }
#endif

#if __IOS__
		public MapLocation(CoreLocation.CLLocationCoordinate2D coord)
		{
			Latitude = coord.Latitude;
			Longitude = coord.Longitude;
            provider = string.Empty;
		}
		public CoreLocation.CLLocationCoordinate2D Native {
			get { return new CoreLocation.CLLocationCoordinate2D (Latitude, Longitude); }
		}
		public static implicit operator CoreLocation.CLLocationCoordinate2D(MapLocation location)
		{
			return location.Native;
		}
#endif
    }

	public struct MapLocationSpan
	{
		public double LatitudeDelta;
		public double LongitudeDelta;
		#if __IOS__
		public MapLocationSpan(MapKit.MKCoordinateSpan span)
		{
			LatitudeDelta = span.LatitudeDelta;
			LongitudeDelta = span.LongitudeDelta;
		}
		public MapKit.MKCoordinateSpan Native {
			get { return new MapKit.MKCoordinateSpan (LatitudeDelta, LongitudeDelta); }
		}
		public static implicit operator MapKit.MKCoordinateSpan(MapLocationSpan span)
		{
			return span.Native;
		}
		#endif
	}

	public struct MapLocationRegion
	{
		public MapLocation Center;
		public MapLocationSpan Span;
		#if __IOS__
		public MapLocationRegion(MapKit.MKCoordinateRegion span)
		{
			Center = new MapLocation(span.Center);
			Span = new MapLocationSpan(span.Span);
		}
		public MapKit.MKCoordinateRegion Native {
			get { return new MapKit.MKCoordinateRegion (Center.Native, Span.Native); }
		}
		public static implicit operator MapKit.MKCoordinateRegion(MapLocationRegion region)
		{
			return region.Native;
		}
		#endif
	}

	public static class Geo 
	{
		public static MapLocationRegion Region(IEnumerable<MapLocation> locations)
		{
			var tl = new MapLocation (-90, 180);
			var br = new MapLocation (90, -180);
			foreach (var coordinate in locations) {
				// narrow the viewport bit-by-bit
				tl.Longitude = Math.Min (tl.Longitude, coordinate.Longitude);
				tl.Latitude = Math.Max (tl.Latitude, coordinate.Latitude);
				br.Longitude = Math.Max (br.Longitude, coordinate.Longitude);
				br.Latitude = Math.Min (br.Latitude, coordinate.Latitude);
			}
			var center = new MapLocation {
				// divide the range by two to get the center
				Latitude = tl.Latitude - (tl.Latitude - br.Latitude) * 0.5,
				Longitude = tl.Longitude + (br.Longitude - tl.Longitude) * 0.5
			};
			var span = new MapLocationSpan {
				// calculate the span, with 20% margin so pins aren’t on the edge
				LatitudeDelta = Math.Abs (tl.Latitude - br.Latitude) * 1.2,
				LongitudeDelta = Math.Abs (br.Longitude - tl.Longitude) * 1.2
			};
			return new MapLocationRegion { 
				Center = center, 
				Span = span 
			};
		}
	}
}

