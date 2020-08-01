using System;
using System.Linq;
using System.Xml.Linq;
using TrackWorkout.Entitys;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.History
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Summary : ContentPage
    {
        HistoryInformation HistoryItemShare;

        public Summary(HistoryInformation HistoryItem)
        {
            InitializeComponent();
            HistoryItemShare = HistoryItem;
            FinishRoutineFrame.BackgroundColor = App.PrimaryThemeColor;
            FinishButtonNew.BackgroundColor = App.SecondaryThemeColor;
            ButtonGridBottom.BackgroundColor = App.PrimaryThemeColor;
            BackButton.BackgroundColor = App.PrimaryThemeColor;
            Notes.FontFamily = App.CustomRegular;

            // Hide default nav
            NavigationPage.SetHasNavigationBar(this, false);

            XDocument Routine = XDocument.Parse(HistoryItem.HistoryXML);

            Notes.Text = HistoryItem.Notes;

            HeaderLabel.Text = Routine.Element("Routine").Element("Name").Value;

            ShowRoutineSummary(Routine);

            BackButton.Clicked += (o, e) =>
            {
                Navigation.PopAsync();
            };
        }

        private async void StartClick(object sender, EventArgs e)
        {
            var QuestionPop = new Pages.PopUps.UserChoiceMessage("Start Routine", "Would you like to start this routine", "Yes", "No");
            var answer = await QuestionPop.Show();
            if (answer) // Yes
            {
                RoutineList currentRoutine;
                string FromSource = string.Empty;

                try
                {
                    currentRoutine = App.UserRoutineListApp.Where(x => x.RoutineID == HistoryItemShare.RoutineID).First();
                    FromSource = "Routine";
                }
                catch
                {
                    currentRoutine = App.CoachAssignedRoutineListApp.Where(x => x.RoutineID == HistoryItemShare.RoutineID).First();
                    FromSource = "Coach";
                }

                await Navigation.PushAsync(new Routine.RoutineInProgress(currentRoutine, "User", "Home"));
            }
        }

        private void ShowRoutineSummary(XDocument Routine)
        {
            foreach (var exercise in Routine.Element("Routine").Elements("Exercise"))
            {
                // Create Name Label
                Label ExerciseLabel = new Label
                {
                    Text = exercise.Element("Description").Value,
                    FontSize = 15,
                    FontFamily = App.CustomRegular,
                    HeightRequest = 20,
                    Margin = new Thickness(15, 10, 0, 0),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = App.PrimaryThemeColor

                };

                // Create Weight Label
                Label WeightLabel = new Label
                {
                    Text = "Weight",
                    FontFamily = App.CustomBold,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 15,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.Gray
                };

                // Create Reps Label
                Label RepsLabel = new Label
                {
                    Text = "Reps",
                    FontFamily = App.CustomBold,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 15,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.Gray
                };

                // Create Frame
                Frame ExerciseFrame = new Frame
                {
                    HasShadow = false,
                    Padding = 0,
                    Margin = new Thickness(0, 0, 0, 40),
                };

                // Add Exercise Name to the Screen
                gridStack.Children.Add(ExerciseLabel);

                // Create grid on this loop
                Grid ExerciseContent = new Grid
                {
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    BackgroundColor = Color.White,
                    ColumnDefinitions = {
                            new ColumnDefinition { Width = 150 },
                            new ColumnDefinition { Width = 150 },
                            new ColumnDefinition { Width = GridLength.Star }
                        }
                };

                // Add Row to Grid that will contain the headers. Then add the headers to the grid
                ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Absolute) });
                ExerciseContent.Children.Add(WeightLabel, 0, 0);
                ExerciseContent.Children.Add(RepsLabel, 1, 0);

                // Fill the grid with Reps and Weight values
                FillGrid(exercise, ExerciseContent);

                // Add Grid to Frame
                ExerciseFrame.Content = ExerciseContent;

                // Add Frame to page
                gridStack.Children.Add(ExerciseFrame);
            }
        }
        // Use to fill the exercise info
        private void FillGrid(XElement exercise, Grid ExerciseContent)
        {
            foreach (var set in exercise.Elements("Set"))
            {
                // Create new object and fill with XML data.
                RoutineList newRoutineObject = new RoutineList();

                newRoutineObject.ExerciseNumber = Int32.Parse(exercise.Element("Number").Value);
                newRoutineObject.Reps = set.Element("Reps").Value;
                newRoutineObject.SetNumber = Int32.Parse(set.Element("Number").Value);
                newRoutineObject.Weight = set.Element("Weight").Value;
                newRoutineObject.RestTimeAfterSet = Int32.Parse(exercise.Element("RestTimeAfterSet").Value);
                newRoutineObject.ExerciseDescription = exercise.Element("Description").Value;

                Color textColor = Color.Gray;
                Color backgroundColor = Color.White;

                // Create objects for the UI
                Label WeightEntry = new Label
                {
                    Text = newRoutineObject.Weight.ToString(),
                    FontSize = 15,
                    FontFamily = App.CustomLight,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = textColor,
                    BackgroundColor = backgroundColor
                };
                Label RepEntry = new Label
                {
                    Text = newRoutineObject.Reps.ToString(),
                    FontSize = 15,
                    HorizontalOptions = LayoutOptions.Center,
                    FontFamily = App.CustomLight,
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = textColor,
                    BackgroundColor = backgroundColor
                };

                Color frameBackground = Color.Transparent;

                if (backgroundColor == App.ThemeColor3)
                {
                    frameBackground = backgroundColor;
                }

                BoxView box1 = new BoxView
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    BackgroundColor = frameBackground
                };

                ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
                // This allows the column to change color using a span on a box view. This must be inserted before the Label.
                ExerciseContent.Children.Add(box1, 0, 3, newRoutineObject.SetNumber, newRoutineObject.SetNumber + 1);

                // Then the labels are placed over the box view.
                ExerciseContent.Children.Add(WeightEntry, 0, newRoutineObject.SetNumber);
                ExerciseContent.Children.Add(RepEntry, 1, newRoutineObject.SetNumber);
            }
        }
        override protected bool OnBackButtonPressed()
        {
            Navigation.PopAsync();

            return true;
        }
    }
}