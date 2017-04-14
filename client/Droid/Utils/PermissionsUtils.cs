using System;
using Android.Content;
using Android.Support.V4.Content;
using Android.Content.PM;
using Android;
using Android.App;
using Android.Support.V4.App;

namespace LiveOakApp.Droid.Utils
{
    public class PermissionsUtils
    {
        public const int CAMERA_PERMISSION_REQUEST_CODE = 1;
        public const int LOCATION_PERMISSION_REQUEST_CODE = 2;
        public const int EXTERNAL_STORAGE_PERMISSION_REQUEST_CODE = 3;
        public const int CAMERA_WITH_STORAGE_PERMISSIONS_REQUEST_CODE = 4;

        public static bool HasCameraPermission(Context context) 
        {
            return ContextCompat.CheckSelfPermission(context, Manifest.Permission.Camera) == (int)Permission.Granted;
        }

        public static void RequestCameraPermission(Activity activity) 
        {
            RequestCameraPermission(activity, CAMERA_PERMISSION_REQUEST_CODE);
        }

        public static void RequestCameraPermission(Activity activity, int requestCode)
        {
            ActivityCompat.RequestPermissions(activity, new string[] { Manifest.Permission.Camera }, requestCode);
        }

        public static bool HasLocationPermission(Context context) 
        {
            return ContextCompat.CheckSelfPermission(context, Manifest.Permission.AccessCoarseLocation) == (int) Permission.Granted
                                && ContextCompat.CheckSelfPermission(context, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted;
        }

        public static void RequestLocationPermission(Activity activity) 
        {
            ActivityCompat.RequestPermissions(activity, new string[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation }, LOCATION_PERMISSION_REQUEST_CODE);
        }

        public static bool HasExternalStoragePermission(Context context) 
        {
            return ContextCompat.CheckSelfPermission(context, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted;
        }

        public static void RequestExternalStoragePermission(Activity activity) 
        {
            ActivityCompat.RequestPermissions(activity, new string[] { Manifest.Permission.WriteExternalStorage }, EXTERNAL_STORAGE_PERMISSION_REQUEST_CODE);
        }

        public static bool HasCameraAndStoragePermission(Context context)
        {
            return HasCameraPermission(context) && HasExternalStoragePermission(context);
        }

        public static void RequestCameraAndStoragePermission(Activity activity)
        {
            RequestCameraAndStoragePermission(activity, CAMERA_WITH_STORAGE_PERMISSIONS_REQUEST_CODE);
        }

        public static void RequestCameraAndStoragePermission(Activity activity, int requestCode)
        {
            ActivityCompat.RequestPermissions(activity, new string[] { Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage }, requestCode);
        }
    }
}
