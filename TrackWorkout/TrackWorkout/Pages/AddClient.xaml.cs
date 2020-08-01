using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Json.Net;
using System.Net.Http;
using TrackWorkout.Entitys;

namespace TrackWorkout.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddClient : ContentPage
    {
        string UserID;
        List<UserInformation> userInformation;

        public AddClient(List<UserInformation> passedInformation)
        {
            InitializeComponent();

            userInformation = passedInformation;

            UserID = userInformation[0].UserId.ToString();

            // Get rid of default navigation
            NavigationPage.SetHasNavigationBar(this, false);
            
        }
        private async void SubmitClicked(object sender, EventArgs e)
        {

            Entitys.ClientRequestInfo passClientRequest = new Entitys.ClientRequestInfo { CoachID = UserID, Email = ClientEmail.Text };

            var client = new HttpClient();
            var jsonContent = JsonNet.Serialize(passClientRequest);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            // API call to webservice to save user in the database
            var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/ClientRequest", httpContent);
            var responseString = await response.Content.ReadAsStringAsync();


            if (responseString == "-2")
            {
                // Display Failure Message and stay on page
                await DisplayAlert("Request is pending", $"You have already sent this client a request and are awaiting their response.", "OK");
            }
            else if (responseString == "-3")
            {
                // Display Failure Message and stay on page
                await DisplayAlert("Client has blocked you", $"You are currently blocked by this client.", "OK");
            }
            else if (responseString == "-1")
            {
                // Display Failure Message and stay on page
                await DisplayAlert("This is your email?!", $"You cannot be your own coach silly.", "OK");
            }
            else if (responseString == "-4")
            {
                // Display Failure Message and stay on page
                await DisplayAlert("Already associated with the client!", $"You are currently this clients coach.", "OK");
            }
            else if (responseString == "0")
            {
                // Display Failure Message and stay on page
                await DisplayAlert("This client does not exist", $"We do not have this email in our database.", "OK");
            }
            else if (responseString == "1")
            {
                // Display Failure Message and stay on page
                await DisplayAlert("Success!", $"Your request has been sent!", "OK");
                await Navigation.PushAsync(new Pages.Clients(userInformation));
            }
            else
            {
                // Display Failure Message and stay on page
                await DisplayAlert("Unknown Error", $"Please contact support", "OK");
            }
        }

    }
}