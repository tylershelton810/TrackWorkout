using System;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.Coach
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCoach : ContentPage
    {
        public AddCoach()
        {
            InitializeComponent();
            this.BackgroundColor = App.PrimaryThemeColor;

            SubmitButton.BackgroundColor = App.PrimaryThemeColor;
            SubmitButton.FontFamily = App.CustomRegular;
            PublicCode.FontFamily = App.CustomLight;
            userEmailLabel.FontFamily = App.CustomBold;
        }
        private async void SubmitClicked(object sender, EventArgs e)
        {
            // Save and error handle
            Entitys.ClientRequestInfo passClientRequest = new Entitys.ClientRequestInfo { CoachID = App.userInformationApp[0].UserId.ToString(), PublicCode = PublicCode.Text };

            var errorPop = new Pages.PopUps.Loading("Adding coach");
            await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);

            (string Response, string Message) = await SharedClasses.WebserviceCalls.AddCoach(passClientRequest);

            await App.Current.MainPage.Navigation.PopPopupAsync();

            if (Response == "success")
            {
                var Pop = new Pages.PopUps.InformationMessage("Success!", Message, "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);

                await Navigation.PushAsync(new Pages.Coach.Coach());
            }
            else
            {
                var Pop = new Pages.PopUps.ErrorMessage("Error", Message, "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
            }
        }
    }
}