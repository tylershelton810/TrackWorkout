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
	public partial class RoutineSummary : ContentPage
	{
        List<UserInformation> userToPass;
        List<RoutineList> OldList;
        List<RoutineList> KeepOldData = new List<RoutineList>();

        public RoutineSummary (List<RoutineList> NewList, List<UserInformation> userInformation)
		{
			InitializeComponent ();

            userToPass = userInformation;


            for (int i = 0; i < NewList.Count; i++)
            {
                KeepOldData.Add(NewList[i]);
            }
            

            for (int i = (NewList.Count - 1); i > 0; i--)
            {

                if (NewList[i].KeepRow == false)
                {
                    NewList.RemoveAt(i);
                }

            }

            GetOldList(NewList);

            NavigationPage.SetHasNavigationBar(this, false);

            string HeaderLabelText = NewList.Min(minRoutineName => minRoutineName.RoutineName);

            Label HeaderLabel = new Label
            {
                Text = HeaderLabelText,
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                HeightRequest = 20,
                Margin = new Thickness(15, 0, 0, 0),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.White
            };

            Button FinishButton = new Button
            {
                Text = "Finish",
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#5bc2dc")
            };

            // Create Frame
            Frame HeaderFrame = new Frame
            {
                HasShadow = true,
                Padding = 0,
            };

            // Create grid on this loop
            Grid HeaderContent = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                BackgroundColor = Color.FromHex("#5bc2dc"),
                ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = 80 }
                        }
            };

            // Add Row to Grid that will contain the headers. Then add the headers to the grid
            HeaderContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60, GridUnitType.Absolute) });
            HeaderContent.Children.Add(HeaderLabel, 0, 0);
            HeaderContent.Children.Add(FinishButton, 1, 0);

            // Add Grid to Frame
            HeaderFrame.Content = HeaderContent;

            // Add Frame to page
            HeaderLayout.Children.Add(HeaderFrame);

            ShowRoutineSummary(NewList);

            FinishButton.Clicked += (o, e) =>
            {
                FinishRoutine(NewList, OldList);
            };

            Notes.Unfocused += (o, e) =>
            {
                NewList[0].Notes = Notes.Text;
            };
        }

        public async void GetOldList(List<RoutineList> NewList)
        {
            PassID idToPass = new PassID { ID = NewList[0].RoutineID.ToString() };

            // Use the Email object to get the rest of the data set
            var client = new HttpClient();
            var jsonContent = JsonNet.Serialize(idToPass);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            // API call to webservice to save user in the database
            var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetRoutineForCompare", httpContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var routineList = JsonConvert.DeserializeObject<RoutineListRootObject>(responseString);

            OldList = routineList.RoutineList;
        }

        private void ShowRoutineSummary(List<RoutineList> NewList)
        {
            // Create new list with unique exercise id
            List<int> uniqueExerciseNumber = NewList.Select(x => x.ExerciseNumber).AsParallel().Distinct().ToList();

            for (int i = 0; i < uniqueExerciseNumber.Count; i++)
            {
                // I need to set the set ID of the new object added then use that to control the GridCount
                var listOfSetWithSameExerciseNumber = NewList.Where(value => value.ExerciseNumber == uniqueExerciseNumber[i]).ToList();

                string ExerciseLabelText = listOfSetWithSameExerciseNumber.Min(minExerciseDescription => minExerciseDescription.ExerciseDescription);

                // Create Name Label
                Label ExerciseLabel = new Label
                {
                    Text = ExerciseLabelText,
                    FontSize = 15,
                    HeightRequest = 20,
                    Margin = new Thickness(15, 10, 0, 0),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.FromHex("#5bc2dc")
                };

                // Create Weight Label
                Label WeightLabel = new Label
                {
                    Text = "Weight",
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
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 15,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.Gray
                };

                // Create Frame
                Frame ExerciseFrame = new Frame
                {
                    HasShadow = true,
                    Padding = 0,
                    Margin = new Thickness(0, 0, 0, 10),
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
                FillGrid(listOfSetWithSameExerciseNumber, ExerciseContent);

                // Add Grid to Frame
                ExerciseFrame.Content = ExerciseContent;

                // Add Frame to page
                gridStack.Children.Add(ExerciseFrame);
            }
        }

        // Use to fill the exercise info
        private void FillGrid(List<RoutineList> listOfSetWithSameExerciseNumber, Grid ExerciseContent)
        {
            for (int i = 0; i < listOfSetWithSameExerciseNumber.Count; i++)
            {
                RoutineList newRoutineObject = new RoutineList();

                newRoutineObject.ExerciseNumber = listOfSetWithSameExerciseNumber[i].ExerciseNumber;
                newRoutineObject.Reps = listOfSetWithSameExerciseNumber[i].Reps;
                newRoutineObject.SetNumber = listOfSetWithSameExerciseNumber[i].SetNumber;
                newRoutineObject.Weight = listOfSetWithSameExerciseNumber[i].Weight;
                newRoutineObject.RestTimeAfterSet = listOfSetWithSameExerciseNumber[i].RestTimeAfterSet;

                Label WeightEntry = new Label
                {
                    Text = newRoutineObject.Weight.ToString(),
                    FontSize = 15,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.Gray,
                    BackgroundColor = Color.White
                };
                Label RepEntry = new Label
                {
                    Text = newRoutineObject.Reps.ToString(),
                    FontSize = 15,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.Gray,
                    BackgroundColor = Color.White
                };                                       

                ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });

                ExerciseContent.Children.Add(WeightEntry, 0, i + 1);
                ExerciseContent.Children.Add(RepEntry, 1, i + 1);

            }

        }

        // When hitting the start routine button a pop up appears asking for confirmation to start. Once confirmed the routine will begin
        private async void FinishRoutine(List<RoutineList> routineList, List<RoutineList> OldList)
        {
            bool Match = true;

            for (int i = 0; i < routineList.Count; i++)
            {
                try
                {
                    if (routineList[i].Weight != OldList[i].Weight)
                    {
                        Match = false;
                    }

                    if (routineList[i].Reps != OldList[i].Reps)
                    {
                        Match = false;
                    }
                    if (routineList.Count != OldList.Count)
                    {
                        Match = false;
                    }
                }
                catch
                {
                    Match = false;
                }
                
            }

                if (Match == false)
            {
                var answer = await DisplayAlert("Confirm", "Would you like to overlay your old routine with this updated one?", "Yes", "No");
                if (answer) // Yes
                {                    
                    routineList[0].ProcedureType = 2; //Replaces routine and logs history

                    var client = new HttpClient();
                    var jsonContent = JsonNet.Serialize(routineList);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    // API call to webservice to save user in the database
                    var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/CompleteWorkout", httpContent);
                    var responseString = await response.Content.ReadAsStringAsync();
                    var userInformation = JsonConvert.DeserializeObject<UserInformationRootObject>(responseString);

                    await DisplayAlert("Overwritten", $"This routine has been updated. Another workout complete! Good job!", "OK");

                    await Navigation.PushAsync(new HomeScreen(userToPass));
                }
                else
                {                    
                    routineList[0].ProcedureType = 1; //Keeps old routine and logs history

                    var client = new HttpClient();
                    var jsonContent = JsonNet.Serialize(routineList);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    // API call to webservice to save user in the database
                    var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/CompleteWorkout", httpContent);
                    var responseString = await response.Content.ReadAsStringAsync();
                    var userInformation = JsonConvert.DeserializeObject<UserInformationRootObject>(responseString);

                    await DisplayAlert("Unchanged", $"This routine will remain the same. Another workout complete! Good job!", "OK");

                    await Navigation.PushAsync(new HomeScreen(userToPass));
                }
            }
            else
            {                
                routineList[0].ProcedureType = 1; //Keeps old routine and logs history

                var client = new HttpClient();
                var jsonContent = JsonNet.Serialize(routineList);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/CompleteWorkout", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var userInformation = JsonConvert.DeserializeObject<UserInformationRootObject>(responseString);

                await DisplayAlert("Congrats", $"Another workout complete! Good job!", "OK");

                await Navigation.PushAsync(new HomeScreen(userToPass));
        }
            
        }

        override protected bool OnBackButtonPressed()
        {
            Navigation.PushAsync(new RoutineInProgress(KeepOldData, userToPass));
            return true;
        }
    }
}