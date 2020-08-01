using System;
using TrackWorkout.Entitys;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterNavigation : ContentPage
    {
        string UserID;

        public ExerciseListRootObject exerciseListApp;

        public MasterNavigation()
        {
            InitializeComponent();

            UserID = App.userInformationApp[0].UserId.ToString();

            //Color
            Client.BackgroundColor = App.PrimaryThemeColor;
            Routine.BackgroundColor = App.PrimaryThemeColor;
            Coach.BackgroundColor = App.PrimaryThemeColor;
            Home.BackgroundColor = App.PrimaryThemeColor;
            Settings.BackgroundColor = App.PrimaryThemeColor;
            SupportUs.BackgroundColor = App.PrimaryThemeColor;

            //Font
            ClientButton.FontFamily = App.CustomRegular;
            RoutineButton.FontFamily = App.CustomRegular;
            CoachButton.FontFamily = App.CustomRegular;
            HomeButton.FontFamily = App.CustomRegular;
            SettingsButton.FontFamily = App.CustomRegular;
            SupportUsButton.FontFamily = App.CustomRegular;
        }

        private async void ClientButtonClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Client.Clients());
        }

        private async void RoutineClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Routine.Routines());
        }

        private async void CoachClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Coach.Coach());            
        }
        private async void HomeClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.HomeScreen());
            
        }

        private async void SettingsClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.SettingsMaster());
        }
        private async void SupportUsClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.SupportUs.SupportUsMaster());
        }
    }
    
}