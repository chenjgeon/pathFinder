using Android;
using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;

namespace pathFinder
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
        GoogleMap map;

        readonly string[] permissionGroup =
        {
            Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation
        };
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
        }
    }
}