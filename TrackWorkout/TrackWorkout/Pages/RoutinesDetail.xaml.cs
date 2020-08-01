using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackWorkout.Entitys;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using Newtonsoft.Json;
using Json.Net;


namespace TrackWorkout.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutinesDetail : ContentPage
    {

        List<UserInformation> userInfo;

        public RoutinesDetail(List<UserInformation> userInformation)
        {
            InitializeComponent();

            userInfo = userInformation;

            CreateRoutineGrids();
        }

        public async void CreateRoutineGrids()
        {
            PassID idToPass = new PassID { ID = userInfo[0].UserId.ToString() };

            // Use the Email object to get the rest of the data set
            var client = new HttpClient();
            var jsonContent = JsonNet.Serialize(idToPass);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            // API call to webservice to save user in the database
            var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetUserRoutineList", httpContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var routineList = JsonConvert.DeserializeObject<RoutineListRootObject>(responseString);
         
            if (routineList.RoutineList.Count == 0)
            {
                // Display Failure Message and stay on page
                Label NoWorkouts = new Label
                {
                    Text = "There are currently no routines to show. Add Routines to begin.",
                    FontSize = 15,
                    Margin = new Thickness(15, 0, 0, 0),
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Color.FromHex("#5bc2dc")
                };

                gridStack.Children.Add(NoWorkouts);
            }

            else
            {
                int currentRoutineID = routineList.RoutineList[0].RoutineID;

                // Create new list with unique routine id
                List<int> uniqueRoutineID = routineList.RoutineList.Select(x => x.RoutineID).AsParallel().Distinct().ToList();

                // Loop through new list
                for (int i = 0; i < uniqueRoutineID.Count; i++)
                {
                    // I need to set the set ID of the new object added then use that to control the GridCount
                    var listOfSetWithSameRoutineID = routineList.RoutineList.Where(value => value.RoutineID == uniqueRoutineID[i]).ToList();

                    string RoutineLabelText = listOfSetWithSameRoutineID.Min(minRoutineName => minRoutineName.RoutineName);

                    // Create Name Label
                    Label RoutineLabel = new Label
                    {
                        Text = RoutineLabelText,
                        FontSize = 15,
                        HeightRequest = 20,
                        Margin = new Thickness(15, 10, 0, 0),
                        //FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.End,
                        TextColor = Color.FromHex("#5bc2dc")
                    };

                    // Create Start Button
                    Button StartRoutine = new Button
                    {
                        Text = "Start Routine",
                        FontSize = 15,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        TextColor = Color.White,
                        BackgroundColor = Color.FromHex("#5bc2dc")
                    };

                    // Create Options Button
                    Button OptionsButton = new Button
                    {
                        Text = "...",
                        FontSize = 20,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.Start,
                        TextColor = Color.Black,
                        BackgroundColor = Color.White
                    };

                    // Create Frame
                    Frame RoutineFrame = new Frame
                    {
                        HasShadow = true,
                        BackgroundColor = Color.Red,
                        Padding = 0,
                        Margin = new Thickness(0, 0, 0, 10)
                    };

                    // Add Routine Name to the Screen
                    gridStack.Children.Add(RoutineLabel);

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

                    // Add Row to Grid that will contain the option button. Then add the button to row last column
                    routineContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
                    routineContent.Children.Add(OptionsButton, 1, 0);

                    // Fill the grid with workout content
                    FillGrid(listOfSetWithSameRoutineID, routineContent);

                    // Create a new row to store the start routine button then add the button to the row.
                    routineContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
                    routineContent.Children.Add(StartRoutine, 0, 2);
                    Grid.SetColumnSpan(StartRoutine, 2);

                    // What to do when clicking the start routine button
                    StartRoutine.Clicked += (o, e) =>
                    {
                        StartNewRoutine(listOfSetWithSameRoutineID, RoutineLabelText);
                    };

                    // Add Grid to Frame
                    RoutineFrame.Content = routineContent;

                    // Add Frame to page
                    gridStack.Children.Add(RoutineFrame);
                }
            }

        }

        // Use to fill the exercise info
        private void FillGrid(List<RoutineList> listOfSetWithSameRoutineID, Grid routineContent)
        {
            // The string that will hold all of the info
            Label RoutineExerciseString = new Label
            {
                FontSize = 15,
                Margin = new Thickness(10, 0, 10, 10),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start                
            };

            // Create new list with unique routine id
            List<int> uniqueExerciseNumber = listOfSetWithSameRoutineID.Select(x => x.ExerciseNumber).AsParallel().Distinct().ToList();

            //Fill values for RoutineExerciseString
            for (int i = 0; i < uniqueExerciseNumber.Count; i++)
            {
                var listWithSameExerciseNumber = listOfSetWithSameRoutineID.Where(value => value.ExerciseNumber == uniqueExerciseNumber[i]).ToList();

                int GetMaxSetNumber = listWithSameExerciseNumber.Max(maxSetNumber => maxSetNumber.SetNumber);

                var listOfMaxSetWithSameExerciseNumber = listWithSameExerciseNumber.Where(y => y.SetNumber == GetMaxSetNumber).ToList();

                if (i == 0)
                {
                    RoutineExerciseString.Text = listOfMaxSetWithSameExerciseNumber[0].ExerciseDescription + ": " + listOfMaxSetWithSameExerciseNumber[0].SetNumber + " Sets";
                }
                else
                {
                    RoutineExerciseString.Text = RoutineExerciseString.Text + ", " + listOfMaxSetWithSameExerciseNumber[0].ExerciseDescription + ": " + listOfMaxSetWithSameExerciseNumber[0].SetNumber + " Sets";
                }                    
            }

            // Add the row that will hold the exercise info then insert into that row
            routineContent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            routineContent.Children.Add(RoutineExerciseString, 0, 1);

            // Merge the columns together on this row to fill the row out completely
            Grid.SetColumnSpan(RoutineExerciseString, 2);

        }

        public interface ICallDialog
        {
            Task CallDialog(object viewModel);
        }

        // When hitting the add new routine button go to the PickExcercise page to begin the process 
        // of creating a new routine.
        private async void AddNewRoutine(object sender, EventArgs e)
        {
            // Use the Email object to get the rest of the data set
            var client = new HttpClient();
            // API call to webservice to save user in the database
            var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetExerciseList", null);
            var responseString = await response.Content.ReadAsStringAsync();
            var exerciseList = JsonConvert.DeserializeObject<ExerciseListRootObject>(responseString);            
            
            await Navigation.PushAsync(new AddExercise.PickExercise(userInfo, exerciseList));
        }

        // When hitting the start routine button a pop up appears asking for confirmation to start. Once confirmed the routine will begin
        private async void StartNewRoutine(List<RoutineList> listOfSetWithSameRoutineID, string RoutineLabelText)
        {
            var answer = await DisplayAlert("Confirm", "Are you ready to start the " + RoutineLabelText + " routine", "Yes", "No");
            if (answer) // Yes
            {
                await Navigation.PushAsync(new RoutineInProgress(listOfSetWithSameRoutineID, userInfo));
            }
        }

        override protected bool OnBackButtonPressed()
        {

            LeavePageCheck();

            return true;
        }

        private async void LeavePageCheck()
        {
            
                await Navigation.PushAsync(new HomeScreen(userInfo));            
        }
    }
}