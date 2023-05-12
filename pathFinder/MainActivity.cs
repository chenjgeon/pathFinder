using Android;
using Android.App;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;

namespace pathFinder
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
        GoogleMap map;
        FusedLocationProviderClient locationProviderClient;
        Android.Locations.Location myLastLocation;
        private LatLng myPosition;

        readonly string[] permissionGroup =
    {
        Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation
    };
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (grantResults.Length < 1)
            {
                return;
            }

            if (grantResults[0] == Android.Content.PM.Permission.Granted &&
                grantResults[1] == Android.Content.PM.Permission.Granted)
            {
                displayLocationAsync();
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            RequestPermissions(permissionGroup, 0);

            //references
            SupportMapFragment mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);
        }
        public void OnMapReady(GoogleMap googleMap)
        {
            var mapStyle = MapStyleOptions
                .LoadRawResourceStyle(this, Resource.Drawable.mapper);
            googleMap.SetMapStyle(mapStyle);
            map = googleMap;

            map.UiSettings.ZoomControlsEnabled = true;

            if (checkPermission())
            {
                displayLocationAsync();
            }
        }
        // needs to be fixed
        async void displayLocationAsync()
        {
            if (locationProviderClient == null)
            {
                locationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            }

            myLastLocation = await locationProviderClient.GetLastLocationAsync();

            if (myLastLocation != null)
            {
                myPosition = new LatLng(myLastLocation.Latitude, myLastLocation.Longitude);
                map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(myPosition, 15));
            }
            else
            {
                Toast.MakeText(this, "Location is null", ToastLength.Long).Show();
            }
        }
        bool checkPermission()
        {
            bool permissionGranted = false;
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, permissionGroup, 0);
            }
            else
            {
                permissionGranted = true;
            }
            return permissionGranted;
        }
    }
}