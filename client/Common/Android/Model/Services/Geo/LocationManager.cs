using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Locations;
using ALocationManager = Android.Locations.LocationManager;

namespace StudioMobile
{
    public class LocationManager : Java.Lang.Object, ILocationManager, ILocationListener
    {
        readonly ALocationManager manager;

        string locationProvider = string.Empty;

        int minDistance;

        public LocationManager(Context context, int _minDistance)
        {
            manager = (ALocationManager)context.GetSystemService(Context.LocationService);
            minDistance = _minDistance;
            locationProvider = FindProvider();
        }

        string FindProvider()
        {
            var providers = new List<string> { ALocationManager.GpsProvider, ALocationManager.NetworkProvider, ALocationManager.PassiveProvider };
            return providers.FirstOrDefault(manager.IsProviderEnabled);
        }

        public ALocationManager Native { get { return manager; } }

        public void RequestForegroundPermissions()
        {
        }

        public void RequestBackgroundPermissions()
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                manager.Dispose();
            }
        }

        public MapLocation3D Location
        {
            get
            {
                var location = manager.GetLastKnownLocation(locationProvider);
                if (location != null)
                {
                    return new MapLocation3D(location);
                }
                else {
                    return null;
                }
            }
        }

        public Action<ILocationManager> AuthorizationChanged { get; set; }
        public LocationAuthorization AuthStatus { get { return LocationAuthorization.Granted; } }

        public Action<ILocationManager> LocationUpdated
        {
            get;
            set;
        }

        public void StartUpdatingLocation()
        {
            manager.RequestLocationUpdates(locationProvider, 0, minDistance, this);
        }

        public void StopUpdatingLocation()
        {
            manager.RemoveUpdates(this);
        }

        #region ILocationListener

        public void OnLocationChanged(Location location)
        {
            if (LocationUpdated != null)
            {
                LocationUpdated(this);
            }
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
        }

        #endregion
    }
}

