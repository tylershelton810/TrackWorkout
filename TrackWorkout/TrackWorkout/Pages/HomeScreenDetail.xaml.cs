using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Xml.Linq;
using System.Linq;
using TrackWorkout.Entitys;
using System.Collections.Generic;
using Microcharts;
using System.Threading.Tasks;
using TrackWorkout.Pages;
using Plugin.Media;
using ImageCircle.Forms.Plugin.Abstractions;
using Rg.Plugins.Popup.Extensions;

namespace TrackWorkout
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class HomeScreenDetail : ContentPage
    {
        Size screenSize;
        XDocument WeightXML;

        public HomeScreenDetail()
        {
            InitializeComponent();

            BuildWeightGraph();

            screenSize = Device.Info.PixelScreenSize;
            PRFrame.TranslationX = ((screenSize.Width / 2) * -1);
            ProfilePicFrame.TranslationX = ((screenSize.Width / 2) * -1);
            WeightChartFrame.TranslationX = ((screenSize.Width / 2) * -1);
            NewWeightEntry.TranslationX = ((screenSize.Width / 2) * -1);
            PersonalRecordButton.Opacity = .4;
            WeightEntryButton.Opacity = .4;
            TapeButton.Opacity = .4;

            FillerBox.BackgroundColor = App.PrimaryThemeColor;
            ProfileFrame.BackgroundColor = App.PrimaryThemeColor;
            MoreInfoButton.BackgroundColor = App.PrimaryThemeColor;
            OKMembershipButton.BackgroundColor = App.PrimaryThemeColor;
            ButtonGrid.BackgroundColor = App.SecondaryThemeColor;
            ButtonGridFrame.BackgroundColor = App.SecondaryThemeColor;
            HistoryButton.BackgroundColor = App.SecondaryThemeColor;
            PersonalRecordButton.BackgroundColor = App.SecondaryThemeColor;
            GridHeaderLabel.TextColor = App.SecondaryThemeColor;
            GridHeaderLabel.FontFamily = App.CustomRegular;

            CreateProfilePicture();
            BuildProfilePic();

            if (App.userInformationApp[0].AccountType == 0)
            {
                MembershipBadge.Source = "FreeAccount.png";
                MembershipPopupLabel.Text = "Free Account";
                OKMembershipButton.Text = "Go Premium!";
            }
            else
            {
                MembershipBadge.Source = "PremiumAccount.png";
                MembershipPopupLabel.Text = "Premium Account";
                OKMembershipButton.Text = "Close";
            }

            UserNameLabel.Text = App.userInformationApp[0].Name;
            UserNameLabel.FontFamily = App.CustomRegular;
            LogoutButton.FontFamily = App.CustomLight;
            Email.Text = App.userInformationApp[0].Email;
            Email.FontFamily = App.CustomRegular;

            BuildHistory();

            BuildPersonalRecordAsync();

            NewWeightEntry.Unfocused += (o, e) =>
            {
                ConfirmWeightEntry();
            };
        }

        //private async void changeProfilePicture(object sender, EventArgs e)
        //{
        //    await CrossMedia.Current.Initialize();

        //    var QuestionPop = new Pages.PopUps.UserChoiceMessage("Change profile picture?", "Would you like to change your profile picture", "Yes", "No");
        //    var answer = await QuestionPop.Show();
        //    if (answer) // Yes
        //    {
        //        QuestionPop = new Pages.PopUps.UserChoiceMessage("Picture Source", "Where should we get a profile picture from?", "New Picture", "Previous Picture");
        //        answer = await QuestionPop.Show();
        //        if (answer)
        //        {
        //            var file = await CrossMedia.Current.PickPhotoAsync();

        //            if (file == null)
        //                return;

        //            SaveProfilePic(file.Path);

        //            ProfilePicture.Source = ImageSource.FromStream(() =>
        //            {
        //                var stream = file.GetStream();
        //                file.Dispose();
        //                return stream;
        //            });

        //            //RemoveBinding pics from stack. This is used for changing previous pics
        //            RemoveFromProfilePicStack();
        //            //Rebuild the profile pic stack
        //            BuildProfilePic();
        //        }
        //        else
        //        {
        //            if (HistoryFrame.IsVisible == true)
        //            {
        //                HistoryButton.Opacity = .4;
        //                await Task.WhenAll(
        //                    HistoryFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150),
        //                    GridHeaderLabel.ScaleTo(0, 150)
        //                    );
        //                HistoryFrame.IsVisible = false;
        //                GridHeaderLabel.IsVisible = false;

        //            }
        //            if (WeightChartFrame.IsVisible == true)
        //            {
        //                await Task.WhenAll(
        //                    GridHeaderLabel.ScaleTo(0, 150),
        //                    WeightChartFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150),
        //                    NewWeightEntry.TranslateTo((screenSize.Width / 2) * -1, 0, 150)
        //                    );
        //                WeightChartFrame.IsVisible = false;
        //                GridHeaderLabel.IsVisible = false;
        //            }
        //            if (PRFrame.IsVisible == true)
        //            {
        //                await Task.WhenAll(
        //                    PRFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150),
        //                    GridHeaderLabel.ScaleTo(0, 150)
        //                    );
        //                PRFrame.IsVisible = false;
        //                GridHeaderLabel.IsVisible = false;

        //            }

        //            GridHeaderLabel.Text = "Profile Picture Selection";
        //            GridHeaderLabel.IsVisible = true;
        //            await Task.WhenAll(
        //                ProfilePicFrame.TranslateTo(0, 0, 150),
        //                GridHeaderLabel.ScaleTo(1, 150)
        //                );

        //        }
        //    }
        //}

        private void BuildWeightGraph()
        {
            //Set up the data points for the Weight Graph            
            WeightXML = XDocument.Parse(App.userInformationApp[0].Weight);
            (List<Microcharts.Entry> weightEntries, float minWeightValue, float maxWeightValue) = Pages.SharedClasses.Build.WeightGraph(WeightXML);

            //Create the chart
            WeightChart.Chart = new LineChart()
            {
                Entries = weightEntries,
                LabelTextSize = 20,
                MinValue = minWeightValue,
                MaxValue = maxWeightValue,
            };
        }

        private async void HistoryClick(object sender, EventArgs e)
        {
            if (PRFrame.IsVisible == true)
            {
                await Task.WhenAll(
                    GridHeaderLabel.ScaleTo(0, 150),
                    PRFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150)
                );
                PRFrame.IsVisible = false;
                GridHeaderLabel.IsVisible = false;

            }
            if (WeightChartFrame.IsVisible == true)
            {
                await Task.WhenAll(
                    GridHeaderLabel.ScaleTo(0, 150),
                    WeightChartFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150),
                    NewWeightEntry.TranslateTo((screenSize.Width / 2) * -1, 0, 150)
                    );
                WeightChartFrame.IsVisible = false;
                GridHeaderLabel.IsVisible = false;
            }
            if (TapeButton.Opacity == 1)
            {

            }
            if (HistoryButton.Opacity == .4)
            {
                HistoryButton.Opacity = 1;
                PersonalRecordButton.Opacity = .4;
                WeightEntryButton.Opacity = .4;
                TapeButton.Opacity = .4;
                HistoryFrame.IsVisible = true;
                GridHeaderLabel.Text = "History";
                GridHeaderLabel.IsVisible = true;
                await Task.WhenAll(
                    HistoryFrame.TranslateTo(0, 0, 150),
                    GridHeaderLabel.ScaleTo(1, 150)
                    );
            }
            else
            {
                HistoryButton.Opacity = .4;
                await Task.WhenAll(
                    HistoryFrame.TranslateTo(((screenSize.Width / 2) * -1), 0, 150),
                    GridHeaderLabel.ScaleTo(0, 150)
                    );
                HistoryFrame.IsVisible = false;
                GridHeaderLabel.IsVisible = false;

            }
        }

        private async void PRClick(object sender, EventArgs e)
        {
            if (HistoryFrame.IsVisible == true)
            {
                HistoryButton.Opacity = .4;
                await Task.WhenAll(
                    HistoryFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150),
                    GridHeaderLabel.ScaleTo(0, 150)
                    );
                HistoryFrame.IsVisible = false;
                GridHeaderLabel.IsVisible = false;

            }
            if (WeightChartFrame.IsVisible == true)
            {
                await Task.WhenAll(
                    GridHeaderLabel.ScaleTo(0, 150),
                    WeightChartFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150),
                    NewWeightEntry.TranslateTo((screenSize.Width / 2) * -1, 0, 150)
                    );
                WeightChartFrame.IsVisible = false;
                GridHeaderLabel.IsVisible = false;
            }
            if (TapeButton.Opacity == 1)
            {
            }
            if (PersonalRecordButton.Opacity == .4)
            {
                HistoryButton.Opacity = .4;
                PersonalRecordButton.Opacity = 1;
                WeightEntryButton.Opacity = .4;
                TapeButton.Opacity = .4;
                PRFrame.IsVisible = true;
                GridHeaderLabel.Text = "Personal Records";
                GridHeaderLabel.IsVisible = true;
                await Task.WhenAll(
                    PRFrame.TranslateTo(0, 0, 150),
                    GridHeaderLabel.ScaleTo(1, 150)
                    );
            }
            else
            {
                PersonalRecordButton.Opacity = .4;
                await Task.WhenAll(
                    PRFrame.TranslateTo(((screenSize.Width / 2) * -1), 0, 150),
                    GridHeaderLabel.ScaleTo(0, 150)
                    );
                PRFrame.IsVisible = false;
                GridHeaderLabel.IsVisible = false;
            }
        }

        private async void WeightEntryClick(object sender, EventArgs e)
        {
            if (HistoryFrame.IsVisible == true)
            {
                HistoryButton.Opacity = .4;
                await Task.WhenAll(
                    HistoryFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150),
                    GridHeaderLabel.ScaleTo(0, 150)
                    );
                HistoryFrame.IsVisible = false;
                GridHeaderLabel.IsVisible = false;

            }
            if (PRFrame.IsVisible == true)
            {
                await Task.WhenAll(
                    PRFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150),
                    GridHeaderLabel.ScaleTo(0, 150)
                    );
                PRFrame.IsVisible = false;
                GridHeaderLabel.IsVisible = false;

            }
            if (TapeButton.Opacity == 1)
            {
            }
            if (WeightEntryButton.Opacity == .4)
            {
                WeightEntryButton.Opacity = 1;
                HistoryButton.Opacity = .4;
                PersonalRecordButton.Opacity = .4;
                TapeButton.Opacity = .4;
                WeightChartFrame.IsVisible = true;
                GridHeaderLabel.Text = "Weigh Ins";
                GridHeaderLabel.IsVisible = true;
                await Task.WhenAll(
                    WeightChartFrame.TranslateTo(0, 0, 150),
                    NewWeightEntry.TranslateTo(0, 0, 150),
                    GridHeaderLabel.ScaleTo(1, 150)
                    );
            }
            else
            {
                WeightEntryButton.Opacity = .4;
                await Task.WhenAll(
                    GridHeaderLabel.ScaleTo(0, 150),
                    WeightChartFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150),
                    NewWeightEntry.TranslateTo((screenSize.Width / 2) * -1, 0, 150)
                    );
            }
        }

        private async void TapeClick(object sender, EventArgs e)
        {
            if (HistoryFrame.IsVisible == true)
            {
                HistoryButton.Opacity = .4;
                await Task.WhenAll(
                    HistoryFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150),
                    GridHeaderLabel.ScaleTo(0, 150)
                    );
                HistoryFrame.IsVisible = false;
                GridHeaderLabel.IsVisible = false;

            }
            if (PRFrame.IsVisible == true)
            {
                await Task.WhenAll(
                    PRFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150),
                    GridHeaderLabel.ScaleTo(0, 150)
                    );
                PRFrame.IsVisible = false;
                GridHeaderLabel.IsVisible = false;

            }
            if (WeightChartFrame.IsVisible == true)
            {
                await Task.WhenAll(
                    GridHeaderLabel.ScaleTo(0, 150),
                    WeightChartFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150),
                    NewWeightEntry.TranslateTo((screenSize.Width / 2) * -1, 0, 150)
                    );
                WeightChartFrame.IsVisible = false;
                GridHeaderLabel.IsVisible = false;
            }
            if (TapeButton.Opacity == .4)
            {
                TapeButton.Opacity = 1;
                HistoryButton.Opacity = .4;
                PersonalRecordButton.Opacity = .4;
                WeightEntryButton.Opacity = .4;
                //PRFrame.IsVisible = true;
                //GridHeaderLabel.Text = "Personal Records";
                //GridHeaderLabel.IsVisible = true;
                //await PRFrame.TranslateTo(0, 0, 250);
            }
            else
            {
                TapeButton.Opacity = .4;
                //await PRFrame.TranslateTo(((screenSize.Width / 2) * -1), 0, 250);
                //PRFrame.IsVisible = false;
                //GridHeaderLabel.IsVisible = false;
            }
        }

        private async void ConfirmWeightEntry()
        {
            var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", $"Would you like to save {NewWeightEntry.Text} as your new weight", "Yes", "No");
            var answer = await QuestionPop.Show();
            if (answer) // Yes
            {

                if (App.userInformationApp[0].WeightType == "Lbs")
                {
                    App.userInformationApp[0].Lbs = Math.Round(float.Parse(NewWeightEntry.Text), 2);
                    App.userInformationApp[0].Kg = Math.Round((float.Parse(NewWeightEntry.Text) * 0.45359237), 2);
                }
                else
                {
                    App.userInformationApp[0].Kg = Math.Round(float.Parse(NewWeightEntry.Text), 2);
                    App.userInformationApp[0].Lbs = Math.Round((float.Parse(NewWeightEntry.Text) * 2.2045855), 2);
                }
                // save weight
                XElement newWeight = new XElement("Weight",
                                    new XElement("Lbs",
                                        new XElement("Date", DateTime.Now),
                                        new XElement("Value", App.userInformationApp[0].Lbs)
                                        ),// end Lbs
                                    new XElement("Kgs",
                                        new XElement("Date", DateTime.Now),
                                        new XElement("Value", App.userInformationApp[0].Kg)
                                        )// end Kgs)
                                    );//end Weight
                WeightXML.Element("WeightTracker").Add(newWeight);

                App.userInformationApp[0].Weight = WeightXML.ToString();

                var LoadingPop = new Pages.PopUps.Loading("Saving weigh in");
                await App.Current.MainPage.Navigation.PushPopupAsync(LoadingPop, true);

                //Save new weigt to the database
                string Response = await Pages.SharedClasses.WebserviceCalls.SaveWeighIn();

                await App.Current.MainPage.Navigation.PopPopupAsync();

                if (Response != "success")
                {
                    var Pop = new Pages.PopUps.ErrorMessage("Error", Response, "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                }

                //Animation
                await WeightChartFrame.ScaleTo(0, 150);

                //rebuild chart
                BuildWeightGraph();

                NewWeightEntry.Text = "";
                NewWeightEntry.Placeholder = "Enter Weight";

                await WeightChartFrame.ScaleTo(1, 150);
            }

        }

        private void BuildHistory()
        {
            try
            {
                if (App.historyInformationApp.Count > 0)
                {
                    int AmountToShow = 10;
                    if (App.historyInformationApp.Count < AmountToShow)
                    {
                        AmountToShow = App.historyInformationApp.Count;
                    }

                    for (int i = 0; i < AmountToShow; i++)
                    {
                        Grid HistoryContent = new Grid
                        {
                            RowSpacing = 0,
                            ColumnSpacing = 0,
                            BackgroundColor = Color.White,
                            Margin = new Thickness(5, 0, 5, 10),
                            ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star },
                        }
                        };

                        Label RoutineName = new Label
                        {
                            Text = App.historyInformationApp[i].Name,
                            FontSize = 12,
                            FontFamily = App.CustomRegular,
                            FontAttributes = FontAttributes.Bold,
                            HeightRequest = 20,
                            Margin = new Thickness(0, 0, 0, 5),
                            BackgroundColor = Color.FromHex("#f5f5f5"),
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            TextColor = App.PrimaryThemeColor
                        };

                        String notesToInsert = App.historyInformationApp[i].Notes;

                        if (notesToInsert == null)
                        {
                            notesToInsert = "Notes: N/A";
                        }

                        else
                        {
                            notesToInsert = "Notes: " + notesToInsert;
                        }

                        Frame NotesFrame = new Frame
                        {
                            HasShadow = false,
                            Padding = 0,
                        };

                        HistoryInformation CurrentHistoryInfo = App.historyInformationApp[i];

                        var ClickGrid = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
                        ClickGrid.TappedCallback = async (sender, args) =>
                        {
                            var QuestionPop = new Pages.PopUps.UserChoiceMessage("View Summary?", "Would you like to view the summary for " + RoutineName.Text, "Yes", "No");
                            var answer = await QuestionPop.Show();
                            if (answer) // Yes
                            {
                                await Navigation.PushAsync(new Pages.History.Summary(CurrentHistoryInfo));
                            }
                        };

                        Grid HistoryContentDetail = new Grid
                        {
                            RowSpacing = 0,
                            ColumnSpacing = 0,
                            BackgroundColor = Color.White,
                            ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = 100 }
                            }
                        };

                        HistoryContentDetail.GestureRecognizers.Add(ClickGrid);

                        Label Notes = new Label
                        {
                            Text = notesToInsert,
                            FontSize = 10,
                            FontFamily = App.CustomRegular,
                            Margin = new Thickness(10, 5, 10, 0),
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.End,
                        };

                        XDocument historyXML = XDocument.Parse(App.historyInformationApp[i].HistoryXML);
                        (Grid ExerciseContentDetail, double totalWeight) = Pages.SharedClasses.Build.RoutineGrids(historyXML);

                        Grid WeightSummary = new Grid
                        {
                            Margin = new Thickness(10, 0, 0, 5),
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.End,
                            ColumnDefinitions = {
                                new ColumnDefinition{ Width = GridLength.Auto},
                                new ColumnDefinition{ Width = GridLength.Auto}
                            }
                        };

                        Label TotalWeightLabel = new Label
                        {
                            Text = "Total Weight:",
                            FontFamily = App.CustomBold,
                            FontSize = 10,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.End,
                        };

                        Label TotalWeightMessage = new Label
                        {
                            Text = totalWeight.ToString("#,###.##") + " " + App.historyInformationApp[i].WeightType,
                            FontFamily = App.CustomRegular,
                            FontSize = 10,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.End,
                        };

                        WeightSummary.Children.Add(TotalWeightLabel, 0, 0);
                        WeightSummary.Children.Add(TotalWeightMessage, 1, 0);

                        Label WorkoutDate = new Label
                        {
                            Text = App.historyInformationApp[i].CompleteDate.ToShortDateString(),
                            FontFamily = App.CustomRegular,
                            FontSize = 10,
                            Margin = new Thickness(0, 0, 10, 5),
                            HorizontalOptions = LayoutOptions.End,
                            VerticalOptions = LayoutOptions.End,
                        };

                        NotesFrame.Content = HistoryContentDetail;

                        HistoryContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                        HistoryContent.Children.Add(RoutineName, 0, 0);

                        HistoryContentDetail.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                        HistoryContentDetail.Children.Add(Notes, 0, 0);
                        Grid.SetColumnSpan(Notes, 2);

                        HistoryContentDetail.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Star) });
                        HistoryContentDetail.Children.Add(ExerciseContentDetail, 0, 1);
                        Grid.SetColumnSpan(ExerciseContentDetail, 2);

                        HistoryContentDetail.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                        HistoryContentDetail.Children.Add(WeightSummary, 0, 2);

                        HistoryContentDetail.Children.Add(WorkoutDate, 1, 2);

                        HistoryContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                        HistoryContent.Children.Add(NotesFrame, 0, 1);

                        HistoryStack.Children.Add(HistoryContent);
                    }
                }
                else
                {
                    // Display Failure Message and stay on page
                    Label NoWorkoutHistory = new Label
                    {
                        Text = "There is currently no routine history to show. Complete routines to use this grid.",
                        FontSize = 15,
                        Margin = new Thickness(15, 0, 0, 0),
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = App.PrimaryThemeColor
                    };

                    HistoryStack.Children.Add(NoWorkoutHistory);
                }
            }
            catch
            {
                // Display Failure Message and stay on page
                Label NoWorkoutHistory = new Label
                {
                    Text = "There is currently no routine history to show. Complete routines to use this grid.",
                    FontSize = 15,
                    Margin = new Thickness(15, 0, 0, 0),
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = App.PrimaryThemeColor
                };

                HistoryStack.Children.Add(NoWorkoutHistory);
            }

        }

        private void BuildPersonalRecordAsync()
        {
            XDocument PRInfo = XDocument.Parse(App.userInformationApp[0].PRData);
            try
            {
                int counter;

                if (PRInfo.Element("PRInfo").Elements("PR").Count() < 10)
                {
                    counter = PRInfo.Element("PRInfo").Elements("PR").Count();
                }
                else
                {
                    counter = 10;
                }

                string Weight;

                if (PRInfo.Element("PRInfo").Element("PR").Element("WeightType").Value == "Lbs")
                {
                    Weight = "Lbs";
                }
                else
                {
                    Weight = "Kgs";
                }

                var query =
                    from objPR in PRInfo.Element("PRInfo").Descendants("PR")
                    let amountOfWeight = (Int32)objPR.Element(Weight)
                    orderby amountOfWeight descending
                    select objPR;

                if (PRInfo.Element("PRInfo").Elements("PR").Count() == 0)
                {
                    // Display Failure Message and stay on page
                    Label NoPR = new Label
                    {
                        Text = "No PERSONAL RECORDS have been recorded. Complete ROUTINES to see your accomplishments here.",
                        FontSize = 12,
                        FontFamily = App.CustomRegular,
                        Margin = new Thickness(15, 0, 0, 0),
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = App.PrimaryThemeColor
                    };

                    PRStack.Children.Add(NoPR);
                }
                else
                {
                    int loopCount = 0;

                    foreach (var PR in query.ToList())
                    {
                        Grid PRContent = Pages.SharedClasses.Build.PersonalRecord(PR, loopCount, counter, Weight);

                        PRStack.Children.Add(PRContent);

                        loopCount = loopCount + 1;
                    }
                }
            }
            catch
            {
                // Display Failure Message and stay on page
                Label NoPR = new Label
                {
                    Text = "No PERSONAL RECORDS have been recorded. Complete ROUTINES to see your accomplishments here.",
                    FontSize = 15,
                    Margin = new Thickness(15, 0, 0, 0),
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = App.PrimaryThemeColor
                };

                PRStack.Children.Add(NoPR);
            }
        }

        private async void LogoutClick(object sender, EventArgs e)
        {
            var QuestionPop = new Pages.PopUps.UserChoiceMessage("Logout", "Are you sure you want to logout ? ", "Yes", "No");
            var answer = await QuestionPop.Show();
            if (answer)
            {
                var returnLogout = await AuthorizationManager.Current.Logout();
                await Navigation.PushAsync(new MainPage());
            }
        }

        private void BuildProfilePic()
        {
            if (App.userInformationApp[0].ProfilePicture != null)
            {
                XDocument storedPictures = XDocument.Parse(App.userInformationApp[0].ProfilePicture);
                foreach (var pic in storedPictures.Element("Profile").Elements("StoredPic"))
                {
                    var path = (string)pic.Value;
                    var doesPicExist = System.IO.File.Exists(path);

                    if ((doesPicExist == true || path.StartsWith("https://lh3.google")) && !ProfilePicture.Source.ToString().Contains(path))
                    {
                        CircleImage profilePicture = new CircleImage
                        {
                            HeightRequest = 150,
                            WidthRequest = 150
                        };

                        Frame profilePictureFrame = new Frame
                        {
                            CornerRadius = 75,
                            Padding = 1,
                            WidthRequest = 150,
                            HeightRequest = 150,
                            HasShadow = false,
                            HorizontalOptions = LayoutOptions.Center
                        };

                        profilePictureFrame.Content = profilePicture;

                        if (path.StartsWith("https://lh3.google"))
                        {
                            profilePicture.Source = new UriImageSource { Uri = new Uri(path) };
                        }
                        else
                        {
                            profilePicture.Source = ImageSource.FromFile(path);
                        }

                        var profilePictureClick = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
                        profilePictureClick.TappedCallback = async (sender, args) =>
                        {
                            var QuestionPop = new Pages.PopUps.UserChoiceMessage("Choose Picture?", "Would you like this to be your new profile picture?", "Yes", "No");
                            var answer = await QuestionPop.Show();
                            if (answer) // Yes
                            {
                                App.userInformationApp[0].ProfileImage = null;
                                XDocument changeCurrentPic = XDocument.Parse(App.userInformationApp[0].ProfilePicture);
                                string storeCurrentProfilePic = changeCurrentPic.Element("Profile").Element("CurrentPic").Value;
                                changeCurrentPic.Element("Profile").Element("CurrentPic").Value = pic.Value;
                                changeCurrentPic.Element("Profile").Add(new XElement("StoredPic", storeCurrentProfilePic));

                                App.userInformationApp[0].ProfilePicture = changeCurrentPic.ToString();

                                //Set the profile picture
                                CreateProfilePicture();

                                await GridHeaderLabel.ScaleTo(0, 150);

                                //Hide the grid and bring the History Grid into focus
                                await ProfilePicFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150); //added await?
                                HistoryButton.Opacity = 1;
                                PersonalRecordButton.Opacity = .4;
                                WeightEntryButton.Opacity = .4;
                                TapeButton.Opacity = .4;
                                HistoryFrame.IsVisible = true;
                                GridHeaderLabel.Text = "History";
                                GridHeaderLabel.IsVisible = true;
                                await Task.WhenAll(
                                    HistoryFrame.TranslateTo(0, 0, 150),
                                    GridHeaderLabel.ScaleTo(1, 150)
                                    );

                                SaveProfilePic(pic.Value);

                                //RemoveBinding pics from stack. This is used for changing previous pics
                                RemoveFromProfilePicStack();
                                //Rebuild the profile pic stack
                                BuildProfilePic();
                            }
                        };

                        profilePictureFrame.GestureRecognizers.Add(profilePictureClick);

                        ProfilePicStack.Children.Add(profilePictureFrame);
                    }
                }
            }
        }

        private async void SaveProfilePic(string path)
        {
            if (App.userInformationApp[0].ProfilePicture == null)
            {
                App.userInformationApp[0].ProfilePicture = "<Profile><CurrentPic>" + path + "</CurrentPic></Profile>";
            }
            else
            {
                XDocument managePics = XDocument.Parse(App.userInformationApp[0].ProfilePicture);
                List<string> storedPics = new List<string>();

                foreach (var node in managePics.Element("Profile").Elements())
                {
                    if (node.Value != path)
                    {
                        storedPics.Add(node.Value);
                    }
                }

                managePics.Element("Profile").Element("CurrentPic").Value = path;
                try
                {
                    managePics.Element("Profile").Elements("StoredPic").Remove();
                }
                catch
                {
                    // Do Nothing if there are no stored pics
                }

                foreach (var item in storedPics)
                {
                    XElement newEl = new XElement("StoredPic", item.ToString());
                    managePics.Element("Profile").Add(newEl);
                }

                App.userInformationApp[0].ProfilePicture = managePics.ToString();
            }

            var LoadingPop = new Pages.PopUps.Loading("Saving profile picture");
            await App.Current.MainPage.Navigation.PushPopupAsync(LoadingPop, true);

            string Response = await Pages.SharedClasses.WebserviceCalls.SaveProfilePicture();

            await App.Current.MainPage.Navigation.PopPopupAsync();

            if (Response != "success")
            {
                var Pop = new Pages.PopUps.ErrorMessage("Error", Response, "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
            }
        }

        private void RemoveFromProfilePicStack()
        {
            foreach (var valueProperty in ProfilePicStack.Children.ToList())
            {
                if (valueProperty.GetType() == typeof(Frame))
                {
                    ProfilePicStack.Children.Remove(valueProperty);
                }
            }
        }

        private void CreateProfilePicture()
        {
            XDocument ProfilePic = new XDocument();
            try
            {
                ProfilePic = XDocument.Parse(App.userInformationApp[0].ProfilePicture);
            }
            catch
            {
                // Do nothing
            }
            //Check if the image is a URI if not use default *FOR NOW*
            if (App.userInformationApp[0].ProfileImage != null)
            {
                ProfilePicture.Source = App.userInformationApp[0].ProfileImage;
            }
            else
            {
                try
                {
                    if (ProfilePic.Element("Profile").Element("CurrentPic").Value != null)
                    {
                        try
                        {
                            var path = (string)ProfilePic.Element("Profile").Element("CurrentPic").Value;
                            var doesPicExist = System.IO.File.Exists(path);

                            //if the picture set as the current does not exist loop through removing all that don't
                            //and get the to one that does. This means they deleted it from there app folder
                            //or reinstalled the app making the folder and DB out of sync. 
                            if (doesPicExist == false)
                            {
                                List<string> picsToRemove = new List<string>();
                                foreach (var pic in ProfilePic.Element("Profile").Elements())
                                {
                                    path = (string)pic.Value;
                                    doesPicExist = System.IO.File.Exists(path);
                                    if (doesPicExist == false && !path.StartsWith("http"))
                                    {
                                        picsToRemove.Add(path);
                                    }
                                }
                                foreach (var item in picsToRemove)
                                {
                                    ProfilePic.Element("Profile").Elements().Where(x => x.Value == item).Remove();
                                }
                            }

                            path = (string)ProfilePic.Element("Profile").Elements().First().Value;

                            if (path.StartsWith("https://lh3.google"))
                            {
                                App.userInformationApp[0].ProfileImage = new UriImageSource { Uri = new Uri(path) };
                                ProfilePicture.Source = App.userInformationApp[0].ProfileImage;
                            }
                            else
                            {
                                ProfilePicture.Source = ImageSource.FromFile(path);
                            }
                        }
                        catch
                        {
                            //default image
                            ProfilePicture.Source = "CoachMETransparent.png";
                            ProfileFrame.BackgroundColor = App.ThemeColor4;
                        }
                    }
                }
                catch
                {
                    //default image
                    ProfilePicture.Source = "CoachMETransparent.png";
                    ProfileFrame.BackgroundColor = App.ThemeColor4;
                }
            }
        }

        private void dismissPopUp(object sender, EventArgs e)
        {
            if (MembershipPopup.IsVisible == true)
            {
                MembershipPopup.IsVisible = false;
            }
        }

        private void MembershipBadgeClick(object sender, EventArgs e)
        {
            App.ButtonPop.Play();
            MembershipPopup.IsVisible = true;
        }

        private void CloseMembershipPopup(object sender, EventArgs e)
        {
            App.ButtonPop.Play();
            MembershipPopup.IsVisible = false;
        }

        override protected bool OnBackButtonPressed()
        {
            return true;
        }

    }
}