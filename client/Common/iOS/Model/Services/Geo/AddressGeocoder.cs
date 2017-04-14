using MapKit;
using CoreLocation;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Foundation;

namespace StudioMobile
{
	public class AddressGeocoder : IAddressGeocoder
	{
		public async Task<List<MapAddress>> Geocode (AddressQuery query, CancellationToken token)
		{
			var searchRequest = query.GetNative ();
			using (var localSearch = new MKLocalSearch (searchRequest)) {
				try {
					var response = await localSearch.StartAsync (token);
					var result = new List<MapAddress> ();
					if (response != null) {
						foreach (var item in response.MapItems) {
							var place = item.Placemark;
							result.Add (new MapAddress {
								City = place.Locality,
								Country = place.Country,
								Label = place.Name,
								PostalCode = place.PostalCode,
								Region = place.IsoCountryCode,
								StreetAddress = place.Thoroughfare,
								Location = new MapLocation (item.Placemark.Location.Coordinate)
							});
						}
					}
					return result;
				} catch (NSErrorException e) {
					throw new GeocoderException (e.Message, e);
				}
			}
		}

		public async Task<List<MapAddress>> Geocode (MapLocation location, CancellationToken token)
		{
			using (var geocoder = new CLGeocoder ())
			using (var clLocation = new CLLocation(location.Latitude, location.Longitude)) {
				try {
					var result = new List<MapAddress> ();
					var placemarks = await geocoder.ReverseGeocodeLocationAsync (clLocation);
					token.ThrowIfCancellationRequested ();
					foreach (var place in placemarks) {
						result.Add (new MapAddress {
							City = place.Locality,
							Country = place.Country,
							Label = place.Name,
							PostalCode = place.PostalCode,
							Region = place.IsoCountryCode,
							StreetAddress = place.Name,
							Location = new MapLocation (place.Location.Coordinate)
						});
					}
					return result;
				} catch (NSErrorException e) {
					throw new GeocoderException (e.Message, e);
				}

			}
		}
	}

	public class MapAddressAnnotation : NSObject, IMKAnnotation 
	{
		CLLocationCoordinate2D coordinate;
		MapAddress? address;
		public virtual MapAddress? Address { 
			get { return address; }
			set { 
				if (!value.HasValue && address.HasValue) {
					coordinate = address.Value.Location.Native;
				}
				address = value;
			}
		}

		public virtual CLLocationCoordinate2D Coordinate {
			get {
				return address.HasValue ? Address.Value.Location.Native : coordinate;
			}
		}

		[Export("title")]
		public virtual NSString GetTitle()
		{
			return Address.HasValue ? new NSString(Address.Value.Label) : new NSString("Loading");
		}

		[Export("subtitle")]
		public virtual NSString GetSubtitle()
		{
			return Address.HasValue && Address.Value.City != null ? new NSString(Address.Value.City) : NSString.Empty;
		}

		[Export("setCoordinate:")]
		public virtual void SetCoordinate(CLLocationCoordinate2D coord)
		{
			if (Address.HasValue) {
				var address = Address.Value;
				address.Location = new MapLocation (coord);
				Address = address;
			} else {
				coordinate = coord;
			}
		}
	}
}

