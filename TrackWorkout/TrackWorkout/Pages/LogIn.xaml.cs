using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Json.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using Newtonsoft.Json;
using TrackWorkout.Entitys;
using TrackWorkout.Pages.Views;

namespace TrackWorkout.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogIn : ContentPage
    {
        Boolean ShowPass;
        public LogIn()
        {
            InitializeComponent();

            ShowPass = false;

            NavigationPage.SetHasNavigationBar(this, false);
        }

        // Handle the Show password button press
        private void ShowPasswordClick(object sender, EventArgs e)
        {
            //Returns focus back to password entry. This was an attempt to keep the keyboard up but sometimes it flashes down then back up
            //Another issue is this sometimes sets the focus to the beggining of the password when I want it to be at the end
            Password.Focus();

            // Flips the icon and if the password is visible of not
            if (ShowPass == false)
            {
                ShowPassword.Source = "OpenEye.png";
                Password.IsPassword = false;
                ShowPass = true;
            }
            else
            {
                ShowPassword.Source = "ClosedEye.png";
                Password.IsPassword = true;
                ShowPass = false;
            }            
        }

        private async void LoginButtonClicked(object sender, EventArgs e)
        {            
            LoadingProfileIcon.IsVisible = true;
            LoginButton.IsEnabled = false;
            logInEmail.IsEnabled = false;
            Password.IsEnabled = false;
            App.ButtonPop.Play();


            Entitys.UserEmail passEmail = new Entitys.UserEmail { Email = logInEmail.Text, Password = Password .Text};

            var client = new HttpClient();
            var jsonContent = JsonNet.Serialize(passEmail);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            // API call to webservice to save user in the database
            var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetUserData", httpContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var userInformation = JsonConvert.DeserializeObject<UserInformationRootObject>(responseString);
            client.Dispose();


            if (userInformation.UserInformation[0].UserId == 0)
            {
                LoadingProfileIcon.IsVisible = false;
                LoginButton.IsEnabled = true;
                logInEmail.IsEnabled = true;
                Password.IsEnabled = true;
                // Display Failure Message and stay on page
                await DisplayAlert("Invalid Entry", userInformation.UserInformation[0].Name, "OK");
            }
            else
            {
                //Get user info
                App.StoreUserInfo(userInformation.UserInformation);

                client = new HttpClient();
                jsonContent = JsonNet.Serialize(passEmail);
                httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetUserHistoryData", httpContent);
                responseString = await response.Content.ReadAsStringAsync();
                client.Dispose();
                var historyInformation = JsonConvert.DeserializeObject<HistoryRootObject>(responseString);

                //Get History Info
                App.StoreHistoryInfo(historyInformation);

                Entitys.PassID passID = new Entitys.PassID { ID = App.userInformationApp[0].UserId.ToString() };               

                // Use the Email object to get the rest of the data set
                 client = new HttpClient();
                 jsonContent = JsonNet.Serialize(passID);
                 httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                 response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetUserRoutineList", httpContent);
                 responseString = await response.Content.ReadAsStringAsync();
                client.Dispose();
                var routineList = JsonConvert.DeserializeObject<RoutineListRootObject>(responseString);

                //Get Routine Info
                App.StoreRoutineInfoCoach(routineList);
                App.StoreRoutineInfoUser(routineList);
                App.StoreRoutineInfoClient(routineList);

                // Use the Email object to get the rest of the data set
                client = new HttpClient();
                 jsonContent = JsonNet.Serialize(passID);
                 httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                 response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetNoteData", httpContent);
                 responseString = await response.Content.ReadAsStringAsync();
                client.Dispose();

                var noteInformation = JsonConvert.DeserializeObject<NoteInformationRootObject>(responseString);

                App.StoreNoteInfo(noteInformation);

                // Use the Email object to get the rest of the data set
                client = new HttpClient();
                jsonContent = JsonNet.Serialize(passID);
                httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetClientData", httpContent);
                responseString = await response.Content.ReadAsStringAsync();
                client.Dispose();
                var clientInformation = JsonConvert.DeserializeObject<ClientInformationRootObject>(responseString);

                App.StoreClientInfo(clientInformation);

                //Go to Home Screen
                await Navigation.PushAsync(new HomeScreen());
            }
        }

        private void LogInEmail_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}