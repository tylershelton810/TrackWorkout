using System;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddClient : ContentPage
    {
        string UserID;

        public AddClient()
        {
            InitializeComponent();

            this.BackgroundColor = App.PrimaryThemeColor;

            PublicCode.FontFamily = App.CustomLight;
            SubmitButton.FontFamily = App.CustomRegular;

            userEmailLabel.FontFamily = App.CustomBold;

            SubmitButton.BackgroundColor = App.PrimaryThemeColor;

            UserID = App.userInformationApp[0].UserId.ToString();            
        }
        private async void SubmitClicked(object sender, EventArgs e)
        {
            Entitys.ClientRequestInfo passClientRequest = new Entitys.ClientRequestInfo { CoachID = UserID, PublicCode = PublicCode.Text };

            var LoadingPop = new Pages.PopUps.Loading("Sending Request");
            await App.Current.MainPage.Navigation.PushPopupAsync(LoadingPop, true);

            (string Response, string Message) = await SharedClasses.WebserviceCalls.AddClient(passClientRequest);

            await App.Current.MainPage.Navigation.PopPopupAsync();

            if (Response == "success")
            {
                var Pop = new Pages.PopUps.InformationMessage("Success!", Message, "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                await Navigation.PushAsync(new Pages.Client.Clients());
            }
            else
            {
                var Pop = new Pages.PopUps.ErrorMessage("Error", Message, "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
            }          
        }

    }
}