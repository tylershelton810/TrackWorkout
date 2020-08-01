using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Json.Net;
using System.Net.Http;
using Newtonsoft.Json;
using TrackWorkout.Entitys;
//using Org.Json;
//using UIKit;
//using CoreGraphics;

namespace TrackWorkout.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientsDetail : ContentPage
    {
        string UserID;
        List<UserInformation> userInformation;

        public ClientsDetail(List<UserInformation> passedInformation)
        {
            InitializeComponent();

            userInformation = passedInformation;

            UserID = userInformation[0].UserId.ToString(); ;

            GetClientData(userInformation);
        }

        public async void GetClientData(List<UserInformation> userInformation)
        {

            Entitys.PassID passID = new Entitys.PassID { ID = UserID };

            // Use the Email object to get the rest of the data set
            var client = new HttpClient();
            var jsonContent = JsonNet.Serialize(passID);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            // API call to webservice to save user in the database
            var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetClientData", httpContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var clientInformation = JsonConvert.DeserializeObject<ClientInformationRootObject>(responseString);


            List<string> uniqueByStatus = clientInformation.ClientInformation.Select(x => x.Status).AsParallel().AsOrdered().Distinct().ToList();

            for (int i = 0; i < uniqueByStatus.Count; i++)
            {
                var listWithSameStatus = clientInformation.ClientInformation.Where(value => value.Status == uniqueByStatus[i]).ToList();

                string Status = listWithSameStatus.Min(status => status.Status);

                Label dynamicLabel = new Label
                {
                    Text = Status,
                    FontSize = 15,
                    HeightRequest = 20,
                    Margin = new Thickness(15, 10, 0, 0),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.FromHex("#5bc2dc")
                };

                // Create Frame
                Frame ClientFrame = new Frame
                {
                    HasShadow = true,
                    Padding = 0,
                    Margin = new Thickness(0, 0, 0, 10),
                };

                gridStack.Children.Add(dynamicLabel);

                // Create grid on this loop
                Grid ClientContent = new Grid
                {
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    BackgroundColor = Color.White,
                    ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star }
                        }
                };

                // Fill the grid with Reps and Weight values
                FillGrid(listWithSameStatus, ClientContent);

                // Add Grid to Frame
                ClientFrame.Content = ClientContent;

                // Add Frame to page
                gridStack.Children.Add(ClientFrame);
            }
        }


        private void FillGrid(List<Entitys.ClientInformation> listWithSameStatus, Grid ClientContent)
        {
            for (int i = 0; i < listWithSameStatus.Count; i++)
            {

                string ClientName = listWithSameStatus[i].ClientName;
                string ClientEmail = listWithSameStatus[i].ClientEmail;
                double ClientWeight = listWithSameStatus[i].ClientWeight;
                string Status = listWithSameStatus[i].Status;
                string WeightType = listWithSameStatus[i].WeightType;
                DateTime ClientBirthday = listWithSameStatus[i].ClientBirthday;
                DateTime BecameClientDate = listWithSameStatus[i].BecameClientDate;
                string ClientID = listWithSameStatus[i].ClientID;

                // Create a Button object 
                Button dynamicButton = new Button
                {
                    Text = ClientName + " (" + ClientEmail + ")",
                    BackgroundColor = Color.White,
                    TextColor = Color.Black,
                    FontSize = 15,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };

                
                Frame ButtonFrame = new Frame
                {
                    HasShadow = true,
                    BackgroundColor = Color.Black,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                
                ButtonFrame.Content = dynamicButton;

                ClientContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });

                ClientContent.Children.Add(dynamicButton, 0, i);

                if (Status == "Active")
                {
                    dynamicButton.Clicked += async (o, e) =>
                    {
                        await Navigation.PushAsync(new Pages.ClientInformation(ClientName, ClientWeight, WeightType, Status));
                    };
                }

                if (Status == "Pending")
                {
                    dynamicButton.Clicked += async (o, e) =>
                    {
                        await DisplayAlert("Client is pending", $"{ClientName} has not yet accepted your request. You cannot view their information.", "OK");
                    };
                }
            }

        }

        private async void AddNewClient(object sender, EventArgs e)
        {
            string passUserID = UserID;

            // Go to the create user page
            await Navigation.PushAsync(new AddClient(userInformation));
        }
    }
}