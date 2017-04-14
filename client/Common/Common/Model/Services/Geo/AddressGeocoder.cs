using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace StudioMobile
{
	public struct MapAddress 
	{
		public MapLocation Location;
		public string City;
		public string Country;
		public string Label;
		public string PostalCode;
		public string Region;
		public string StreetAddress;

		public override string ToString ()
		{
			var sb = new System.Text.StringBuilder ();
			if (!string.IsNullOrWhiteSpace (StreetAddress)) {
				sb.Append (StreetAddress);
			} else if (!string.IsNullOrWhiteSpace (Label)) {
				sb.Append (Label);
			}
			if (!string.IsNullOrWhiteSpace (City)) {
				sb.Append (", ").Append (City);
			}
			if (!string.IsNullOrWhiteSpace (Region)) {
				sb.Append (", ").Append (Region);
			}
			if (!string.IsNullOrWhiteSpace (Country)) {
				sb.Append (", ").Append (Region);
			}
			if (!string.IsNullOrWhiteSpace (PostalCode)) {
				sb.Append (", ").Append (PostalCode);
			}
			return sb.ToString ();
		}
	}

	public struct AddressQuery 
	{
		public string Text;
		public int MaxResults;
		public MapLocation Center;
		public double LatitudinalMeters;
		public double LongitudinalMeters;
	}

	public class GeocoderException : Exception {
		public GeocoderException ()
		{
		}

		public GeocoderException (string message) : base (message)
		{
		}

		public GeocoderException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}

		public GeocoderException (string message, Exception innerException) : base (message, innerException)
		{
		}
	}

	public interface IAddressGeocoder 
	{
		Task<List<MapAddress>> Geocode(AddressQuery query, CancellationToken token);
		Task<List<MapAddress>> Geocode(MapLocation location, CancellationToken token);
	}
}

#if __IOS__
namespace StudioMobile {
	using MapKit;
	using CoreLocation;
	using System.Runtime.InteropServices;
	using ObjCRuntime;

	public static class GeocoderExtensions
	{
		public static MKLocalSearchRequest GetNative (this AddressQuery query)
		{
			var center = new CLLocationCoordinate2D (query.Center.Latitude, query.Center.Longitude);
			var region = MKCoordinateRegionMakeWithDistance (center, query.LatitudinalMeters, query.LongitudinalMeters);
			return new MKLocalSearchRequest {
				NaturalLanguageQuery = query.Text,
				Region = region
			};
		}

		[DllImport (Constants.MapKitLibrary)]
		static extern MKCoordinateRegion MKCoordinateRegionMakeWithDistance (
			CLLocationCoordinate2D centerCoordinate, 
			double latitudinalMeters, 
			double longitudinalMeters);

		public static MapLocationRegion MakeRegion (this MapLocation center, double ns, double ew)
		{
			return new MapLocationRegion(MKCoordinateRegionMakeWithDistance (center, ns, ew));
		}
	}
}
#elif __ANDROID__
namespace StudioMobile {
	public static class GeocoderExtensions
	{
        public static double[] GetBoundingBox(this AddressQuery query)
        {
            return GetBoundingBox(query.Center.Latitude, query.Center.Longitude, query.LatitudinalMeters, query.LongitudinalMeters);
        }
        public static double[] GetBoundingBox(double latitude, double longitude, double latitudinalMeters, double longitudinalMeters)
        {
            double latRadian = (Math.PI / 180) * latitude;

            double degLatKm = 110.574235;
            double degLongKm = 110.572833 * Math.Cos(latRadian);
            double deltaLat = latitudinalMeters / 1000.0 / degLatKm;
            double deltaLong = longitudinalMeters / 1000.0 / degLongKm;

            double minLat = latitude - deltaLat;
            double minLong = longitude - deltaLong;
            double maxLat = latitude + deltaLat;
            double maxLong = longitude + deltaLong;

            return new double[] { minLat, minLong, maxLat, maxLong };
        }
		public static MapLocationRegion MakeRegion (this MapLocation center, double ns, double ew)
		{
			throw new NotImplementedException();
		}
	}
}
#else
namespace StudioMobile {
	public static class GeocoderExtensions
	{
		public static MapLocationRegion MakeRegion (this MapLocation center, double ns, double ew)
		{
			throw new NotImplementedException();
		}
	}
}
#endif

