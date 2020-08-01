
using Android.App;
using Android.Content;
using Android.Content.PM;
using System;
using Android.OS;
using Lottie.Forms.Droid;
using Xamarin.Auth;
using TrackWorkout.Entitys;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Auth.Api;
using TrackWorkout.Interface;
using System.Threading.Tasks;
using Firebase.Auth;
using System.Collections.Generic;
using ImageCircle.Forms.Plugin.Droid;
using Auth0.OidcClient;
using System.IO;
using Plugin.Media;
using Plugin.CurrentActivity;

namespace TrackWorkout.Droid
{
    [Activity(Label = "coachME",
        Icon = "@drawable/Barbell",
        LaunchMode = LaunchMode.SingleInstance,
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)
        ]
    [IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "com.companyname.trackworkout",
    DataHost = "dev--28cyisb.auth0.com",
    DataPathPrefix = "/android/com.companyname.trackworkout/callback")]

    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public Auth0Client client;
        public IdentityModel.OidcClient.LoginResult loginResult;
        internal static MainActivity Instance { get; private set; }

        protected override async void OnCreate(Bundle bundle)
        {

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            AuthorizationManager.Current = new Authorization();

            Rg.Plugins.Popup.Popup.Init(this, bundle);
            base.OnCreate(bundle);
            ImageCircleRenderer.Init();
            global::Xamarin.Forms.Forms.Init(this, bundle);
            global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, bundle);

            AnimationViewRenderer.Init();

            Instance = this;
            await CrossMedia.Current.Initialize();
            CrossCurrentActivity.Current.Init(this, bundle);

            //AuthenticateUser();

            LoadApplication(new App());
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
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            Auth0.OidcClient.ActivityMediator.Instance.Send(intent.DataString);
        }
        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }
    }

}