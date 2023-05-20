using Android;
using Android.App;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using PathFinder.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace pathFinder
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
        GoogleMap map;
        FusedLocationProviderClient locationProviderClient;
        Android.Locations.Location myLastLocation;
        private LatLng myPosition;
        Button findDirection;

        TextView placeTextView;
        MapHelpers mapHelpers = new MapHelpers();

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
            findDirection = (Button)FindViewById(Resource.Id.getDirectionButton);

            findDirection.Click += FindDirection_Click;
            placeTextView = FindViewById<TextView>(Resource.Id.placeTextView);
        }

        private void FindDirection_Click(object sender, System.EventArgs e)
        {
            if (checkPermission())
            {
                displayLocationAsync();
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            var mapStyle = MapStyleOptions
                .LoadRawResourceStyle(this, Resource.Drawable.mapper);
            googleMap.SetMapStyle(mapStyle);
            map = googleMap;

            map.UiSettings.ZoomControlsEnabled = true;

            map.CameraMoveStarted += Map_CameraMoveStarted;
            map.CameraIdle += Map_CameraIdle;
        }
        IList<Address> addresses= new List<Address>();
        Geocoder geocoder;
        private async void Map_CameraIdle(object sender, System.EventArgs e)
        {
            var position = map.CameraPosition.Target;
            geocoder = new Geocoder(this);
            addresses = await geocoder.GetFromLocationAsync(position.Latitude, position.Longitude, 1);

            #region
            if (addresses != null && addresses.Count > 0)
            {
                Address addresss = addresses[0];
                string formattedAddress = addresss.GetAddressLine(0);
                placeTextView.Text = formattedAddress.ToUpper();
            }
           else
            {
                placeTextView.Text = "Where to?";
            }
            #endregion
        }

        private void Map_CameraMoveStarted(object sender, GoogleMap.CameraMoveStartedEventArgs e)
        {
            
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