using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TrackWorkout.Entitys;
using TrackWorkout.Pages.AddExercise;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutineInProgress : ContentPage
    {
        int timeLeft;
        public System.Timers.Timer timer = new System.Timers.Timer(1000);
        //public System.Threading.Timer timer = new System.Threading.Timer();
        List<RoutineList> ModifiedRoutine;
        List<RoutineList> AddedDuringRoutine;

        AddExercise.Single myPage;

        //ExerciseList addNewObject;

        // Create grid on this loop
        Grid HeaderContent = new Grid
        {
            RowSpacing = 0,
            ColumnSpacing = 0,
            BackgroundColor = Color.FromHex("#5bc2dc"),
            ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = 35 },
                            new ColumnDefinition { Width = 90 },
                            new ColumnDefinition { Width = 35 },
                            new ColumnDefinition { Width = 70 }
                        }
        };

        Button FinishButton = new Button
        {
            Text = "Finish",
            TextColor = Color.White,
            BackgroundColor = Color.FromHex("#5bc2dc")
        };

        Label TimerLabel = new Label
        {
            FontSize = 15,
            Text = "+",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            TextColor = Color.White
        };
        
        Button AddTimer = new Button
        {
            FontSize = 20,
            Text = "+",
            HorizontalOptions = LayoutOptions.Start,            
            VerticalOptions = LayoutOptions.Center,
            TextColor = Color.White,
            BackgroundColor = Color.FromHex("#5bc2dc")
        };

        Button MinusTimer = new Button
        {
            FontSize = 30,
            Text = "-",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            TextColor = Color.White,
            BackgroundColor = Color.FromHex("#5bc2dc")
        };

        List<UserInformation> userOnBack = new List<UserInformation>();

        public RoutineInProgress(List<RoutineList> routineList, List<UserInformation> userInformation)
        {
            InitializeComponent();

            userOnBack = userInformation;

            ModifiedRoutine = routineList;

            AddedDuringRoutine = new List<RoutineList>();

            foreach (var ObjectToUpdate in ModifiedRoutine.Where(x => x.KeepRow == false))
            {
                ObjectToUpdate.KeepRow = false;
            }
            
            NavigationPage.SetHasNavigationBar(this, false);

            TimerLabel.Text = "";
            AddTimer.IsVisible = false;
            MinusTimer.IsVisible = false;
            //AddTimer.IsEnabled = false;
            //MinusTimer.IsEnabled = false;

            timer.Interval = 1000;
            timer.Elapsed += timer_Tick;

            string HeaderLabelText = routineList.Min(minRoutineName => minRoutineName.RoutineName);
            // Create Header Label
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

            // Create Frame
            Frame HeaderFrame = new Frame
            {
                HasShadow = true,
                Padding = 0,
            };

            

            // Add Row to Grid that will contain the headers. Then add the headers to the grid
            HeaderContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60, GridUnitType.Absolute) });
            HeaderContent.Children.Add(HeaderLabel, 0, 0);
            HeaderContent.Children.Add(MinusTimer, 1, 0);
            HeaderContent.Children.Add(TimerLabel, 2, 0);
            HeaderContent.Children.Add(AddTimer, 3, 0);
            HeaderContent.Children.Add(FinishButton, 4, 0);

            // Add Grid to Frame
            HeaderFrame.Content = HeaderContent;

            // Add Frame to page
            HeaderLayout.Children.Add(HeaderFrame);

            AddTimer.Clicked += (o, e) =>
            {
                timeLeft = timeLeft + 15;
                TimerLabel.Text = "Rest Time: " + timeLeft.ToString();
            };

            MinusTimer.Clicked += (o, e) =>
            {
                timeLeft = timeLeft - 15;
                TimerLabel.Text = "Rest Time: " + timeLeft.ToString();
            };

            FinishButton.Clicked += (o, e) =>
            {               
                FinishRoutine();
            };

            BuildRoutine(1);
        }

        private void BuildRoutine(int Type)
        {
            List<int> uniqueExerciseNumber;

            if (Type == 1)
            {
                // Create new list with unique routine id
                uniqueExerciseNumber = ModifiedRoutine.Select(x => x.ExerciseNumber).AsParallel().Distinct().ToList();
                uniqueExerciseNumber.Sort();
            }
            else
            {
                uniqueExerciseNumber = AddedDuringRoutine.Select(x => x.ExerciseNumber).AsParallel().Distinct().ToList();
                uniqueExerciseNumber.Sort();
            }
                      
            for (int i = 0; i < uniqueExerciseNumber.Count; i++)
            {
                List<RoutineList> listOfSetWithSameExerciseNumber;
                string ExerciseLabelText;
                // I need to set the set ID of the new object added then use that to control the GridCount
                if (Type == 1)
                {
                    listOfSetWithSameExerciseNumber = ModifiedRoutine.Where(value => value.ExerciseNumber == uniqueExerciseNumber[i]).ToList();
                    ExerciseLabelText = listOfSetWithSameExerciseNumber.Min(minExerciseDescription => minExerciseDescription.ExerciseDescription);
                }
                else
                {
                    listOfSetWithSameExerciseNumber = AddedDuringRoutine.Where(value => value.ExerciseNumber == uniqueExerciseNumber[i]).ToList();
                    ExerciseLabelText = listOfSetWithSameExerciseNumber.Min(minExerciseDescription => minExerciseDescription.ExerciseDescription);
                }

                // Create Name Label
                Label ExerciseLabel = new Label
                {
                    Text = ExerciseLabelText,
                    FontSize = 15,
                    HeightRequest = 20,
                    Margin = new Thickness(15, 0, 0, 0),
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
                

                Grid ExerciseHeader = new Grid
                {
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    BackgroundColor = Color.FromHex("#f5f5f5"),
                    ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star }
                            //new ColumnDefinition {Width = 150 }
                        }
                };

                ExerciseHeader.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Absolute) });
                ExerciseHeader.Children.Add(ExerciseLabel, 0, 0);
                //ExerciseHeader.Children.Add(AddSetButton, 1, 0);

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
                FillGrid(listOfSetWithSameExerciseNumber, ExerciseContent, ExerciseHeader, ExerciseFrame, Type);

                // Add Grid to Frame
                ExerciseFrame.Content = ExerciseContent;

                // Add Frame to page
                gridStack.Children.Add(ExerciseFrame);

            }
        }

        // Use to fill the exercise info
        private void FillGrid(List<RoutineList> listOfSetWithSameExerciseNumber, Grid ExerciseContent, Grid ExerciseHeader, Frame ExerciseFrame, int Type)
        {
            for (int i = 0; i < listOfSetWithSameExerciseNumber.Count; i++)
            {
                AddRow(listOfSetWithSameExerciseNumber, ExerciseContent, i, ExerciseHeader, ExerciseFrame, Type);
            }


        }

        private void AddRow(List<RoutineList> listOfSetWithSameExerciseNumber, Grid ExerciseContent, int i, Grid ExerciseHeader, Frame ExerciseFrame, int Type)
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

            if (Type != 1)
            {
                ModifiedRoutine.Add(newRoutineObject);
            }

            Entry WeightEntry = new Entry
            {
                Text = newRoutineObject.Weight.ToString(),
                FontSize = 15,
                Keyboard = Keyboard.Numeric,
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
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                MaxLength = 3,
                TextColor = Color.Gray,
                BackgroundColor = Color.White
            };
            Frame ButtonFrame = new Frame
            {
                Padding = 2,
                HasShadow = true,
                CornerRadius = 45,
                Margin = new Thickness(0, 0, 0, 0),
                BackgroundColor = Color.Black,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            Button CompleteSet = new Button
            {
                CornerRadius = 45,
                WidthRequest = 20,
                HeightRequest = 20,
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


            if (listOfSetWithSameExerciseNumber[i].KeepRow == true)
            {
                CompleteSet.BackgroundColor = Color.Green;
            }

            int RestTime = newRoutineObject.RestTimeAfterSet;

            ButtonFrame.Content = CompleteSet;

            ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });

            ExerciseContent.Children.Add(WeightEntry, 0, i + 1);
            ExerciseContent.Children.Add(RepEntry, 1, i + 1);
            ExerciseContent.Children.Add(ButtonFrame, 2, i + 1);

            if (i == listOfSetWithSameExerciseNumber.Count - 1)
            {
                ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });

                ExerciseContent.Children.Add(AddSet, 0, i + 2);
                ExerciseContent.Children.Add(RemoveSet, 1, i + 2);
            }

            CompleteSet.Clicked += (o, e) =>
            {
                if (CompleteSet.BackgroundColor == Color.White)
                {
                    CompleteSet.BackgroundColor = Color.Green;

                    // if(newRoutineObject)

                    foreach (var ObjectToUpdate in ModifiedRoutine.Where(ExerciseID => ExerciseID.ExerciseNumber == newRoutineObject.ExerciseNumber).Where(SetID => SetID.SetNumber == newRoutineObject.SetNumber))
                    {
                        ObjectToUpdate.KeepRow = true;
                    }

                    timeLeft = RestTime;
                    TimerLabel.Text = "Rest Time: " + timeLeft.ToString();
                    //AddTimer.IsEnabled = true;
                    AddTimer.IsVisible = true;
                    //MinusTimer.IsEnabled = true;
                    MinusTimer.IsVisible = true;

                    timer.Start();
                }
                else
                {
                    CompleteSet.BackgroundColor = Color.White;

                    foreach (var ObjectToUpdate in ModifiedRoutine.Where(ExerciseID => ExerciseID.ExerciseNumber == newRoutineObject.ExerciseNumber).Where(SetID => SetID.SetNumber == newRoutineObject.SetNumber))
                    {
                        ObjectToUpdate.KeepRow = false;
                    }

                    timer.Stop();
                    TimerLabel.Text = "";
                }

            };

            WeightEntry.Unfocused += (o, e) =>
            {
                foreach (var ObjectToUpdate in ModifiedRoutine.Where(ExerciseID => ExerciseID.ExerciseNumber == newRoutineObject.ExerciseNumber).Where(SetID => SetID.SetNumber == newRoutineObject.SetNumber))
                {
                    ObjectToUpdate.Weight = WeightEntry.Text;
                }

            };

            RepEntry.Unfocused += (o, e) =>
            {
                foreach (var ObjectToUpdate in ModifiedRoutine.Where(ExerciseID => ExerciseID.ExerciseNumber == newRoutineObject.ExerciseNumber).Where(SetID => SetID.SetNumber == newRoutineObject.SetNumber))
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
                if (ModifiedRoutine.Count == 1)
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

                        //ExerciseHeader.RowDefinitions.RemoveAt(1);
                        ExerciseHeader.Children.RemoveAt(0);
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
                    var getObjectToIndex = ModifiedRoutine.Where<RoutineList>(x => x.ExerciseNumber == listOfSetWithSameExerciseNumber[lastListElement].ExerciseNumber && x.SetNumber == listOfSetWithSameExerciseNumber[lastListElement].SetNumber).Single<RoutineList>();
                    int indexToDelete = ModifiedRoutine.IndexOf(getObjectToIndex);

                    listOfSetWithSameExerciseNumber.RemoveAt(lastListElement);
                    ModifiedRoutine.RemoveAt(indexToDelete);

                    if (ModifiedRoutine.Count == 1)
                    {
                        RemoveSet.IsVisible = false;
                    }
                    if (ModifiedRoutine.Count == 2)
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


            ModifiedRoutine.Add(RowToAdd);            

            AddRow(listOfSetWithSameExerciseNumber, ExerciseContent, lastListElement, ExerciseHeader, ExerciseFrame, 1);
        }

        // When hitting the start routine button a pop up appears asking for confirmation to start. Once confirmed the routine will begin
        private async void FinishRoutine()
        {

            var answer = await DisplayAlert("Confirm", "Are you finished with your routine", "Yes", "No");
            if (answer) // Yes
            {
                timer.Stop();

                await Navigation.PushAsync(new RoutineSummary(ModifiedRoutine, userOnBack));                  
            }


        }

        private async void PressAddExercise(object sender, SelectedItemChangedEventArgs e)
        {
            // Use the Email object to get the rest of the data set
            var client = new HttpClient();
            // API call to webservice to save user in the database
            var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetExerciseList", null);
            var responseString = await response.Content.ReadAsStringAsync();
            var exerciseList = JsonConvert.DeserializeObject<ExerciseListRootObject>(responseString);

            //Handle popup page 
            Application.Current.ModalPopping += HandleModalPopping;
            myPage = new AddExercise.Single(exerciseList);           

            await Application.Current.MainPage.Navigation.PushModalAsync(myPage);                        
        }

        private void HandleModalPopping(object sender, ModalPoppingEventArgs e)
        {

            if (e.Modal == myPage)
            {
                // now we can retrieve that phone number:
                var PageExerciseObject = myPage.returnExercise;

                RoutineList NewExerciseObject = new RoutineList();

                NewExerciseObject.ExerciseDescription = PageExerciseObject.ExerciseDescription;
                NewExerciseObject.KeepRow = false;
                NewExerciseObject.Reps = "0";
                NewExerciseObject.RestTimeAfterSet = 30;
                NewExerciseObject.SetNumber = 1;
                NewExerciseObject.Weight = "0";

                var CurrentMaxExerciseNumber = ModifiedRoutine.Max(maxExerciseNumber => maxExerciseNumber.ExerciseNumber);
                NewExerciseObject.ExerciseNumber = CurrentMaxExerciseNumber + 1;

                NewExerciseObject.RoutineID = ModifiedRoutine.Min(RoutineID => RoutineID.RoutineID);

                NewExerciseObject.RoutineName = ModifiedRoutine.Min(RoutineName => RoutineName.RoutineName);

                AddedDuringRoutine.Add(NewExerciseObject);

                BuildRoutine(2);

                // remember to remove the event handler:
                myPage = null;
                AddedDuringRoutine = new List<RoutineList>();
                Application.Current.ModalPopping -= HandleModalPopping;
                
            }
            
        }

        private async void testing(ExerciseList test)
        {
            await DisplayAlert("Test", $"Value: {test.ExerciseDescription}", "OK");
        }


        private void timer_Tick(object sender, ElapsedEventArgs e)
        {
            // Display the new time left
            // by updating the TimerLabel label.
            timeLeft--;
            // When using Timers and UI be sure to invoke on the main thread or the UI will not update.
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { TimerLabel.Text = "Rest Time: " + timeLeft.ToString(); });

            //Load sounds
            var soundOne = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            soundOne.Load("Beep2.wav");
            

            if (timeLeft == 10)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { HeaderContent.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { FinishButton.BackgroundColor = Color.FromHex("#5bc2dc"); });
            }
            if (timeLeft == 9)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { HeaderContent.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { FinishButton.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.BackgroundColor = Color.FromHex("#5bdcb6"); });
            }
            if (timeLeft == 8)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { HeaderContent.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { FinishButton.BackgroundColor = Color.FromHex("#5bc2dc"); });
            }
            if (timeLeft == 7)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { HeaderContent.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { FinishButton.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.BackgroundColor = Color.FromHex("#5bdcb6"); });
            }
            if (timeLeft == 6)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { HeaderContent.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { FinishButton.BackgroundColor = Color.FromHex("#5bc2dc"); });
            }

            if (timeLeft == 5)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { HeaderContent.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { FinishButton.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.BackgroundColor = Color.FromHex("#5bdcb6"); });
            }

            if (timeLeft == 4)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { HeaderContent.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { FinishButton.BackgroundColor = Color.FromHex("#5bc2dc"); });
            }

            if (timeLeft == 3)
            {
                soundOne.Play();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { HeaderContent.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { FinishButton.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.BackgroundColor = Color.FromHex("#5bdcb6"); });
            }

            if (timeLeft == 2)
            {
                soundOne.Play();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.BackgroundColor = Color.FromHex("#5bc2dc"); });                
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { HeaderContent.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { FinishButton.BackgroundColor = Color.FromHex("#5bc2dc"); });
            }

            if (timeLeft == 1)
            {
                soundOne.Play();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { HeaderContent.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { FinishButton.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.BackgroundColor = Color.FromHex("#5bdcb6"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.BackgroundColor = Color.FromHex("#5bdcb6"); });
            }

            if (timeLeft <= 0)
            {
                timer.Stop();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { HeaderContent.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { FinishButton.BackgroundColor = Color.FromHex("#5bc2dc"); });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { TimerLabel.Text = ""; });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.IsVisible = false; });
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.IsVisible = false; });
                soundOne.Load("Beep.wav");
                soundOne.Play();
            }
        }

        override protected bool OnBackButtonPressed()
        {

            LeavePageCheck();

            return true;            
        }

        private async void LeavePageCheck()
        {
            var answer = await DisplayAlert("Confirm", "Are you sure you want to cancel your routine?", "Yes", "No");
            if (answer) // Yes
            {
                timer.Stop();

                await Navigation.PushAsync(new Routines(userOnBack));
            }
        }

    }
}