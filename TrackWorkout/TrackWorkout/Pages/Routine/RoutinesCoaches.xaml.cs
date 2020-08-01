using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackWorkout.Entitys;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Xml.Linq;

namespace TrackWorkout.Pages.Routine
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutinesCoaches : ContentPage
    {
        List<RoutineList> CurrentCoachList;

        public RoutinesCoaches()
        {
            InitializeComponent();

            //MyRoutines.BackgroundColor = App.SecondaryThemeColor;
            CoachRoutines.BackgroundColor = App.SecondaryThemeColor;
            CoachRoutines.BorderColor = App.PrimaryThemeColor;
            CouchRoutineFrame.BackgroundColor = App.PrimaryThemeColor;
            ButtonGridBottom.BackgroundColor = App.PrimaryThemeColor;

            CurrentCoachList = App.CoachAssignedRoutineListApp.ToList();

            // Create new list with unique routine id
            List<int> uniqueRoutineID = CurrentCoachList.Select(x => x.RoutineID).AsParallel().Distinct().ToList();
            // Loop through new list
            for (int i = 0; i < uniqueRoutineID.Count; i++)
            {
                CreateRoutineGrids(uniqueRoutineID, i);
            }
            
        }

        public void CreateRoutineGrids(List<int> uniqueRoutineID, int i)
        {
            // I need to set the set ID of the new object added then use that to control the GridCount
            var currentRoutine = CurrentCoachList[i];

            string RoutineLabelText = currentRoutine.RoutineName;
            string CoachLabelText = currentRoutine.CoachName;

            // Create Name Label
            Label RoutineLabel = new Label
            {
                Text = RoutineLabelText,
                FontSize = 12,
                FontFamily = App.CustomBold,
                HeightRequest = 20,
                Margin = new Thickness(15, 10, 0, 0),
                //FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                TextColor = App.PrimaryThemeColor
            };

            Label CoachLabel = new Label
            {
                Text = CoachLabelText,
                FontAttributes = FontAttributes.Bold,
                FontSize = 12,
                FontFamily = App.CustomRegular,
                HeightRequest = 20,
                Margin = new Thickness(0, 10, 15, 0),
                //FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
                TextColor = App.PrimaryThemeColor
            };

            // Create Start Button
            Button StartRoutine = new Button
            {
                Text = "Start Routine",
                FontSize = 12,
                FontFamily = App.CustomBold,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                TextColor = Color.White,
                BackgroundColor = App.PrimaryThemeColor
            };

            // Create Frame
            Frame RoutineFrame = new Frame
            {
                HasShadow = false,
                BackgroundColor = Color.Red,
                Padding = 0,
                Margin = new Thickness(0, 0, 0, 10)
            };

            // Create grid on this loop
            Grid routineHeader = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                BackgroundColor = Color.Transparent,
                ColumnDefinitions = {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 100 }
                }
            };

            // Add Row to Grid that will contain the option button. Then add the button to row last column
            routineHeader.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
            routineHeader.Children.Add(RoutineLabel, 0, 0);
            routineHeader.Children.Add(CoachLabel, 1, 0);

            // Add Routine Name to the Screen
            gridStack.Children.Add(routineHeader);

            // Create grid on this loop
            Grid routineContent = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                BackgroundColor = Color.White,
                ColumnDefinitions = {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 50 }
                }
            };

            // Fill the grid with workout content
            FillGrid(currentRoutine, routineContent);

            // Create a new row to store the start routine button then add the button to the row.
            routineContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
            routineContent.Children.Add(StartRoutine, 0, 2);
            Grid.SetColumnSpan(StartRoutine, 2);

            // What to do when clicking the start routine button
            StartRoutine.Clicked += (o, e) =>
            {
                StartNewRoutine(currentRoutine, RoutineLabelText);
            };

            // Add Grid to Frame
            RoutineFrame.Content = routineContent;

            // Add Frame to page
            gridStack.Children.Add(RoutineFrame);                            

        }

        // Use to fill the exercise info
        private void FillGrid(RoutineList currentRoutine, Grid routineContent)
        {
            XDocument routineXML = XDocument.Parse(currentRoutine.RoutineDetail);
            (Grid ExerciseContentDetail, double totalWeight) = Pages.SharedClasses.Build.RoutineGrids(routineXML);

            // Add the row that will hold the exercise info then insert into that row
            routineContent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            routineContent.Children.Add(ExerciseContentDetail, 0, 1);

            // Merge the columns together on this row to fill the row out completely
            Grid.SetColumnSpan(ExerciseContentDetail, 2);

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
                Text = totalWeight.ToString("#,###.##") + " Lbs",
                FontFamily = App.CustomRegular,
                FontSize = 10,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
            };

            WeightSummary.Children.Add(TotalWeightLabel, 0, 0);
            WeightSummary.Children.Add(TotalWeightMessage, 1, 0);


            routineContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
            routineContent.Children.Add(WeightSummary, 0, 2);

        }

        public interface ICallDialog
        {
            Task CallDialog(object viewModel);
        }        

        private async void RoutinePage(object sender, EventArgs e)
        {            
            await Navigation.PopAsync();
        }

        // When hitting the start routine button a pop up appears asking for confirmation to start. Once confirmed the routine will begin
        private async void StartNewRoutine(RoutineList currentRoutine, string RoutineLabelText)
        {
            var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", "Are you ready to start the " + RoutineLabelText + " routine", "Yes", "No");
            var answer = await QuestionPop.Show();
            if (answer) // Yes
            {
               await Navigation.PushAsync(new RoutineInProgress(currentRoutine, "Coach", "Routine"));
            }
        }

        override protected bool OnBackButtonPressed()
        {

            LeavePageCheck();

            return true;
        }

        private async void LeavePageCheck()
        {
            
            await Navigation.PushAsync(new HomeScreen());            
        }
    }
}