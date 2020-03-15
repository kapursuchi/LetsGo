using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Firebase;
using Firebase.Auth;

namespace LetsGo.Droid
{
    [Activity(Label = "LetsGo", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        //private FirebaseAuth auth;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            //var options = new FirebaseOptions.Builder()
            //.SetApplicationId("1:967356054347:android:df2a4d986cc6d14ab2c6ce")
            //.SetApiKey("AIzaSyB7HjqOiov5yKez8m12xKqJjfbziE4cXig")
            //.SetDatabaseUrl("https://letsgo-f4d0d.firebaseio.com/")
            //.Build();

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            //var firebaseApp = FirebaseApp.InitializeApp(this, options);
            //auth = FirebaseAuth.getInstance();
            //FirebaseApp.InitializeApp(Application.Context);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}