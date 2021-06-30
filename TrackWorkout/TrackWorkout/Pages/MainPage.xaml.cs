using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using TrackWorkout.Entitys;
using Xamarin.Forms;

namespace TrackWorkout.Pages
{
    public partial class MainPage : ContentPage
    {
        List<String> AnimationTest = new List<String>();
        Boolean ShowPass;


        public MainPage()
        {
            InitializeComponent();

            ShowPass = false;

            AnimationTest.Add("Done.json");
            AnimationTest.Add("Loading1.json");
            AnimationTest.Add("Loading2.json");
            AnimationTest.Add("Loading4.json");
            AnimationTest.Add("Loading5.json");
            AnimationTest.Add("LoadingDNA.json");

            NavigationPage.SetHasNavigationBar(this, false);

            AutomaticLoginAction();
        }

        private void LoginButtonClick(object sender, EventArgs e)
        {
            AutomaticLoginAction();
        }

        private async void AutomaticLoginAction()
        {
            try
            {
                LoginButton.IsVisible = false;
                var returnValue = await AuthorizationManager.Current.Login();

                var errorPop = new Pages.PopUps.Loading("Retrieving Profile Data");
                await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);

                App.StoreLoginToken(returnValue);

                LoginAction();
            }
            catch
            {
                await App.Current.MainPage.Navigation.PopPopupAsync();
                LoginButton.IsEnabled = true;

                var errorPop = new Pages.PopUps.ErrorMessage("Error", "Something went wrong while logging in. Please try again. If the issue continues please contact coachME support", "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);
            }
        }

        private async void LoginAction()
        {
            App.ButtonPop.Play();
            var loginToken = App.loginToken;

            Entitys.UserEmail passEmail = new Entitys.UserEmail
            {
                Email = loginToken.User.FindFirst(x => x.Type == "email").Value.ToString(),
                Photo = "<Profile><CurrentPic>" + loginToken.User.FindFirst(x => x.Type == "picture").Value.ToString() + "</CurrentPic></Profile>"
            };

            string Response = await Pages.SharedClasses.WebserviceCalls.GetMasterData(passEmail);
            await App.Current.MainPage.Navigation.PopPopupAsync();

            if (Response == "new user")
            {
                App.PrimaryThemeColor = App.MyPallet[0].Hex;
                App.LoadingText = "";

                var Pop = new Pages.PopUps.InformationMessage("New User", $"The email {passEmail.Email} has not been set up with coachME. We will redirect you!", "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);

                // Go To Create a user page
                await Navigation.PushAsync(new CreateUser(passEmail, "From Google"));
            }
            else if (Response == "success")
            {
                //Go to Homescreen 
                await Navigation.PushAsync(new HomeScreen());
            }
            else
            {
                var errorPop = new Pages.PopUps.ErrorMessage("Error", Response, "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);
            }
        }

    }
}
