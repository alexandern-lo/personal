using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Android.Content;
using Android.Locations;

namespace StudioMobile
{
    public class AddressGeocoder : Java.Lang.Object, IAddressGeocoder
    {
        readonly Context context;
        readonly int maxResults;

        public AddressGeocoder(Context _context, int _maxResults = 5)
        {
            context = _context;
            maxResults = _maxResults;
        }

        public async Task<List<MapAddress>> Geocode(AddressQuery query, CancellationToken token)
        {
            using (var geocoder = new Geocoder(context))
            {
                try
                {
                    var box = query.GetBoundingBox();
                    var placemarks = await geocoder.GetFromLocationNameAsync(query.Text, query.MaxResults, box[0], box[1], box[2], box[3]);
                    var result = new List<MapAddress>();
                    foreach (var place in placemarks)
                    {
                        result.Add(new MapAddress
                        {
                            City = place.Locality,
                            Country = place.CountryName,
                            Label = place.FeatureName,
                            PostalCode = place.PostalCode,
                            Region = place.CountryCode,
                            StreetAddress = place.MaxAddressLineIndex >= 1 ? place.GetAddressLine(0) : null,
                            Location = new MapLocation(place.Latitude, place.Longitude)
                        });
                    }
                    return result;
                }
                catch (Java.IO.IOException e)
                {
                    throw new GeocoderException(e.Message, e);
                }
            }
        }

        public async Task<List<MapAddress>> Geocode(MapLocation location, CancellationToken token)
        {
            using (var geocoder = new Geocoder(context))
            {
                try
                {
                    var result = new List<MapAddress>();
                    var placemarks = await geocoder.GetFromLocationAsync(location.Latitude, location.Longitude, maxResults);
                    token.ThrowIfCancellationRequested();
                    foreach (var place in placemarks)
                    {
                        result.Add(new MapAddress
                        {
                            City = place.Locality,
                            Country = place.CountryName,
                            Label = place.FeatureName,
                            PostalCode = place.PostalCode,
                            Region = place.CountryCode,
                            StreetAddress = place.MaxAddressLineIndex >= 1 ? place.GetAddressLine(0) : null,
                            Location = new MapLocation(place.Latitude, place.Longitude, location.provider)
                        });
                    }
                    return result;
                }
                catch (Java.IO.IOException e)
                {
                    throw new GeocoderException(e.Message, e);
                }
            }
        }
    }
}
