using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microcharts;
using TrackWorkout.Entitys;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientInformation : ContentPage
    {
        Entitys.ClientInformation PassClientInfo;
        Size screenSize;
        XDocument WeightXML;

        public ClientInformation(Entitys.ClientInformation Client)
        {
            InitializeComponent();

            PassClientInfo = Client;

            BuildWeightGraph();

            screenSize = Device.Info.PixelScreenSize;
            PRFrame.TranslationX = ((screenSize.Width / 2) * -1);
            ProfilePicFrame.TranslationX = ((screenSize.Width / 2) * -1);
            WeightChartFrame.TranslationX = ((screenSize.Width / 2) * -1);

            PersonalRecordButton.Opacity = .4;
            WeightEntryButton.Opacity = .4;
            TapeButton.Opacity = .4;


            ProfileFrame.BackgroundColor = App.PrimaryThemeColor;
            ButtonGrid.BackgroundColor = App.SecondaryThemeColor;
            ButtonGridFrame.BackgroundColor = App.SecondaryThemeColor;
            HistoryButton.BackgroundColor = App.SecondaryThemeColor;
            PersonalRecordButton.BackgroundColor = App.SecondaryThemeColor;
            GridHeaderLabel.TextColor = App.SecondaryThemeColor;
            GridHeaderLabel.FontFamily = App.CustomRegular;

            ProfileBox.BackgroundColor = App.PrimaryThemeColor;
            ProfileFrame.BackgroundColor = App.PrimaryThemeColor;
            RoutineFrame.BackgroundColor = App.PrimaryThemeColor;
            Home.BackgroundColor = App.SecondaryThemeColor;
            ButtonGridBottom.BackgroundColor = App.PrimaryThemeColor;
            Note.BackgroundColor = App.PrimaryThemeColor;
            Routine.BackgroundColor = App.PrimaryThemeColor;

            NavigationPage.SetHasNavigationBar(this, false);

            //Check if the image is a URI if not use default *FOR NOW*
            if (Client.ProfileImage != null)
            {
                ProfilePicture.Source = Client.ProfileImage;
            }
            else
            {
                //default image
                ProfilePicture.Source = "CoachMETransparent.png";
                ProfileFrame.BackgroundColor = App.ThemeColor4;
            }

            //WeightXML = XDocument.Parse(App.ClientInformationApp.Where(x => x.ClientID.ToString() == PassClientInfo.ClientID).First().ClientWeight);

            UserNameLabel.Text = Client.ClientName;
            UserNameLabel.FontFamily = App.CustomRegular;
            ClientEmailLabel.Text = Client.ClientEmail;
            ClientEmailLabel.FontFamily = App.CustomLight;

            BuildHistory();

            BuildPersonalRecordAsync();
        }

        private void BuildWeightGraph()
        {
            //Set up the data points for the Weight Graph            
            WeightXML = XDocument.Parse(PassClientInfo.ClientWeightHistory);
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

        private void BuildHistory()
        {
            HistoryRootObject currentClient = new HistoryRootObject();
            var orderedList = App.clientHistoryInformationApp.Where(x => x.ID.ToString() == PassClientInfo.ClientID).ToList();
            currentClient.HistoryInformation = orderedList;

            try
            {
                if (currentClient.HistoryInformation.Count > 0)
                {
                    int AmountToShow = 10;
                    if (currentClient.HistoryInformation.Count < AmountToShow)
                    {
                        AmountToShow = currentClient.HistoryInformation.Count;
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
                            Text = currentClient.HistoryInformation[i].Name,
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

                        String notesToInsert = currentClient.HistoryInformation[i].Notes;

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

                        HistoryInformation CurrentHistoryInfo = currentClient.HistoryInformation[i];

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

                        XDocument historyXML = XDocument.Parse(currentClient.HistoryInformation[i].HistoryXML);
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
                            Text = totalWeight.ToString("#,###.##") + " " + currentClient.HistoryInformation[i].WeightType,
                            FontFamily = App.CustomRegular,
                            FontSize = 10,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.End,
                        };

                        WeightSummary.Children.Add(TotalWeightLabel, 0, 0);
                        WeightSummary.Children.Add(TotalWeightMessage, 1, 0);

                        Label WorkoutDate = new Label
                        {
                            Text = currentClient.HistoryInformation[i].CompleteDate.ToShortDateString(),
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
            try
            {
                XDocument PRInfo = XDocument.Parse(PassClientInfo.ClientPRData);
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
                    WeightChartFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150)
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
                    WeightChartFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150)
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
                    GridHeaderLabel.ScaleTo(1, 150)
                    );
            }
            else
            {
                WeightEntryButton.Opacity = .4;
                await Task.WhenAll(
                    GridHeaderLabel.ScaleTo(0, 150),
                    WeightChartFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150)
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
                    WeightChartFrame.TranslateTo((screenSize.Width / 2) * -1, 0, 150)
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
            }
            else
            {
                TapeButton.Opacity = .4;
            }
        }


        private async void GoRoutine(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Client.ClientRoutine(PassClientInfo));
        }

        private async void BackClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Clients());
        }

        private async void GoNote(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Client.ClientNote(PassClientInfo));
        }

        private void BuildHistory(HistoryRootObject currentClient)
        {
            try
            {
                if (currentClient.HistoryInformation.Count > 0)
                {
                    int AmountToShow = 10;
                    if (App.clientHistoryInformationApp.Count < AmountToShow)
                    {
                        AmountToShow = currentClient.HistoryInformation.Count;
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
                            Text = currentClient.HistoryInformation[i].Name,
                            FontSize = 15,
                            HeightRequest = 20,
                            BackgroundColor = Color.FromHex("#f5f5f5"),
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            TextColor = App.PrimaryThemeColor
                        };

                        String notesToInsert = currentClient.HistoryInformation[i].Notes;

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

                        HistoryInformation CurrentHistoryInfo = currentClient.HistoryInformation[i];

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
                            new ColumnDefinition { Width = 120 }
                        }
                        };

                        HistoryContentDetail.GestureRecognizers.Add(ClickGrid);

                        Label Notes = new Label
                        {
                            Text = notesToInsert,
                            FontSize = 15,
                            Margin = new Thickness(15, 10, 10, 0),
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.End,
                        };

                        XDocument historyXML = XDocument.Parse(currentClient.HistoryInformation[i].HistoryXML);
                        int totalWeight = 0;

                        foreach (var exercise in historyXML.Element("Routine").Elements("Exercise"))
                        {
                            foreach (var set in exercise.Elements("Set"))
                            {
                                totalWeight = totalWeight + Int32.Parse(set.Element("Weight").Value);
                            }
                        }


                        Label Summary = new Label
                        {
                            Text = "Total Weight: " + totalWeight + " " + currentClient.HistoryInformation[i].WeightType,
                            FontSize = 15,
                            Margin = new Thickness(15, 10, 10, 0),
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.End,
                        };

                        Label WorkoutDate = new Label
                        {
                            Text = currentClient.HistoryInformation[i].CompleteDate.ToShortDateString(),
                            FontSize = 15,
                            Margin = new Thickness(15, 10, 10, 0),
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.End,
                        };

                        NotesFrame.Content = HistoryContentDetail;

                        HistoryContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                        HistoryContent.Children.Add(RoutineName, 0, 0);

                        HistoryContentDetail.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                        HistoryContentDetail.Children.Add(Notes, 0, 0);
                        Grid.SetColumnSpan(Notes, 2);

                        HistoryContentDetail.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                        HistoryContentDetail.Children.Add(Summary, 0, 1);

                        HistoryContentDetail.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                        HistoryContentDetail.Children.Add(WorkoutDate, 1, 1);

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
                    Text = "There is currently no routine history to show for this client.",
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

        override protected bool OnBackButtonPressed()
        {
            Navigation.PushAsync(new Clients());

            return true;
        }
    }
}