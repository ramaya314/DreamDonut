using System;
using System.Collections.Generic;
using System.Linq;

using XLabs.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Mvvm;
using XLabs.Platform.Services.Geolocation;

using Foundation;
using UIKit;

using DreamDonut.Abstractions;
using DreamDonut.Utilities;

namespace DreamDonut.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : XFormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            // Code for starting up the Xamarin Test Cloud Agent
#if DEBUG
			Xamarin.Calabash.Start();
            #endif


            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
            UIApplication.SharedApplication.SetStatusBarHidden (false, false);

            NSDictionary userDefautls = NSDictionary.FromObjectAndKey(new NSNumber(0.5f), new NSString("font_size"));
            NSUserDefaults.StandardUserDefaults.RegisterDefaults (userDefautls);
            NSUserDefaults.StandardUserDefaults.Synchronize ();

            Shared.conditionals = new Conditionals();

            if (!Resolver.IsSet)
                SetIoc();
            
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        private void SetIoc()
        {
            var resolverContainer = new SimpleContainer();

            resolverContainer.Register<INativeOperations, NativeOperations> ();
            resolverContainer.Register<IDevice> (t => AppleDevice.CurrentDevice);
            resolverContainer.Register<IGeolocator, Geolocator>();

            var app = new XFormsAppiOS();
            app.Init(this);
            resolverContainer.Register<IXFormsApp>(app);

            Resolver.SetResolver(resolverContainer.GetResolver());
        }
    }
}
