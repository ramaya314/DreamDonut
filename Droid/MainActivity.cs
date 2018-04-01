using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Mvvm;
using XLabs.Platform.Services.Geolocation;

using DreamDonut.Utilities;
using DreamDonut.Abstractions;

namespace DreamDonut.Droid
{
    [Activity(Label = "DreamDonut.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            Instance = this;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Shared.conditionals = new Conditionals();

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            //if(!Resolver.IsSet) SetIoc();

            LoadApplication(new App());
        }


    }
}
