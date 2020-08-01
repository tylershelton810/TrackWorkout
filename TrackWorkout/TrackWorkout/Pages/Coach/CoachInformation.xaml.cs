using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.Coach
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CoachInformation : ContentPage
    {
        Entitys.CoachList PassCoachInfo;

        public CoachInformation(Entitys.CoachList Coach)
        {
            InitializeComponent();

            FillerBox.BackgroundColor = App.PrimaryThemeColor;
            FillerBox2.BackgroundColor = App.PrimaryThemeColor;
            ProfileFrame.BackgroundColor = App.PrimaryThemeColor;
            Note.BackgroundColor = App.PrimaryThemeColor;
            Home.BackgroundColor = App.PrimaryThemeColor;

            Home.BackgroundColor = App.SecondaryThemeColor;
            ButtonGridBottom.BackgroundColor = App.PrimaryThemeColor;
            Note.BackgroundColor = App.PrimaryThemeColor;
            CoachIconFrame.BackgroundColor = App.PrimaryThemeColor;

            NavigationPage.SetHasNavigationBar(this, false);            

            //Check if the image is a URI if not use default *FOR NOW*
            if (Coach.ProfileImage != null)
            {
                ProfilePicture.Source = Coach.ProfileImage;
            }
            else
            {
                //default image
                ProfilePicture.Source = "CoachMETransparent.png";
                ProfileFrame.BackgroundColor = App.ThemeColor4;
            }

            UserNameLabel.Text = Coach.Name;
            UserNameLabel.FontFamily = App.CustomRegular;
            CoachEmailLabel.Text = Coach.Email;
            CoachEmailLabel.FontFamily = App.CustomLight;

            PassCoachInfo = Coach;

            BackButton.Clicked += (o, e) =>
            {
                OnBackButtonPressed();
            };
        }

        private async void GoNote(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Coach.CoachNote(PassCoachInfo));
        }

        override protected bool OnBackButtonPressed()
        {
            Navigation.PushAsync(new CoachDetail());

            return true;
        }
    }
}