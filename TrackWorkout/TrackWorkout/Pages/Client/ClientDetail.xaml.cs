using ImageCircle.Forms.Plugin.Abstractions;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientsDetail : ContentPage
    {
        

        public ClientsDetail()
        {
            InitializeComponent();

            NoClientLabel.FontFamily = App.CustomBold;
            NoClientLabel.TextColor = App.SecondaryThemeColor;
            PlusFrame.BackgroundColor = App.SecondaryThemeColor;
            PlusButton.BackgroundColor = App.SecondaryThemeColor;
            PlusFrame2.BackgroundColor = App.PrimaryThemeColor;
            ButtonGridBottom.BackgroundColor = App.PrimaryThemeColor;
            RefreshingLayout.RefreshColor = App.SecondaryThemeColor;

            


            //NavigationPage.SetHasNavigationBar(this, false);

            GetClientData();
        }

        private void WhileRefreshing(object sender, EventArgs e)
        {
            RefreshClientData();
        }

        private async void RefreshClientData()
        {
            Entitys.PassID passClientRequest = new Entitys.PassID { ID = App.userInformationApp[0].UserId.ToString()};

            var errorPop = new Pages.PopUps.Loading("Getting Client Data");
            await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);

            string Response = await SharedClasses.WebserviceCalls.RefreshClientList(passClientRequest);

            await App.Current.MainPage.Navigation.PopPopupAsync();

            if (Response == "success")
            {
                RefreshingLayout.IsRefreshing = false;                
                Navigation.InsertPageBefore(new ClientsDetail(), this); await Navigation.PopAsync();
            }
            else
            {
                var Pop = new Pages.PopUps.InformationMessage("No Clients", Response, "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
            }        
        }

        public void GetClientData()
        {

            List<string> uniqueByStatus = App.ClientInformationApp.OrderBy(y => y.OrderID).Select(x => x.Status).Distinct().ToList();

            if (uniqueByStatus.Count != 0)
            {
                NoClientLabel.IsVisible = false;
                NoClientAnimation.IsVisible = false;

                for (int i = 0; i < uniqueByStatus.Count; i++)
                {
                    var listWithSameStatus = App.ClientInformationApp.Where(value => value.Status == uniqueByStatus[i]).ToList();

                    string Status = listWithSameStatus.Min(status => status.Status);

                    Label dynamicLabel = new Label
                    {
                        Text = Status,
                        FontFamily = App.CustomRegular,
                        FontSize = 15,
                        HeightRequest = 20,
                        Margin = new Thickness(15, 10, 0, 0),
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.End,
                        TextColor = App.PrimaryThemeColor
                    };

                    // Create Frame
                    Frame ClientFrame = new Frame
                    {
                        HasShadow = false,
                        Padding = 0,
                        BackgroundColor = Color.Transparent,
                        Margin = new Thickness(0, 0, 0, 10),
                    };

                    gridStack.Children.Add(dynamicLabel);

                    // Create grid on this loop
                    Grid ClientContent = new Grid
                    {
                        RowSpacing = 0,
                        ColumnSpacing = 0,
                        BackgroundColor = Color.Transparent,
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
            
        }


        private void FillGrid(List<Entitys.ClientInformation> listWithSameStatus, Grid ClientContent)
        {
            for (int i = 0; i < listWithSameStatus.Count; i++)
            {
                var PassClientInfo = listWithSameStatus[i];

                string ClientName = listWithSameStatus[i].ClientName;
                string ClientEmail = listWithSameStatus[i].ClientEmail;
                double ClientWeight = listWithSameStatus[i].ClientWeight;
                string Status = listWithSameStatus[i].Status;
                string WeightType = listWithSameStatus[i].WeightType;
                DateTime ClientBirthday = listWithSameStatus[i].ClientBirthday;
                DateTime BecameClientDate = listWithSameStatus[i].BecameClientDate;
                string ClientID = listWithSameStatus[i].ClientID;

                Frame ProfilePicFrame = new Frame
                {
                    BackgroundColor = Color.White,
                    HasShadow = false,
                    CornerRadius = 40,
                    Padding = 1
                };

                // Create an Profile Picture Image Button object 
                CircleImage ProfilePicture = new CircleImage
                {
                    Source = "CoachMETransparent.png",
                };

                if (listWithSameStatus[i].ProfileImage != null)
                {
                    ProfilePicture.Source = listWithSameStatus[i].ProfileImage;
                }
                else
                {
                    //default image
                    ProfilePicture.Source = "CoachMETransparent.png";
                    ProfilePicFrame.BackgroundColor = App.ThemeColor4;
                }

                // Create a Button object 
                Label dynamicButton = new Label
                {
                    Text = ClientName,
                    FontFamily = App.CustomRegular,
                    BackgroundColor = Color.Transparent,
                    TextColor = Color.Black,
                    Margin = new Thickness(20, 0, 0, 0),
                    FontSize = 15,
                    HorizontalOptions = LayoutOptions.Start
                };


                Frame ButtonFrame = new Frame
                {
                    HasShadow = false,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center
                };

                var ClickGrid = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
                ClickGrid.TappedCallback = async (sender, args) =>
                {
                    if (Status == "Active")
                    {
                        await Navigation.PushAsync(new Pages.Client.ClientInformation(PassClientInfo));
                    }

                    if (Status == "Pending (Awaiting Client Response)")
                    {
                        var Pop = new Pages.PopUps.InformationMessage("CLIENT is pending", $"{ClientName} has not yet accepted your request. You cannot view their information.", "OK");
                        await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                    }

                    if (Status == "Pending (Awaiting Coach Response)")
                    {
                        Entitys.AcceptRejectCoach executeProc;
                        string headerToDisplay;
                        string messageToDisplay;

                        var QuestionPop = new Pages.PopUps.UserChoiceMessage("CLIENT is pending", $"Would you like to accept {ClientName} as a CLIENT?", "Yes", "No");
                        var answer = await QuestionPop.Show();
                        if (answer)
                        {
                            executeProc = new Entitys.AcceptRejectCoach { CoachID = App.userInformationApp[0].UserId.ToString(), UserID = PassClientInfo.ClientID, AcceptOrReject = "2" };
                            headerToDisplay = "ACCEPTED CLIENT";
                            messageToDisplay = $"You have taken on {PassClientInfo.ClientName} as a Client!";
                        }
                        else
                        {
                            executeProc = new Entitys.AcceptRejectCoach { CoachID = App.userInformationApp[0].UserId.ToString(), UserID = PassClientInfo.ClientID, AcceptOrReject = "3" };
                            headerToDisplay = "REJECTED CLIENT";
                            messageToDisplay = $"You have turned down {PassClientInfo.ClientName} as a Client.";
                        }

                        var errorPop = new Pages.PopUps.Loading("", false);
                        await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);

                        string Response = await SharedClasses.WebserviceCalls.HandleClient(executeProc);

                        await App.Current.MainPage.Navigation.PopPopupAsync();

                        if (Response == "success")
                        {
                            var Pop = new Pages.PopUps.InformationMessage(headerToDisplay, messageToDisplay, "OK");
                            await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);

                            await Navigation.PushAsync(new Pages.Client.Clients());
                        }
                        else
                        {
                            var Pop = new Pages.PopUps.ErrorMessage("Error", Response, "OK");
                            await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                        }
                    }
                };

                ButtonFrame.Content = dynamicButton;

                ProfilePicFrame.Content = ProfilePicture;

                Grid ClientContentDetail = new Grid
                {
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    Margin = new Thickness(10, 10, 0, 0),
                    BackgroundColor = Color.Transparent,
                    ColumnDefinitions = {
                        new ColumnDefinition { Width = 80 },
                        new ColumnDefinition { Width = GridLength.Star }
                        }
                };

                //Add content to Grid
                ClientContentDetail.RowDefinitions.Add(new RowDefinition { Height = new GridLength(90, GridUnitType.Auto) });
                ClientContentDetail.Children.Add(ProfilePicFrame, 0, 0);
                ClientContentDetail.Children.Add(ButtonFrame, 1, 0);

                //Make the grid clickable
                ClientContentDetail.GestureRecognizers.Add(ClickGrid);

                //Add Row on page
                ClientContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(90, GridUnitType.Absolute) });

                //Add Content to page row
                ClientContent.Children.Add(ClientContentDetail, 0, i);

            }

        }

        private async void AddNewClient(object sender, EventArgs e)
        {
            // Go to the create user page
            await Navigation.PushAsync(new AddClient());
        }
        override protected bool OnBackButtonPressed()
        {
            Navigation.PushAsync(new HomeScreen());

            return true;
        }
    }
}