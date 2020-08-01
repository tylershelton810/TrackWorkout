using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Lottie.Forms.iOS.Renderers;
using TrackWorkout.Interface;
using UIKit;
using Auth0.OidcClient;

namespace TrackWorkout.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public Auth0Client client;
        public IdentityModel.OidcClient.LoginResult loginResult;
        
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Auth.Presenters.XamarinIOS.AuthenticationConfiguration.Init();
                       
            global::Xamarin.Forms.Forms.Init();
            Rg.Plugins.Popup.Popup.Init();
            AnimationViewRenderer.Init();
            ImageCircleRenderer.Init();
            AuthorizationManager.Current = new Authorization();

            //AuthenticateUser();

            LoadApplication(new App());

            //Firebase.Core.App.Configure();
            return base.FinishedLaunching(app, options);
        }
        public async void AuthenticateUser()
        {
            client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = "dev--28cyisb.auth0.com",
                ClientId = "4qhP08l7Vub3qrBCQhKu7b0mooKCSmRi"
            });

            loginResult = await client.LoginAsync();

            App.StoreLoginToken(loginResult);
        }
        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            ActivityMediator.Instance.Send(url.AbsoluteString);

            return true;
        }
    }
}
