using Json.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TrackWorkout.Entitys;
using TrackWorkout.Pages;
using TrackWorkout.Pages.Views;
using TrackWorkout.Services;
using Xamarin.Auth;
using Xamarin.Forms;
using Auth0.OidsClient;

namespace TrackWorkout.Pages
{
    public partial class MainPage : ContentPage
    {
        List<String> AnimationTest = new List<String>();
        Boolean ShowPass;
        GoogleUser user;

        // Create the client used to login
        var client = new Auth0Client(new Auth0ClientOptions
        {
            Domain = "dev--28cyisb.auth0.com",
            ClientId = "wi3pfGGk1DQY6Cz06F8Y8lb3d50KIdzr"
        });

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
        }
        private async void FacebookLoginClick(object sender, EventArgs e)
        {
            await DisplayAlert("Facebook Login", "This feature is coming soon", "OK");
        }
        private async void TestLoginClicked(object sender, EventArgs e)
        {
            Password.Text = "password";
            logInEmail.Text = "tylershelton810@gmail.com";
            //logInEmail.Text = "Test@test.com";
            LoginAction("Email");
        }
        private async void GoogleLoginClick(object sender, EventArgs e)
        {
            App.ButtonPop.Play();

            await GoogleLogin.ScaleTo(.95, 25);
            await GoogleLogin.ScaleTo(1, 25);

            //Call the login page
            var loginResult = await client.LoginAsync();

        }
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
            LoginAction("Email");
        }

        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {            
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            user = null;
            if (e.IsAuthenticated)
            {
                // If the user is authenticated, request their basic user data from Google
                // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
                var request = new OAuth2Request("GET", new Uri(GoogleServices.UserInfoUrl), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    // Deserialize the data and store it in the account store
                    // The users email address will be used to identify data in SimpleDB
                    string userJson = await response.GetResponseTextAsync();
                    user = JsonConvert.DeserializeObject<GoogleUser>(userJson);                    
                }

                LoginAction("Google");

                //if (account != null)
                //{
                //    //store.Delete(account, GoogleServices.AppName);
                //}

                //await store.SaveAsync(account = e.Account, GoogleServices.AppName);
                //await DisplayAlert("Email address", user.Email, "OK");
            }
        }

        private async void LoginAction(string Source)
        {
            LoadingProfileIcon.IsVisible = true;
            //LoginButton.IsEnabled = false;
            CreateAccountButton.IsEnabled = false;
            App.ButtonPop.Play();

            Entitys.UserEmail passEmail = null;

            if (Source == "Google")
            {
                passEmail = new Entitys.UserEmail { Email = user.Email.ToString(), GoogleID = user.Id, Photo = user.Picture };
            }
            else if (Source == "Email")
            {
                passEmail = new Entitys.UserEmail { Email = logInEmail.Text, Password = Password.Text };
            }
            var client = new HttpClient();
            var jsonContent = JsonNet.Serialize(passEmail);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            // API call to webservice to save user in the database
            var WebCallResponse = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetUserData", httpContent);
            var responseString = await WebCallResponse.Content.ReadAsStringAsync();
            var userInformation = JsonConvert.DeserializeObject<UserInformationRootObject>(responseString);
            client.Dispose();

            if (userInformation.UserInformation[0].UserId == 0)
            {
                LoadingProfileIcon.IsVisible = false;
                //LoginButton.IsEnabled = true;
                CreateAccountButton.IsEnabled = true;
                // Go To Create a user page
                await Navigation.PushAsync(new CreateUser(passEmail, "From Google"));
            }
            else
            {
                //Get user info
                App.StoreUserInfo(userInformation.UserInformation);

                client = new HttpClient();
                jsonContent = JsonNet.Serialize(passEmail);
                httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                WebCallResponse = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetUserHistoryData", httpContent);
                responseString = await WebCallResponse.Content.ReadAsStringAsync();
                client.Dispose();
                var historyInformation = JsonConvert.DeserializeObject<HistoryRootObject>(responseString);

                //Get History Info
                App.StoreHistoryInfo(historyInformation);
                App.StoreClientHistoryInfo(historyInformation);

                Entitys.PassID passID = new Entitys.PassID { ID = App.userInformationApp[0].UserId.ToString() };

                // Use the Email object to get the rest of the data set
                client = new HttpClient();
                jsonContent = JsonNet.Serialize(passID);
                httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                WebCallResponse = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetUserRoutineList", httpContent);
                responseString = await WebCallResponse.Content.ReadAsStringAsync();
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
                WebCallResponse = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetNoteData", httpContent);
                responseString = await WebCallResponse.Content.ReadAsStringAsync();
                client.Dispose();
                var noteInformation = JsonConvert.DeserializeObject<NoteInformationRootObject>(responseString);

                App.StoreNoteInfo(noteInformation);

                // Use the Email object to get the rest of the data set
                client = new HttpClient();
                jsonContent = JsonNet.Serialize(passID);
                httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                WebCallResponse = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetClientData", httpContent);
                responseString = await WebCallResponse.Content.ReadAsStringAsync();
                client.Dispose();
                var clientInformation = JsonConvert.DeserializeObject<ClientInformationRootObject>(responseString);

                App.StoreClientInfo(clientInformation);

                // Use the Email object to get the rest of the data set
                client = new HttpClient();
                jsonContent = JsonNet.Serialize(passID);
                httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetCoachList", httpContent);
                responseString = await response.Content.ReadAsStringAsync();
                var coachListInformation = JsonConvert.DeserializeObject<CoachListRootObject>(responseString);

                App.StoreCoachInfo(coachListInformation);

                //Go to Home Screen
                await Navigation.PushAsync(new HomeScreen());
            }
        }

        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            string error = e.Message;
        }       

        private async void CreateAccountButtonClicked(object sender, EventArgs e)
        {
            App.ButtonPop.Play();
            await CreateAccountButton.ScaleTo(.95, 25);
            await CreateAccountButton.ScaleTo(1, 25);
            // Go to the create user page
            await Navigation.PushAsync(new CreateUser(null, "From Scratch"));
        }              
    }
}
