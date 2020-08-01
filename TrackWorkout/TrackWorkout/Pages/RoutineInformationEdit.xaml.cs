using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackWorkout.Entitys;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Xaml;
using TrackWorkout;
using System.Net.Http;
using Newtonsoft.Json;
using Json.Net;
using System.Data;

namespace TrackWorkout.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoutineInformationEdit : ContentPage
	{        
        List<UserInformation> UserList;

        List<RoutineList> BuildingRoutine = new List<RoutineList>();

        Grid HeaderContent = new Grid
        {
            RowSpacing = 0,
            ColumnSpacing = 0,
            BackgroundColor = Color.FromHex("#5bc2dc"),
            ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star },                            
                            new ColumnDefinition { Width = 70 }
                        }
        };

        Label HeaderLabel = new Label
        {
            Text = "Create Routine",
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

        public RoutineInformationEdit (List<RoutineList> exercises, List<UserInformation> userInformation)
		{
			InitializeComponent ();            

            BuildingRoutine = exercises;            

            //addedID = 0;
            //OrderID = 1;
            UserList = userInformation;

            NavigationPage.SetHasNavigationBar(this, false);
            
            // Create Frame
            Frame HeaderFrame = new Frame
            {
                HasShadow = true,
                Padding = 0,
            };

            // Add Row to Grid that will contain the headers. Then add the headers to the grid
            HeaderContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60, GridUnitType.Absolute) });
            HeaderContent.Children.Add(HeaderLabel, 0, 0);
            HeaderContent.Children.Add(FinishButton, 1, 0);

            // Add Grid to Frame
            HeaderFrame.Content = HeaderContent;

            // Add Frame to page
            HeaderLayout.Children.Add(HeaderFrame);

            FinishButton.Clicked += (o, e) =>
            {
                FinishRoutine();
            };

            BuildRoutine(exercises);

            RoutineName.Unfocused += (o, e) =>
            {
                HeaderLabel.Text = RoutineName.Text;
            };
        }

        private void BuildRoutine(List<RoutineList> Routine)
        {
            // Create new list with unique routine id
            List<int> uniqueExerciseNumber = Routine.Select(x => x.ExerciseNumber).AsParallel().Distinct().ToList();
            uniqueExerciseNumber.Sort();

            for (int i = 0; i < uniqueExerciseNumber.Count; i++)
            {
                // I need to set the set ID of the new object added then use that to control the GridCount
                var listOfSetWithSameExerciseNumber = Routine.Where(value => value.ExerciseNumber == uniqueExerciseNumber[i]).ToList();

                string ExerciseLabelText = listOfSetWithSameExerciseNumber.Min(minExerciseDescription => minExerciseDescription.ExerciseDescription);
                int RestTimeValue = listOfSetWithSameExerciseNumber.Min(minRestTime => minRestTime.RestTimeAfterSet);

                // Create Name Label
                Label ExerciseLabel = new Label
                {
                    Text = ExerciseLabelText,
                    FontSize = 15,
                    HeightRequest = 20,
                    Margin = new Thickness(15, 0, 0, 5),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.FromHex("#5bc2dc")
                };

                Button RemoveRest = new Button
                {
                    Text = "-",
                    FontSize = 20,
                    //HeightRequest = 20,
                    //Margin = new Thickness(15, 0, 0, 0),
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.FromHex("#dc765b"),
                    BackgroundColor = Color.Transparent
                };
                
                Label RestLabel = new Label
                {
                    Text = "Rest Time: " + RestTimeValue.ToString(),
                    FontSize = 15,
                    HeightRequest = 20,
                    Margin = new Thickness(0, 0, 0, 5),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.FromHex("#5bc2dc")
                };

                Button AddRest = new Button
                {
                    Text = "+",
                    FontSize = 15,                    
                    //HeightRequest = 20,
                    Margin = new Thickness(0, 5, 0, 0),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.FromHex("#5bdc76"),
                    BackgroundColor = Color.Transparent
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


                Grid ExerciseHeader = new Grid
                {
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    BackgroundColor = Color.FromHex("#f5f5f5"),
                    ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = 40 },
                            new ColumnDefinition { Width = 90 },
                            new ColumnDefinition { Width = 40 }
                            //new ColumnDefinition {Width = 150 }
                        }
                };

                ExerciseHeader.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
                ExerciseHeader.Children.Add(ExerciseLabel, 0, 0);
                ExerciseHeader.Children.Add(RemoveRest, 1, 0);
                ExerciseHeader.Children.Add(RestLabel, 2, 0);
                ExerciseHeader.Children.Add(AddRest, 3, 0);

                RemoveRest.Clicked += (o, e) =>
                {                   
                    RestTimeValue = RestTimeValue - 15;
                    RestLabel.Text = "Rest Time: " + RestTimeValue.ToString();

                    if (RestTimeValue == 15)
                    {
                        RemoveRest.IsVisible = false;
                    }

                    foreach (var ObjectToUpdate in BuildingRoutine.Where(ExerciseID => ExerciseID.ExerciseNumber == listOfSetWithSameExerciseNumber[0].ExerciseNumber))
                    {
                        ObjectToUpdate.RestTimeAfterSet = RestTimeValue;
                    }
                    
                };

                AddRest.Clicked += (o, e) =>
                {
                    RestTimeValue = RestTimeValue + 15;
                    RestLabel.Text = "Rest Time: " + RestTimeValue.ToString();

                    if (RestTimeValue == 30)
                    {
                        RemoveRest.IsVisible = true;
                    }

                    foreach (var ObjectToUpdate in BuildingRoutine.Where(ExerciseID => ExerciseID.ExerciseNumber == listOfSetWithSameExerciseNumber[0].ExerciseNumber))
                    {
                        ObjectToUpdate.RestTimeAfterSet = RestTimeValue;
                    }
                };

                // Add Exercise Name to the Screen
                gridStack.Children.Add(ExerciseHeader);

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
                FillGrid(listOfSetWithSameExerciseNumber, ExerciseContent, ExerciseHeader, ExerciseFrame);

                // Add Grid to Frame
                ExerciseFrame.Content = ExerciseContent;

                // Add Frame to page
                gridStack.Children.Add(ExerciseFrame);
            }
        }

        private void FillGrid(List<RoutineList> listOfSetWithSameExerciseNumber, Grid ExerciseContent, Grid ExerciseHeader, Frame ExerciseFrame)
        {
            for (int i = 0; i < listOfSetWithSameExerciseNumber.Count; i++)
            {
                AddRow(listOfSetWithSameExerciseNumber, ExerciseContent, i, ExerciseHeader, ExerciseFrame);
            }
        }

        private void AddRow(List<RoutineList> listOfSetWithSameExerciseNumber, Grid ExerciseContent, int i, Grid ExerciseHeader, Frame ExerciseFrame)
        {

            RoutineList newRoutineObject = new RoutineList();

            newRoutineObject.ExerciseNumber = listOfSetWithSameExerciseNumber[i].ExerciseNumber;
            newRoutineObject.Reps = listOfSetWithSameExerciseNumber[i].Reps;
            newRoutineObject.SetNumber = listOfSetWithSameExerciseNumber[i].SetNumber;
            newRoutineObject.Weight = listOfSetWithSameExerciseNumber[i].Weight;
            newRoutineObject.RestTimeAfterSet = listOfSetWithSameExerciseNumber[i].RestTimeAfterSet;
            newRoutineObject.ExerciseDescription = listOfSetWithSameExerciseNumber[i].ExerciseDescription;
            newRoutineObject.RoutineID = listOfSetWithSameExerciseNumber[i].RoutineID;
            newRoutineObject.RoutineName = listOfSetWithSameExerciseNumber[i].RoutineName;

            Entry WeightEntry = new Entry
            {
                Text = newRoutineObject.Weight.ToString(),
                FontSize = 15,
                Keyboard = Keyboard.Numeric,
                WidthRequest = 30,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                MaxLength = 3,
                TextColor = Color.Gray,
                BackgroundColor = Color.White
            };
            Entry RepEntry = new Entry
            {
                Text = newRoutineObject.Reps.ToString(),
                FontSize = 15,
                Keyboard = Keyboard.Numeric,
                WidthRequest = 30,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                MaxLength = 3,
                TextColor = Color.Gray,
                BackgroundColor = Color.White
            };
           

            Button AddSet = new Button
            {
                Text = "Add",
                TextColor = Color.FromHex("#5bdc76"),
                BackgroundColor = Color.Transparent
            };

            Button RemoveSet = new Button
            {
                Text = "Remove",
                TextColor = Color.FromHex("#dc765b"),
                BackgroundColor = Color.Transparent
            };
            
            int RestTime = newRoutineObject.RestTimeAfterSet;            

            ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });

            ExerciseContent.Children.Add(WeightEntry, 0, i + 1);
            ExerciseContent.Children.Add(RepEntry, 1, i + 1);

            if (i == listOfSetWithSameExerciseNumber.Count - 1)
            {
                ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });

                ExerciseContent.Children.Add(AddSet, 0, i + 2);
                ExerciseContent.Children.Add(RemoveSet, 1, i + 2);
            }
           
            WeightEntry.Unfocused += (o, e) =>
            {
                foreach (var ObjectToUpdate in BuildingRoutine.Where(ExerciseID => ExerciseID.ExerciseNumber == newRoutineObject.ExerciseNumber).Where(SetID => SetID.SetNumber == newRoutineObject.SetNumber))
                {
                    ObjectToUpdate.Weight = WeightEntry.Text;
                }

            };

            RepEntry.Unfocused += (o, e) =>
            {
                foreach (var ObjectToUpdate in BuildingRoutine.Where(ExerciseID => ExerciseID.ExerciseNumber == newRoutineObject.ExerciseNumber).Where(SetID => SetID.SetNumber == newRoutineObject.SetNumber))
                {
                    ObjectToUpdate.Reps = RepEntry.Text;
                }
            };

            AddSet.Clicked += (o, e) =>
            {
                AddSetClick(ExerciseContent, listOfSetWithSameExerciseNumber, ExerciseHeader, ExerciseFrame);
            };

            RemoveSet.Clicked += (o, e) =>
            {
                var lastListElement = listOfSetWithSameExerciseNumber.Count - 1;

                // Stops the last set from being removed.
                if (BuildingRoutine.Count == 1)
                {
                    DisplayAlert("Warning", "This is the last set. This cannot be removed.", "OK");
                    RemoveSet.IsVisible = false;
                }
                else
                {

                    // 3 rows means there is only one data row left. Row 0 = headers Row 1 = data Row 2 = buttons.
                    // This if will delete the rest of the gris completely.
                    if (ExerciseContent.RowDefinitions.Count == 3)
                    {
                        foreach (var child in ExerciseContent.Children.ToList())
                        {
                            ExerciseContent.Children.Remove(child);
                        }

                        ExerciseContent.RowDefinitions.RemoveAt(ExerciseContent.RowDefinitions.Count - 1);
                        ExerciseContent.RowDefinitions.RemoveAt(ExerciseContent.RowDefinitions.Count - 1);
                        ExerciseContent.RowDefinitions.RemoveAt(ExerciseContent.RowDefinitions.Count - 1);

                        foreach (var child in ExerciseHeader.Children.ToList())
                        {
                            ExerciseHeader.Children.Remove(child);
                        }

                        ExerciseHeader.RowDefinitions.RemoveAt(0);

                        gridStack.Children.Remove(ExerciseFrame);
                    }

                    // This loop will delete the last data row
                    else
                    {
                        foreach (var child in ExerciseContent.Children.ToList().Where(child => Grid.GetRow(child) == ExerciseContent.RowDefinitions.Count - 2))
                        {
                            ExerciseContent.Children.Remove(child);
                        }

                        foreach (var child in ExerciseContent.Children.ToList().Where(child => Grid.GetRow(child) > ExerciseContent.RowDefinitions.Count - 2))
                        {
                            //ExerciseContent.Children.Remove(child);
                            Grid.SetRow(child, Grid.GetRow(child) - 1);
                        }

                        ExerciseContent.RowDefinitions.RemoveAt(ExerciseContent.RowDefinitions.Count - 2);
                    }

                    // This removed it from the list completely.
                    var getObjectToIndex = BuildingRoutine.Where<RoutineList>(x => x.ExerciseNumber == listOfSetWithSameExerciseNumber[lastListElement].ExerciseNumber && x.SetNumber == listOfSetWithSameExerciseNumber[lastListElement].SetNumber).Single<RoutineList>();
                    int indexToDelete = BuildingRoutine.IndexOf(getObjectToIndex);

                    listOfSetWithSameExerciseNumber.RemoveAt(lastListElement);
                    BuildingRoutine.RemoveAt(indexToDelete);

                    if (BuildingRoutine.Count == 1)
                    {
                        RemoveSet.IsVisible = false;
                    }
                    if (BuildingRoutine.Count == 2)
                    {
                        RemoveSet.IsVisible = true;
                    }
                }

            };
        }

        private void AddSetClick(Grid ExerciseContent, List<RoutineList> listOfSetWithSameExerciseNumber, Grid ExerciseHeader, Frame ExerciseFrame)
        {
            var RowToAdd = new RoutineList();

            var lastListElement = listOfSetWithSameExerciseNumber.Count - 1;
            var lastGridRowElement = ExerciseContent.RowDefinitions.Count - 1;

            RowToAdd.ExerciseDescription = listOfSetWithSameExerciseNumber[lastListElement].ExerciseDescription;
            RowToAdd.ExerciseNumber = listOfSetWithSameExerciseNumber[lastListElement].ExerciseNumber;
            RowToAdd.KeepRow = false;
            RowToAdd.Reps = listOfSetWithSameExerciseNumber[lastListElement].Reps;
            RowToAdd.RestTimeAfterSet = listOfSetWithSameExerciseNumber[lastListElement].RestTimeAfterSet;
            RowToAdd.RoutineID = listOfSetWithSameExerciseNumber[lastListElement].RoutineID;
            RowToAdd.RoutineName = listOfSetWithSameExerciseNumber[lastListElement].RoutineName;
            RowToAdd.SetNumber = listOfSetWithSameExerciseNumber[lastListElement].SetNumber + 1;
            RowToAdd.Weight = listOfSetWithSameExerciseNumber[lastListElement].Weight;

            listOfSetWithSameExerciseNumber.Add(RowToAdd);

            foreach (var child in ExerciseContent.Children.ToList().Where(child => Grid.GetRow(child) == lastGridRowElement))
            {
                ExerciseContent.Children.Remove(child);
            }

            foreach (var child in ExerciseContent.Children.ToList().Where(child => Grid.GetRow(child) > lastGridRowElement))
            {
                //ExerciseContent.Children.Remove(child);
                Grid.SetRow(child, Grid.GetRow(child) - 1);
            }

            ExerciseContent.RowDefinitions.RemoveAt(lastGridRowElement);


            lastListElement = listOfSetWithSameExerciseNumber.Count - 1;


            BuildingRoutine.Add(RowToAdd);

            AddRow(listOfSetWithSameExerciseNumber, ExerciseContent, lastListElement, ExerciseHeader, ExerciseFrame);
        }

        private async void FinishRoutine()
        {

            var answer = await DisplayAlert("Confirm", "Are you finished with your routine", "Yes", "No");
            if (answer) // Yes
            {
                bool weightValid = true;
                bool repValid = true;

                for (int i = 0; i < BuildingRoutine.Count; i++)
                {
                    if (BuildingRoutine[i].Reps == null || BuildingRoutine[i].Reps == "")
                    {
                        repValid = false;
                    }

                    if (BuildingRoutine[i].Weight == null || BuildingRoutine[i].Weight == "")
                    {
                        weightValid = false;
                    }

                }

                if (RoutineName.Text == null)
                {
                    await DisplayAlert("Invalid Routine Name", $"Please add a routine name", "OK");
                }

                else if (weightValid == false)
                {
                    await DisplayAlert("Fill in all weight fields", $"There are weight fields that are not filled", "OK");
                }
                else if (repValid == false)
                {
                    await DisplayAlert("Fill in all rep fields", $"There are rep fields that are not filled", "OK");
                }
                else
                {
                    BuildingRoutine[0].RoutineName = RoutineName.Text;
                    BuildingRoutine[0].UserID = UserList[0].UserId.ToString();
                    InsertRoutine();
                }
            }


        }                      
       
        private async void InsertRoutine()
        {
            //var passAsArray = selectedExercises.ToArray();

            var client = new HttpClient();
            var jsonContent = JsonNet.Serialize(BuildingRoutine);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            // API call to webservice to save user in the database
            var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/AddNewRoutine", httpContent);
            var responseString = await response.Content.ReadAsStringAsync();

            await DisplayAlert("Routine Added!", $"{RoutineName.Text} has been successfully created!", "OK");

            await Navigation.PushAsync(new Routines(UserList));
        }

        override protected bool OnBackButtonPressed()
        {

            LeavePageCheck();

            return true;
        }

        private async void LeavePageCheck()
        {
            var answer = await DisplayAlert("Confirm", "Are you sure you want to cancel creating your routine?", "Yes", "No");
            if (answer) // Yes
            {
                await Navigation.PushAsync(new Routines(UserList));
            }
        }

    }
}