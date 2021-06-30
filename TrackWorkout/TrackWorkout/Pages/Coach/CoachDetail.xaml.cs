using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TrackWorkout.Entitys;
using ImageCircle.Forms.Plugin.Abstractions;
using Rg.Plugins.Popup.Extensions;

namespace TrackWorkout.Pages.Coach
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CoachDetail : ContentPage
    {

        public CoachDetail()
        {
            InitializeComponent();

            NoCoachLabel.FontFamily = App.CustomBold;
            NoCoachLabel.TextColor = App.SecondaryThemeColor;
            AddButton.BackgroundColor = App.SecondaryThemeColor;
            PlusFrame.BackgroundColor = App.SecondaryThemeColor;
            PlusFrame2.BackgroundColor = App.PrimaryThemeColor;
            ButtonGridBottom.BackgroundColor = App.PrimaryThemeColor;
            RefreshingLayout.RefreshColor = App.SecondaryThemeColor;

            GetClientData();

        }

        private void WhileRefreshing(object sender, EventArgs e) //async?
        {
            RefreshClientData();
        }

        private async void RefreshClientData()
        {
            Entitys.PassID passCoachRequest = new Entitys.PassID { ID = App.userInformationApp[0].UserId.ToString() };

            string Response = await SharedClasses.WebserviceCalls.RefreshCoachList(passCoachRequest);

            if (Response == "success")
            {
                RefreshingLayout.IsRefreshing = false;
                Navigation.InsertPageBefore(new CoachDetail(), this); await Navigation.PopAsync();
            }
            else
            {
                var Pop = new Pages.PopUps.ErrorMessage("No Coaches", Response, "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
            }
        }

        public void GetClientData()
        {
            List<string> uniqueByStatus = App.CoachInformationApp.OrderBy(y => y.OrderID).Select(x => x.Status).Distinct().ToList();

            if (uniqueByStatus.Count != 0)          
            {
                NoCoachLabel.IsVisible = false;
                NoCoachAnimation.IsVisible = false;

                for (int i = 0; i < uniqueByStatus.Count; i++)
                {
                    var listWithSameStatus = App.CoachInformationApp.Where(value => value.Status == uniqueByStatus[i]).ToList();

                    string Status = listWithSameStatus.Min(status => status.Status);

                    Label dynamicLabel = new Label
                    {
                        Text = Status,
                        FontSize = 15,
                        FontFamily = App.CustomRegular,
                        HeightRequest = 20,
                        Margin = new Thickness(15, 10, 0, 0),
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.End,
                        TextColor = App.PrimaryThemeColor
                    };

                    // Create Frame
                    Frame CoachFrame = new Frame
                    {
                        HasShadow = false,
                        Padding = 0,
                        BackgroundColor = Color.Transparent,
                        Margin = new Thickness(0, 0, 0, 10),
                    };

                    gridStack.Children.Add(dynamicLabel);

                    // Create grid on this loop
                    Grid CoachContent = new Grid
                    {
                        RowSpacing = 0,
                        ColumnSpacing = 0,
                        BackgroundColor = Color.Transparent,
                        ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star }
                        }
                    };

                    // Fill the grid with Reps and Weight values
                    FillGrid(listWithSameStatus, CoachContent);

                    // Add Grid to Frame
                    CoachFrame.Content = CoachContent;

                    // Add Frame to page
                    gridStack.Children.Add(CoachFrame);
                }
            }
        }

        private void FillGrid(List<Entitys.CoachList> listWithSameStatus, Grid CoachContent)
        {
            for (int i = 0; i < listWithSameStatus.Count; i++)
            {
                var PassCoachInfo = listWithSameStatus[i];

                string CoachName = listWithSameStatus[i].Name;
                string CoachEmail = listWithSameStatus[i].Email;
                string Status = listWithSameStatus[i].Status;
                DateTime BecameCoachDate = listWithSameStatus[i].RelationshipCreateDate;
                string CoachID = listWithSameStatus[i].CoachID.ToString();

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
                    Text = CoachName,
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
                ClickGrid.TappedCallback = async (sender, args) => {
                    if (Status == "Active")
                    {
                        //This will need to go to a coach information page
                        await Navigation.PushAsync(new Pages.Coach.CoachInformation(PassCoachInfo));
                    }

                    if (Status == "Pending (Awaiting Coach Response)")
                    {
                        var InfoPop = new Pages.PopUps.InformationMessage("COACH is pending", $"{CoachName} has not yet accepted your request. You cannot view their information.", "OK");
                        await App.Current.MainPage.Navigation.PushPopupAsync(InfoPop, true);
                    }

                    if (Status == "Pending (Awaiting Client Response)")
                    {
                        Entitys.AcceptRejectCoach executeProc;
                        string headerToDisplay;
                        string messageToDisplay;

                        var QuestionPop = new Pages.PopUps.UserChoiceMessage("COACH is pending", $"Would you like to accept {CoachName} as a COACH?", "Yes", "No");
                        var answer = await QuestionPop.Show();
                        if (answer)
                        {
                            executeProc = new Entitys.AcceptRejectCoach { CoachID = PassCoachInfo.CoachID.ToString(), UserID = App.userInformationApp[0].UserId.ToString(), AcceptOrReject = "2" };
                            headerToDisplay = "ACCEPTED COACH";
                            messageToDisplay = $"You have accepted {PassCoachInfo.Name} as your Coach!";
                        }
                        else
                        {
                            executeProc = new Entitys.AcceptRejectCoach { CoachID = PassCoachInfo.CoachID.ToString(), UserID = App.userInformationApp[0].UserId.ToString(), AcceptOrReject = "3" };
                            headerToDisplay = "REJECTED COACH";
                            messageToDisplay = $"You have turned down {PassCoachInfo.Name} as a Coach.";
                        }

                        var errorPop = new Pages.PopUps.Loading("");
                        await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);

                        string Response = await SharedClasses.WebserviceCalls.HandleCoach(executeProc);

                        await App.Current.MainPage.Navigation.PopPopupAsync();

                        if (Response == "success")
                        {

                            var Pop = new Pages.PopUps.InformationMessage(headerToDisplay, messageToDisplay, "OK");
                            await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);

                            await Navigation.PushAsync(new Pages.Coach.Coach());
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

                Grid CoachContentDetail = new Grid
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
                CoachContentDetail.RowDefinitions.Add(new RowDefinition { Height = new GridLength(90, GridUnitType.Auto) });
                CoachContentDetail.Children.Add(ProfilePicFrame, 0, 0);
                CoachContentDetail.Children.Add(ButtonFrame, 1, 0);

                //Make the grid clickable
                CoachContentDetail.GestureRecognizers.Add(ClickGrid);

                //Add Row on page
                CoachContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(90, GridUnitType.Absolute) });

                //Add Content to page row
                CoachContent.Children.Add(CoachContentDetail, 0, i);

            }

        }

        private async void AddNewCoach(object sender, EventArgs e)
        {
            // go to an add new coach page or have a popup control this. 
            await Navigation.PushAsync(new Pages.Coach.AddCoach());
        }
        override protected bool OnBackButtonPressed()
        {
            Navigation.PushAsync(new HomeScreen());

            return true;
        }


    }
}