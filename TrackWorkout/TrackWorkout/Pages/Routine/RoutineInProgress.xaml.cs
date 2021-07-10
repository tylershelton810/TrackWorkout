﻿using Rg.Plugins.Popup.Extensions;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;
using TrackWorkout.CustomRenderer;
using TrackWorkout.Entitys;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.Routine
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutineInProgress : ContentPage
    {
        int timeLeft;
        int startTime;
        string formattedTimeLeft;
        string timerExercise;
        public System.Timers.Timer timer = new System.Timers.Timer(1000);
        //public System.Threading.Timer timer = new System.Threading.Timer();
        bool BarbellMenuSet;
        XDocument doc;
        XDocument lastRoutine;
        List<Microcharts.Entry> timerEntries = new List<Microcharts.Entry>();

        AddExercise.Single myPage;

        Frame Filler = new Frame
        {
            BackgroundColor = Color.Transparent,
            HasShadow = false,
            HeightRequest = 70
        };

        Label TimerLabel = new Label
        {
            FontSize = 10,
            Text = "+",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            TextColor = Color.White,
            BackgroundColor = Color.Transparent
        };

        BoxView ClockBackground = new BoxView
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = App.SecondaryThemeColor
        };

        Label ClockDisplay = new Label
        {
            FontSize = 25,
            FontAttributes = FontAttributes.Bold,
            TextColor = App.PrimaryThemeColor,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        Button AddTimer = new Button
        {
            FontSize = 15,
            Text = "+",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            TextColor = Color.White,
            BackgroundColor = Color.Transparent
        };

        Button MinusTimer = new Button
        {
            FontSize = 15,
            Text = "-",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            TextColor = Color.White,
            BackgroundColor = Color.Transparent
        };



        string FromSourceToPass;
        string StartSourceToPass;

        Size screenSize = Device.Info.PixelScreenSize;
        RoutineList oldRoutine;

        public RoutineInProgress(RoutineList routine, string FromSource, string StartSource)
        {
            InitializeComponent();

            TimerGrid.TranslationX = ((screenSize.Width / 2) * -1);
            TimerGrid.IsVisible = false;
            //Place to the right
            RestClock.TranslationX = ((screenSize.Width / 2) * 2);
            RestClock.IsVisible = false;
            ButtonGridBottom.BackgroundColor = App.PrimaryThemeColor;
            TimerGridThemeColor.BackgroundColor = App.PrimaryThemeColor;
            addButtonFrame.BackgroundColor = App.PrimaryThemeColor;
            addButtonFrame2.BackgroundColor = App.SecondaryThemeColor;
            AddExerciseButtonButtonAction.BackgroundColor = App.SecondaryThemeColor;
            BarbellButton.BackgroundColor = App.PrimaryThemeColor;

            // Create a document to hold the edited routine and to control the UI
            doc = XDocument.Parse(routine.RoutineDetail);

            // Store old routine to be compared later
            oldRoutine = routine;

            // Create a document to hold the last time this routing was complete. This
            // will be used to display previous results to help guide the routine. 
            // Coach should be able to turn this off
            if (App.historyInformationApp.Count > 0)
            {
                foreach (HistoryInformation HistoryRecord in App.historyInformationApp)
                {
                    if (HistoryRecord.RoutineID == routine.RoutineID)
                    {
                        lastRoutine = XDocument.Parse(HistoryRecord.HistoryXML);

                        break;
                    }
                }
            }

            // Move the bar weight loader to the left of screen
            BarbellGrid.TranslationX = ((screenSize.Width / 2) * -1);

            // Hide the bar weight loader
            BarbellMenuSet = false;

            // set up the bar weight loader
            number45Count.SelectedIndex = 0;
            number35Count.SelectedIndex = 0;
            number25Count.SelectedIndex = 0;
            number10Count.SelectedIndex = 0;
            number5Count.SelectedIndex = 0;
            number2Point5Count.SelectedIndex = 0;
            WeightDropDown.SelectedIndex = 0;

            FromSourceToPass = FromSource;
            StartSourceToPass = StartSource;

            bool alreadyHasKeepRow = false;

            try
            {
                foreach (var exercise in doc.Element("Routine").Elements("Exercise"))
                {
                    foreach (var set in exercise.Elements("Set"))
                    {
                        if (set.Element("KeepRow").Value == "1" || set.Element("KeepRow").Value == "0")
                        {
                            alreadyHasKeepRow = true;
                        }
                    }
                }
            }
            catch
            {
                alreadyHasKeepRow = false;
            }

            if (alreadyHasKeepRow == false)
            {
                // Set KeepRow to false for every set
                foreach (var exercise in doc.Element("Routine").Elements("Exercise"))
                {
                    foreach (var set in exercise.Elements("Set"))
                    {
                        set.Add(new XElement("KeepRow", 0));
                    }
                }
            }

            // Hide system nav
            NavigationPage.SetHasNavigationBar(this, false);

            // Set up variables for rep timer
            TimerLabel.Text = "";
            AddTimer.IsVisible = false;
            MinusTimer.IsVisible = false;
            timer.Interval = 1000;
            timer.Elapsed += timer_Tick;

            // Create Header Label
            HeaderLabel.Text = doc.Element("Routine").Element("Name").Value;
            double timerWidth = screenSize.Width / 12;

            RestClock.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });



            if (FromSourceToPass == "Coach" && oldRoutine.Locked == true)
            {
                addButtonFrame2.Opacity = 0.5;
            }
            else
            {
                RestClock.Children.Add(ClockBackground, 0, 0);
                Grid.SetColumnSpan(ClockBackground, 3);
                RestClock.Children.Add(MinusTimer, 0, 0);
                RestClock.Children.Add(TimerLabel, 1, 0);
                RestClock.Children.Add(AddTimer, 2, 0);
            }

            //When the add timer is pressed
            AddTimer.Clicked += (o, e) =>
            {
                foreach (var exercise in doc.Element("Routine").Elements("Exercise"))
                {
                    if (exercise.Element("Description").Value == timerExercise)
                    {
                        exercise.Element("RestTimeAfterSet").SetValue(Int32.Parse(exercise.Element("RestTimeAfterSet").Value) + 15);
                    }
                }
                timeLeft = timeLeft + 15;
                startTime = timeLeft;

                var timespan = TimeSpan.FromSeconds(timeLeft);
                formattedTimeLeft = timespan.ToString(@"mm\:ss");

                // When using Timers and UI be sure to invoke on the main thread or the UI will not update.                
                TimerLabel.Text = formattedTimeLeft;

                if (timeLeft >= 16)
                {
                    //MinusTimer.IsEnabled = true;
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.IsVisible = true; });
                }
                if (timeLeft >= 285)
                {
                    //AddTimer.IsEnabled = false;
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.IsVisible = false; });
                }
            };

            //When the minus timer is pressed
            MinusTimer.Clicked += (o, e) =>
            {
                foreach (var exercise in doc.Element("Routine").Elements("Exercise"))
                {
                    if (exercise.Element("Description").Value == timerExercise)
                    {
                        exercise.Element("RestTimeAfterSet").SetValue(Int32.Parse(exercise.Element("RestTimeAfterSet").Value) - 15);
                    }
                }
                timeLeft = timeLeft - 15;

                var timespan = TimeSpan.FromSeconds(timeLeft);
                formattedTimeLeft = timespan.ToString(@"mm\:ss");


                TimerLabel.Text = formattedTimeLeft;

                if (timeLeft <= 15)
                {
                    //MinusTimer.IsEnabled = false;
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.IsVisible = false; });
                }
                if (timeLeft <= 284)
                {
                    //AddTimer.IsEnabled = true;
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.IsVisible = true; });
                }
            };

            // Begin building the page
            BuildRoutine(1);


            // Add blank space to the bottom of the page to make room for the button grid.
            gridStack.Children.Add(Filler);

            // handle the bar weight loader
            WeightDropDown.Unfocused += (o, e) =>
            {
                ClearWeightToolAsync();
            };

            number45Count.Unfocused += (o, e) =>
            {
                if (WeightDropDown.SelectedIndex != 0)
                {
                    ClearWeightToolAsync();
                }
            };

            number35Count.Unfocused += (o, e) =>
            {
                if (WeightDropDown.SelectedIndex != 0)
                {
                    ClearWeightToolAsync();
                }
            };

            number25Count.Unfocused += (o, e) =>
            {
                if (WeightDropDown.SelectedIndex != 0)
                {
                    ClearWeightToolAsync();
                }
            };

            number10Count.Unfocused += (o, e) =>
            {
                if (WeightDropDown.SelectedIndex != 0)
                {
                    ClearWeightToolAsync();
                }
            };

            number5Count.Unfocused += (o, e) =>
            {
                if (WeightDropDown.SelectedIndex != 0)
                {
                    ClearWeightToolAsync();
                }
            };

            number2Point5Count.Unfocused += (o, e) =>
            {
                if (WeightDropDown.SelectedIndex != 0)
                {
                    ClearWeightToolAsync();
                }
            };
        }

        //When the finish button is pressed       
        private void FinishClick(object sender, EventArgs e)
        {
            FinishRoutine();
        }

        private async void TimerBackButtonPress(object sender, EventArgs e)
        {
            TimerForward.IsVisible = true;
            await Task.WhenAll(
                TimerGrid.TranslateTo((screenSize.Width / 2) * -1, 0, 150)
                );
            TimerGrid.IsVisible = false;
        }

        private async void TimerForwardPress(object sender, EventArgs e)
        {
            TimerGrid.IsVisible = true;

            await Task.WhenAll(
                TimerGrid.TranslateTo(0, 0, 150)
                );
            TimerForward.IsVisible = false;
        }
        private void AddFrameClick(object sender, EventArgs e)
        {
            PressAddExercise(sender, e);
        }

        private async Task ClearWeightToolAsync() //added await?
        {
            foreach (var valueProperty in BarbellGrid.Children.ToList())
            {
                if (valueProperty.GetType() == typeof(Label) || valueProperty.GetType() == typeof(Frame))
                {
                    BarbellGrid.Children.Remove(valueProperty);
                }
            }

            await CalculateWeightAsync(); //added await?
        }

        private void BuildRoutine(int Type)
        {
            // Create a node list to loop through to create the page
            IEnumerable<XElement> loopThrough;
            int SupersetID = 0;

            // 1 means it is on load of the page while a 2 indicates it was an added exercise
            if (Type == 1)
            {
                loopThrough = doc.Element("Routine")
                                 .Elements("Exercise")
                                 .OrderBy(x => x.Element("Number").Value);
            }
            else
            {
                // Get max exercise creating a grid only for the new exercise
                var maxExercise = doc.Element("Routine").Elements("Exercise").Max(Number => Int32.Parse(Number.Element("Number").Value));
                loopThrough = doc.Element("Routine").Elements("Exercise").Where(Number => Int32.Parse(Number.Element("Number").Value) == maxExercise);
            }


            // Loop through creating grids for the each exercise
            foreach (var exercise in loopThrough)
            {
                if (exercise.Attribute("SupersetID").Value == "0")
                {
                    //Loop through to create group set or single exercises
                    var dataToPass = CreateSingleExerciseGroup(exercise, Type);

                    AddFrameToPage(dataToPass);
                }
                else
                {

                    if (Int32.Parse(exercise.Attribute("SupersetID").Value) != SupersetID)
                    {
                        //Start Creating new superset
                        SupersetID = Int32.Parse(exercise.Attribute("SupersetID").Value);

                        CreateSupersetExerciseGroup(exercise, Type);
                    }
                }
            }
        }
    

        private void CreateSupersetExerciseGroup(XElement exercise, int type)
        {
            //Create a list of all exercises in the superset
            IEnumerable<XElement> exerciseInSuperset = GetSupersetList(exercise);

            int SupersetCount = exerciseInSuperset.Count();
            int currentCount = 1;

            // Create Frame
            Frame ExerciseFrame = new Frame
            {
                HasShadow = false,
                Padding = new Thickness(0, 0, 0, 0),
                Margin = new Thickness(0, 0, 0, 10),
            };

            if (FromSourceToPass == "Coach" && oldRoutine.Locked == true)
            {
                ExerciseFrame.Padding = new Thickness(0, 0, 0, 10);
            }

            // Create grid on this loop
            Grid ExerciseContent = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                BackgroundColor = Color.White,
                ColumnDefinitions = {
                            new ColumnDefinition { Width = 75 },
                            new ColumnDefinition { Width = 75 },
                            new ColumnDefinition { Width = 75 },
                            new ColumnDefinition { Width = 75 },
                            new ColumnDefinition { Width = GridLength.Star }
                        }
            };

            foreach (var exerc in exerciseInSuperset)
            {
                bool isLastInSuperset = false;
                bool isFirseInSuperset = false;
                if (currentCount == SupersetCount)
                {
                    isLastInSuperset = true;
                }
                if (currentCount == 1)
                {
                    isFirseInSuperset = true;
                }

                string LastLabel = "";

                try
                {
                    // If this exist it will continue, otherwise it will exeption leaving the Label text blank.
                    XElement HistoryExist = lastRoutine.Descendants("Exercise").Where(x => x.Element("ID").Value == exerc.Element("ID").Value).First();

                    LastLabel = "Last";
                }
                catch
                {
                    // Do Nothing
                }

                // Create Name Label
                Label ExerciseLabel = new Label
                {
                    Text = exerc.Element("Description").Value.TrimStart().TrimEnd(),
                    FontFamily = App.CustomRegular,
                    FontSize = 15,
                    HeightRequest = 20,
                    Margin = new Thickness(15, 0, 0, 0),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = App.PrimaryThemeColor,
                    BackgroundColor = Color.Transparent
                };

                //This will only be added to first in a superset or if it is not a superset. 
                //Create ListMenu Icon
                ImageButton ExerciseOption = new ImageButton
                {
                    Source = "ListMenu.png",
                    HorizontalOptions = LayoutOptions.End,
                    WidthRequest = 25,
                    Margin = new Thickness(0, 10, 30, 0)
                };

                //When the add timer is pressed
                ExerciseOption.Clicked += async (o, e) =>
                {
                // By adding nul here the red option is ignored
                string action = await DisplayActionSheet(exerc.Element("Description").Value + " Options", "Cancel", null, "Superset", "Alter Rest Time");

                    if (action == "Superset")
                    {
                    //Get the superset ID
                    int superSetID = int.Parse(exerc.Attribute("SupersetID").Value);
                    //If the superset ID does not exists then we need to increment the ID from the highest one
                    if (superSetID == 0)
                        {
                            superSetID = doc.Element("Routine").Elements("Exercise").Max(supersetID => Int32.Parse(supersetID.Attribute("SupersetID").Value)) + 1;
                        }
                        action = await DisplayActionSheet(exercise.Element("Description").Value + " Superset", "Cancel", null, "Combine with existing exercise?", "Add new exercise");

                        // Create Exercise List
                        List<ExerciseList> ExerciseListToPass = new List<ExerciseList>();

                        if (action == "Combine with existing exercise?")
                        {
                            //Get list of exercises in the current workout
                            foreach (var holder in doc.Element("Routine").Elements("Exercise"))
                            {
                                //Ensure that the exercise where the options button was clicked does not make it in.
                                if (exercise.Element("Description").Value != holder.Element("Description").Value)
                                {
                                    ExerciseList Obj = new ExerciseList
                                    {
                                        ExerciseID = int.Parse(App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(holder.Element("ID").Value)).First().ExerciseID.ToString()),
                                        FirstLetter = char.Parse(App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(holder.Element("ID").Value)).First().FirstLetter.ToString()),
                                        ExerciseDescription = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(holder.Element("ID").Value)).First().ExerciseDescription,
                                        FirstMuscleCode = int.Parse(App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(holder.Element("ID").Value)).First().FirstMuscleCode.ToString()),
                                        FirstMuscleCodeDescription = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(holder.Element("ID").Value)).First().FirstMuscleCodeDescription,
                                        SecondMuscleCode = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(holder.Element("ID").Value)).First().SecondMuscleCode,
                                        SecondMuscleCodeDescription = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(holder.Element("ID").Value)).First().SecondMuscleCodeDescription,
                                        ThirdMuscleCode = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(holder.Element("ID").Value)).First().ThirdMuscleCode,
                                        ThirdMuscleCodeDescription = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(holder.Element("ID").Value)).First().ThirdMuscleCodeDescription
                                    };

                                    ExerciseListToPass.Add(Obj);
                                }
                            }                            
                        }
                        else if (action == "Add new exercise")
                        {
                            //Current Object to remove from the passed in list. 
                            ExerciseList Obj = new ExerciseList
                            {
                                ExerciseID = int.Parse(App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().ExerciseID.ToString()),
                                FirstLetter = char.Parse(App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().FirstLetter.ToString()),
                                ExerciseDescription = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().ExerciseDescription,
                                FirstMuscleCode = int.Parse(App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().FirstMuscleCode.ToString()),
                                FirstMuscleCodeDescription = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().FirstMuscleCodeDescription,
                                SecondMuscleCode = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().SecondMuscleCode,
                                SecondMuscleCodeDescription = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().SecondMuscleCodeDescription,
                                ThirdMuscleCode = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().ThirdMuscleCode,
                                ThirdMuscleCodeDescription = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().ThirdMuscleCodeDescription
                            };

                            //Get the total list of exercises and subtract out the current one to be passed to the single page. 
                            ExerciseListToPass = App.exerciseListApp;
                            ExerciseListToPass.Remove(Obj);
                        }

                        //Handle popup page 
                        Application.Current.ModalPopping += HandleModalPopping;
                        myPage = new AddExercise.Single(ExerciseListToPass, "Superset", superSetID, exercise);

                        await Application.Current.MainPage.Navigation.PushModalAsync(myPage);
                    }
                    else if (action == "Alter Rest Time")
                    {

                    }
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

                // Create Last Weight Label
                Label LastWeightLabel = new Label
                {
                    Text = LastLabel,
                    FontFamily = App.CustomBold,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 15,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.LightGray
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
                    TextColor = Color.Gray,
                    BackgroundColor = Color.Transparent
                };

                // Create Last Reps Label
                Label LastRepsLabel = new Label
                {
                    Text = LastLabel,
                    FontFamily = App.CustomBold,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 15,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.LightGray,
                    BackgroundColor = Color.Transparent
                };

                // Create grid for exercise label
                Grid ExerciseHeader = new Grid
                {
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    BackgroundColor = Color.Transparent,
                    ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star }
                        }
                };

                if (isFirseInSuperset)
                {
                    ExerciseHeader.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Absolute) });
                    ExerciseHeader.Children.Add(ExerciseLabel, 0, 0);

                    ExerciseHeader.Children.Add(ExerciseOption, 1, 0);
                    // Add Exercise Name to the Screen
                    gridStack.Children.Add(ExerciseHeader);
                }
                else
                {
                    //We have to include the header in the Exercise Content Grid after the first one.
                    ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
                    ExerciseContent.Children.Add(ExerciseLabel, 0, ExerciseContent.RowDefinitions.Count() - 1);
                    //Header can span 5 columns
                    Grid.SetColumnSpan(ExerciseLabel, 5);
                }


                // Add Row to Grid that will contain the headers. Then add the headers to the grid
                ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
                ExerciseContent.Children.Add(WeightLabel, 0, ExerciseContent.RowDefinitions.Count() - 1);
                ExerciseContent.Children.Add(LastWeightLabel, 1, ExerciseContent.RowDefinitions.Count() - 1);
                ExerciseContent.Children.Add(RepsLabel, 2, ExerciseContent.RowDefinitions.Count() - 1);
                ExerciseContent.Children.Add(LastRepsLabel, 3, ExerciseContent.RowDefinitions.Count() - 1);

                // Fill the grid with Reps and Weight values
                FillGrid(exerc, ExerciseContent, ExerciseHeader, ExerciseFrame, type, isLastInSuperset, true);

                currentCount++;
            }

            // Add Grid to Frame
            ExerciseFrame.Content = ExerciseContent;
            // Add Frame to page
            gridStack.Children.Add(ExerciseFrame);
        }

        private void AddFrameToPage(Tuple<XElement, Grid, Grid, int> dataToPass)
        {
            // Create Frame
            Frame ExerciseFrame = new Frame
            {
                HasShadow = false,
                Padding = new Thickness(0, 0, 0, 0),
                Margin = new Thickness(0, 0, 0, 10),
            };

            if (FromSourceToPass == "Coach" && oldRoutine.Locked == true)
            {
                ExerciseFrame.Padding = new Thickness(0, 0, 0, 10);
            }

            // Fill the grid with Reps and Weight values
            //Tuple<XElement, Grid, Grid, int>(exercise, ExerciseContent, ExerciseHeader, type);
            FillGrid(dataToPass.Item1, dataToPass.Item2, dataToPass.Item3, ExerciseFrame, dataToPass.Item4);

            // Add Grid to Frame // item2 is the ExerciseContent grid
            ExerciseFrame.Content = dataToPass.Item2;
            // Add Frame to page
            gridStack.Children.Add(ExerciseFrame);
        }

        private Tuple<XElement, Grid, Grid, int> CreateSingleExerciseGroup(XElement exercise, int type, bool superset = false)
        {
            string LastLabel = "";

            try
            {
                // If this exist it will continue, otherwise it will exeption leaving the Label text blank.
                XElement HistoryExist = lastRoutine.Descendants("Exercise").Where(x => x.Element("ID").Value == exercise.Element("ID").Value).First();

                LastLabel = "Last";
            }
            catch
            {
                // Do Nothing
            }

            // Create Name Label
            Label ExerciseLabel = new Label
            {
                Text = exercise.Element("Description").Value.TrimStart().TrimEnd(),
                FontFamily = App.CustomRegular,
                FontSize = 15,
                HeightRequest = 20,
                Margin = new Thickness(15, 0, 0, 0),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                TextColor = App.PrimaryThemeColor,
                BackgroundColor = Color.Transparent
            };

            //This will only be added to first in a superset or if it is not a superset. 
            //Create ListMenu Icon
            ImageButton ExerciseOption = new ImageButton
            {
                Source = "ListMenu.png",
                HorizontalOptions = LayoutOptions.End,
                WidthRequest = 25,
                Margin = new Thickness(0, 10, 30, 0)
            };

            //When the add timer is pressed
            ExerciseOption.Clicked += async (o, e) =>
            {
                    // By adding nul here the red option is ignored
                    string action = await DisplayActionSheet(exercise.Element("Description").Value + " Options", "Cancel", null, "Superset", "Alter Rest Time");

                if (action == "Superset")
                {
                        //Get the superset ID
                        int superSetID = int.Parse(exercise.Attribute("SupersetID").Value);
                        //If the superset ID does not exists then we need to increment the ID from the highest one
                        if (superSetID == 0)
                    {
                        superSetID = doc.Element("Routine").Elements("Exercise").Max(supersetID => Int32.Parse(supersetID.Attribute("SupersetID").Value)) + 1;
                    }
                    action = await DisplayActionSheet(exercise.Element("Description").Value + " Superset", "Cancel", null, "Combine with existing exercise?", "Add new exercise");

                    // Create Exercise List
                    List<ExerciseList> ExerciseListToPass = new List<ExerciseList>();

                    if (action == "Combine with existing exercise?")
                    {                        
                        //Get list of exercises in the current workout
                        foreach (var holder in doc.Element("Routine").Elements("Exercise"))
                        {
                            //Ensure that the exercise where the options button was clicked does not make it in.
                            if (exercise.Element("Description").Value != holder.Element("Description").Value)
                            {
                                ExerciseList Obj = new ExerciseList
                                {
                                    ExerciseID = int.Parse(App.exerciseListApp.Where(x => x.ExerciseID.ToString() == holder.Element("ID").Value).First().ExerciseID.ToString()),
                                    FirstLetter = char.Parse(App.exerciseListApp.Where(x => x.ExerciseID.ToString() == holder.Element("ID").Value).First().FirstLetter.ToString()),
                                    ExerciseDescription = App.exerciseListApp.Where(x => x.ExerciseID.ToString() == holder.Element("ID").Value).First().ExerciseDescription,
                                    FirstMuscleCode = int.Parse(App.exerciseListApp.Where(x => x.ExerciseID.ToString() == holder.Element("ID").Value).First().FirstMuscleCode.ToString()),
                                    FirstMuscleCodeDescription = App.exerciseListApp.Where(x => x.ExerciseID.ToString() == holder.Element("ID").Value).First().FirstMuscleCodeDescription,
                                    SecondMuscleCode = App.exerciseListApp.Where(x => x.ExerciseID.ToString() == holder.Element("ID").Value).First().SecondMuscleCode,
                                    SecondMuscleCodeDescription = App.exerciseListApp.Where(x => x.ExerciseID.ToString() == holder.Element("ID").Value).First().SecondMuscleCodeDescription,
                                    ThirdMuscleCode = App.exerciseListApp.Where(x => x.ExerciseID.ToString() == holder.Element("ID").Value).First().ThirdMuscleCode,
                                    ThirdMuscleCodeDescription = App.exerciseListApp.Where(x => x.ExerciseID.ToString() == holder.Element("ID").Value).First().ThirdMuscleCodeDescription
                                };

                                ExerciseListToPass.Add(Obj);
                            }
                        }                        
                    }
                    else if (action == "Add new exercise")
                    {
                        //Current Object to remove from the passed in list. 
                        ExerciseList Obj = new ExerciseList
                        {
                            ExerciseID = int.Parse(App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().ExerciseID.ToString()),
                            FirstLetter = char.Parse(App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().FirstLetter.ToString()),
                            ExerciseDescription = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().ExerciseDescription,
                            FirstMuscleCode = int.Parse(App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().FirstMuscleCode.ToString()),
                            FirstMuscleCodeDescription = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().FirstMuscleCodeDescription,
                            SecondMuscleCode = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().SecondMuscleCode,
                            SecondMuscleCodeDescription = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().SecondMuscleCodeDescription,
                            ThirdMuscleCode = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().ThirdMuscleCode,
                            ThirdMuscleCodeDescription = App.exerciseListApp.Where(x => x.ExerciseID == Int32.Parse(exercise.Element("ID").Value)).First().ThirdMuscleCodeDescription
                        };

                        //Get the total list of exercises and subtract out the current one to be passed to the single page. 
                        ExerciseListToPass = App.exerciseListApp;
                        //This is not working.......
                        ExerciseListToPass.Remove(Obj);
                    }

                    //Handle popup page 
                    Application.Current.ModalPopping += HandleModalPopping;
                    myPage = new AddExercise.Single(ExerciseListToPass, "Superset", superSetID, exercise);

                    await Application.Current.MainPage.Navigation.PushModalAsync(myPage);
                }
                else if (action == "Alter Rest Time")
                {

                }
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

            // Create Last Weight Label
            Label LastWeightLabel = new Label
            {
                Text = LastLabel,
                FontFamily = App.CustomBold,
                FontAttributes = FontAttributes.Bold,
                FontSize = 15,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                TextColor = Color.LightGray
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
                TextColor = Color.Gray,
                BackgroundColor = Color.Transparent
            };

            // Create Last Reps Label
            Label LastRepsLabel = new Label
            {
                Text = LastLabel,
                FontFamily = App.CustomBold,
                FontAttributes = FontAttributes.Bold,
                FontSize = 15,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                TextColor = Color.LightGray,
                BackgroundColor = Color.Transparent
            };

            

            // Create grid for exercise label
            Grid ExerciseHeader = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                BackgroundColor = Color.Transparent,
                ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star }
                        }
            };

            ExerciseHeader.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
            ExerciseHeader.Children.Add(ExerciseLabel, 0, 0);

            ExerciseHeader.Children.Add(ExerciseOption, 1, 0);
            

            // Add Exercise Name to the Screen
            gridStack.Children.Add(ExerciseHeader);

            // Create grid on this loop
            Grid ExerciseContent = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                BackgroundColor = Color.White,
                ColumnDefinitions = {
                            new ColumnDefinition { Width = 75 },
                            new ColumnDefinition { Width = 75 },
                            new ColumnDefinition { Width = 75 },
                            new ColumnDefinition { Width = 75 },
                            new ColumnDefinition { Width = GridLength.Star }
                        }
            };

            // Add Row to Grid that will contain the headers. Then add the headers to the grid
            ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) });
            ExerciseContent.Children.Add(WeightLabel, 0, 0);
            ExerciseContent.Children.Add(LastWeightLabel, 1, 0);
            ExerciseContent.Children.Add(RepsLabel, 2, 0);
            ExerciseContent.Children.Add(LastRepsLabel, 3, 0);

            return new Tuple<XElement, Grid, Grid, int>(exercise, ExerciseContent, ExerciseHeader, type);

            
        }

        // Use to fill the exercise info
        private void FillGrid(XElement exercise, Grid ExerciseContent, Grid ExerciseHeader, Frame ExerciseFrame, int Type, bool isLastInSuper = false, bool isSuper = false)
        {
            // Loop through each set filling in the rest of the grid
            foreach (var set in exercise.Elements("Set"))
            {
                AddRow(set, exercise, ExerciseContent, ExerciseHeader, ExerciseFrame, Type, isLastInSuper, isSuper);
            }
        }

        private void AddRow(XElement set, XElement exercise, Grid ExerciseContent, Grid ExerciseHeader, Frame ExerciseFrame, int Type, bool isLastInSuper = false, bool isSuper = false)
        {
            // Create new object and fill with XML data.
            RoutineList newRoutineObject = new RoutineList();
            int maxSetForExercise = exercise.Elements("Set").Max(x => (int)x.Element("Number"));

            // Create a node from History where matches exercise and Set  
            XElement HistoryToUse;
            string HistoryWeight = "";
            string HistoryRep = "";

            try
            {
                HistoryToUse = lastRoutine.Descendants("Exercise").Where(x => x.Element("ID").Value == exercise.Element("ID").Value).Elements("Set").Where(y => y.Element("Number").Value == set.Element("Number").Value).First();
                HistoryWeight = HistoryToUse.Element("Weight").Value;
                HistoryRep = HistoryToUse.Element("Reps").Value;
            }
            catch
            {
                // Do Nothing but do not error
            }
            newRoutineObject.ExerciseNumber = Int32.Parse(exercise.Element("Number").Value);
            newRoutineObject.Reps = set.Element("Reps").Value.TrimStart().TrimEnd();
            newRoutineObject.SetNumber = Int32.Parse(set.Element("Number").Value);
            newRoutineObject.Weight = set.Element("Weight").Value.TrimStart().TrimEnd();
            newRoutineObject.RestTimeAfterSet = Int32.Parse(exercise.Element("RestTimeAfterSet").Value);
            newRoutineObject.ExerciseDescription = exercise.Element("Description").Value.TrimStart().TrimEnd();

            // Create objects for the UI
            MyEntry WeightEntry = new MyEntry
            {
                Text = newRoutineObject.Weight.ToString(),
                FontFamily = App.CustomRegular,
                FontSize = 15,
                WidthRequest = 30,
                Keyboard = Keyboard.Numeric,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                MaxLength = 3,
                TextColor = Color.Gray,
                BackgroundColor = Color.Transparent
            };
            Label LastWeight = new Label
            {
                Text = HistoryWeight,
                FontFamily = App.CustomLight,
                FontSize = 15,
                WidthRequest = 30,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                //Margin = new Thickness(0, 0, 0, 12.5),
                TextColor = Color.LightGray,
                BackgroundColor = Color.Transparent
            };
            MyEntry RepEntry = new MyEntry
            {
                Text = newRoutineObject.Reps.ToString(),
                FontSize = 15,
                FontFamily = App.CustomRegular,
                WidthRequest = 30,
                Keyboard = Keyboard.Numeric,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                MaxLength = 3,
                TextColor = Color.Gray,
                BackgroundColor = Color.Transparent
            };
            Label LastRep = new Label
            {
                Text = HistoryRep,
                FontSize = 15,
                FontFamily = App.CustomLight,
                WidthRequest = 30,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                //Margin = new Thickness(0, 0, 0, 12.5),
                TextColor = Color.LightGray,
                BackgroundColor = Color.Transparent
            };
            Frame ButtonFrame = new Frame
            {
                Padding = 1.5,
                HasShadow = false,
                // This is needed to keep the circles round
                CornerRadius = 11,
                BackgroundColor = Color.Black,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };

            Button CompleteSet = new Button
            {
                CornerRadius = 10,
                WidthRequest = 20,
                HeightRequest = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Color.White
            };
            Button AddSet = new Button
            {
                Text = "Add",
                FontFamily = App.CustomBold,
                TextColor = Color.FromHex("#5bdc76"),
                BackgroundColor = Color.Transparent
            };
            Button RemoveSet = new Button
            {
                Text = "Remove",
                FontFamily = App.CustomBold,
                TextColor = Color.FromHex("#dc765b"),
                BackgroundColor = Color.Transparent
            };

            // Set the Complete set button to green if the KeepRow indicator is true.
            if (set.Element("KeepRow").Value == "1")
            {
                CompleteSet.BackgroundColor = Color.Green;
            }

            // Create UI row for entry fields
            ButtonFrame.Content = CompleteSet;

            //If it is building the page, or it is last in the superset, or it is not a superset at all
            if (Type == 1 || isLastInSuper || isSuper == false)
            {
                ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
                ExerciseContent.Children.Add(WeightEntry, 0, ExerciseContent.RowDefinitions.Count() - 1);
                ExerciseContent.Children.Add(LastWeight, 1, ExerciseContent.RowDefinitions.Count() - 1);
                ExerciseContent.Children.Add(RepEntry, 2, ExerciseContent.RowDefinitions.Count() - 1);
                ExerciseContent.Children.Add(LastRep, 3, ExerciseContent.RowDefinitions.Count() - 1);
                ExerciseContent.Children.Add(ButtonFrame, 4, ExerciseContent.RowDefinitions.Count() - 1);
            }
            else
            {
                int index = GetSupersetIndex(exercise, ExerciseContent);

                //Add the new row
                ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });

                //Move the rows that are below the decided index down one with all the child elements
                foreach (var child in ExerciseContent.Children.ToList().Where(child => Grid.GetRow(child) >= index).OrderByDescending(child => Grid.GetRow(child)))
                {
                    Grid.SetRow(child, Grid.GetRow(child) + 1);
                }

                //Place the new row at the index.
                ExerciseContent.Children.Add(WeightEntry, 0, index);
                ExerciseContent.Children.Add(LastWeight, 1, index);
                ExerciseContent.Children.Add(RepEntry, 2, index);
                ExerciseContent.Children.Add(LastRep, 3, index);
                ExerciseContent.Children.Add(ButtonFrame, 4, index);
            }

            // If this is the last set go ahead and add another row for the add remove set buttons. 
            if (newRoutineObject.SetNumber == maxSetForExercise)
            {
                if (FromSourceToPass != "Coach" || oldRoutine.Locked == false)
                {
                    //if not a superset or is the last exercise in a superset
                    if (isSuper == false || isLastInSuper)
                    {
                        ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
                        ExerciseContent.Children.Add(AddSet, 0, ExerciseContent.RowDefinitions.Count() - 1);
                        ExerciseContent.Children.Add(RemoveSet, 2, ExerciseContent.RowDefinitions.Count() - 1);
                    }
                }
            }

            // Handle complete set button
            CompleteSet.Clicked += (o, e) =>
            {
                if (CompleteSet.BackgroundColor == Color.White)
                {
                    // Flip color to green
                    CompleteSet.BackgroundColor = Color.Green;

                    // Set the keep row to true for this set
                    if (Int32.Parse(set.Element("Number").Value) == newRoutineObject.SetNumber)
                    {
                        set.Element("KeepRow").SetValue(1);
                    }

                    timerExercise = exercise.Element("Description").Value;

                    // set rest time indicator based off of the new object
                    int RestTime = Int32.Parse(exercise.Element("RestTimeAfterSet").Value);
                    // Start timer
                    timeLeft = RestTime;

                    var timespan = TimeSpan.FromSeconds(timeLeft);
                    formattedTimeLeft = timespan.ToString(@"mm\:ss");

                    startTime = RestTime;
                    TimerLabel.Text = formattedTimeLeft;
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        AddTimer.IsVisible = true;
                        MinusTimer.IsVisible = true;
                    });

                    //Create the entry
                    Microcharts.Entry AddToGraph = new Microcharts.Entry(timeLeft)
                    {
                        Color = SKColor.Parse(App.SecondaryThemeColor.ToHex().ToString())
                    };

                    try
                    {
                        timerEntries.RemoveAt(0);
                    }
                    catch
                    {
                        //Do Nothing. This means there is nothing in the list
                    }
                    timerEntries.Add(AddToGraph);

                    //Show Chart
                    TimerGrid.IsVisible = true;
                    RestClock.IsVisible = true;
                    TimerBackButton.IsVisible = true;
                    TimerGrid.TranslateTo(0, 0, 150);
                    RestClock.TranslateTo(0, 0, 150);
                    //Create the chart

                    TimerChart.Chart = new Microcharts.RadialGaugeChart
                    {
                        Entries = timerEntries,
                        MinValue = 0,
                        MaxValue = startTime,
                        StartAngle = 180
                    };

                    ClockDisplay.Text = formattedTimeLeft;
                    TimerGrid.Children.Add(ClockDisplay, 0, 0);

                    timer.Start();
                }
                else
                {
                    // Flip color to White
                    CompleteSet.BackgroundColor = Color.White;

                    // Set the keep row to false for this set
                    if (Int32.Parse(set.Element("Number").Value) == newRoutineObject.SetNumber)
                    {
                        set.Element("KeepRow").SetValue(0);
                    }

                    timerExercise = null;

                    try
                    {
                        //Remove this so the graph does not have a second donut
                        timerEntries.RemoveAt(0);
                    }
                    catch
                    {
                        // Do Nothing
                    }

                    // Stop timer
                    timer.Stop();
                    TimerLabel.Text = "";
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        TimerBackButton.IsVisible = false;
                        RestClock.TranslateTo((screenSize.Width / 2) * -1, 0, 150);
                        RestClock.IsVisible = false;
                        TimerForward.IsVisible = false;
                        //TimerBackForward.IsVisible = false;
                    });
                }

            };

            // Handle the weight entry field
            WeightEntry.Focused += (o, e) =>
            {
                WeightEntry.Text = "";
            };

            WeightEntry.Unfocused += (o, e) =>
            {
                foreach (var ObjectToUpdate in doc.Element("Routine").Elements("Exercise"))
                {
                    if (Int32.Parse(ObjectToUpdate.Element("Number").Value) == newRoutineObject.ExerciseNumber)
                    {
                        foreach (var setToUpdate in ObjectToUpdate.Elements("Set"))
                        {
                            if (Int32.Parse(setToUpdate.Element("Number").Value) == newRoutineObject.SetNumber)
                            {
                                if (Int32.Parse(setToUpdate.Element("Number").Value) == newRoutineObject.SetNumber)
                                {
                                    if (WeightEntry.Text == "")
                                    {
                                        if (setToUpdate.Element("Weight").Value == "0" || setToUpdate.Element("Weight").Value == "")
                                        {
                                            WeightEntry.Text = "0";
                                        }
                                        else
                                        {
                                            WeightEntry.Text = setToUpdate.Element("Weight").Value;
                                        }
                                    }
                                    else
                                    {
                                        setToUpdate.Element("Weight").SetValue(WeightEntry.Text);
                                    }
                                }
                            }
                        }
                    }
                }

            };

            // Handle the Rep Entry field
            RepEntry.Focused += (o, e) =>
            {
                RepEntry.Text = "";
            };

            RepEntry.Unfocused += (o, e) =>
            {
                // XML
                foreach (var ObjectToUpdate in doc.Element("Routine").Elements("Exercise"))
                {
                    if (Int32.Parse(ObjectToUpdate.Element("Number").Value) == newRoutineObject.ExerciseNumber)
                    {
                        foreach (var setToUpdate in ObjectToUpdate.Elements("Set"))
                        {
                            if (Int32.Parse(setToUpdate.Element("Number").Value) == newRoutineObject.SetNumber)
                            {
                                if (RepEntry.Text == "")
                                {
                                    if (setToUpdate.Element("Reps").Value == "0" || setToUpdate.Element("Reps").Value == "")
                                    {
                                        RepEntry.Text = "0";
                                    }
                                    else
                                    {
                                        RepEntry.Text = setToUpdate.Element("Reps").Value;
                                    }
                                }
                                else
                                {
                                    setToUpdate.Element("Reps").SetValue(RepEntry.Text);
                                }
                            }
                        }
                    }
                }
            };

            // Add set click //DO SOMETHING FOR SUPER
            AddSet.Clicked += (o, e) =>
            {
                if (isSuper)
                {
                    //Create a list of all exercises in the superset
                    IEnumerable<XElement> exerciseInSuperset = GetSupersetList(exercise);

                    //If it is a superset it needs to loop through this call for each exercise in the superset
                    foreach (var eachExcercise in exerciseInSuperset)
                    {
                        isLastInSuper = false;
                        bool isFirstInSuper = false;

                        if (eachExcercise == exerciseInSuperset.Last())
                        {
                            isLastInSuper = true;
                        }
                        if (eachExcercise == exerciseInSuperset.First())
                        {
                            isFirstInSuper = true;
                        }

                        AddSetClick(ExerciseContent, set, eachExcercise, ExerciseHeader, ExerciseFrame, isLastInSuper, isSuper, isFirstInSuper);
                    }
                }
                else
                {
                    //Not a superset so this is only called once. 
                    AddSetClick(ExerciseContent, set, exercise, ExerciseHeader, ExerciseFrame, isLastInSuper, isSuper);
                }

            };

            //DO SOMETHING FOR SUPER
            RemoveSet.Clicked += async (o, e) =>
            {
                // Stops the last set from being removed.
                if (doc.Element("Routine").Elements("Exercise").Elements("Set").Count() == 1)
                {
                    var Pop = new Pages.PopUps.InformationMessage("Warning", "This is the last set. This cannot be removed.", "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);

                    RemoveSet.IsVisible = false;
                }
                else
                {
                    // Remove the Add and Remove buttons
                    if (isSuper == true)
                    {
                        //Create a list of all exercises in the superset
                        IEnumerable<XElement> exerciseInSuperset = GetSupersetList(exercise);
                        bool lastSetFirstExercise = true;
                        int countExercisesInSuper = exerciseInSuperset.Count();

                        //If it is a superset it needs to loop through this call for each exercise in the superset
                        foreach (var eachExcercise in exerciseInSuperset)
                        {
                            isLastInSuper = false;

                            if (eachExcercise == exerciseInSuperset.Last())
                            {
                                isLastInSuper = true;
                            }
                            //Count the sets left before deleting
                            int numOfSets = Int32.Parse(
                                            eachExcercise.Elements("Set")
                                                         .Max(x => x.Element("Number").Value));
                            //Get index to remove for the UI
                            int index = GetSupersetIndex(eachExcercise, ExerciseContent);

                            index--;

                            //Remove Childrend from UI
                            foreach (var child in ExerciseContent.Children.ToList().Where(child => Grid.GetRow(child) == index))
                            {
                                ExerciseContent.Children.Remove(child);
                            }

                            //Move the child elements up the grid so we can remove the bottom rows after. 
                            foreach (var child in ExerciseContent.Children.ToList().Where(child => Grid.GetRow(child) > index))
                            {
                                Grid.SetRow(child, Grid.GetRow(child) - 1);
                            }

                            //Remove Content row 
                            ExerciseContent.RowDefinitions.RemoveAt(ExerciseContent.RowDefinitions.Count - 1);

                            // Remove from the xml
                            doc.Element("Routine")
                                .Elements("Exercise")
                                .Where(x => x.Element("ID").Value == eachExcercise.Element("ID").Value)
                                .Elements("Set")
                                .Where(z => z.Element("Number").Value == eachExcercise.Elements("Set").Max(y => y.Element("Number").Value))
                                .Remove();

                            //This is the last set in the superset
                            if (numOfSets == 1)
                            {
                                //Remove all of the children out of grid
                                foreach (var child in ExerciseContent.Children.ToList())
                                {
                                    ExerciseContent.Children.Remove(child);
                                }

                                //If this is the First time in this loop 
                                if (lastSetFirstExercise)
                                {
                                    //Loop through the amount of exercises that are in the superset
                                    for (int i = 0; i < countExercisesInSuper; i++)
                                    {
                                        //Remove that row
                                        ExerciseContent.RowDefinitions.RemoveAt(ExerciseContent.RowDefinitions.Count - 1);
                                    }

                                    //Set this so it does not make it into this if again for this super set
                                    lastSetFirstExercise = false;
                                }

                                //Remove the add and remove row
                                ExerciseContent.RowDefinitions.RemoveAt(ExerciseContent.RowDefinitions.Count - 1);
                               
                                if (isLastInSuper)
                                {
                                    //Get the index of the header in the gridStack by getting the index of the Frame which we can see at this point and subtracting one. 
                                    var indexOfHeader = gridStack.Children.IndexOf(ExerciseFrame) - 1;
                                    //Remove the frame from the gridStack all together if this is the last exercise in the superset
                                    gridStack.Children.Remove(ExerciseFrame);
                                    //Remove the header from the gridStack.
                                    gridStack.Children.RemoveAt(indexOfHeader);
                                }

                                //Remove complete excercise
                                //XML Data
                                doc.Element("Routine")
                                   .Elements("Exercise")
                                   .Where(x => x.Element("ID").Value == eachExcercise.Element("ID").Value)
                                   .Remove();                               
                            }
                        }
                    }
                    else
                    {
                        // Get index to handle the UI. This is 0 based so when handling the max exercise ID subtract 1
                        var lastListElement = exercise.Elements("Set").Max(x => (int)x.Element("Number")) - 1;


                        // 3 rows means there is only one data row left. Row 0 = headers Row 1 = data Row 2 = buttons.
                        // This if will delete the rest of the grid completely.
                        if (ExerciseContent.RowDefinitions.Count == 3)
                        {
                            foreach (var child in ExerciseContent.Children.ToList())
                            {
                                ExerciseContent.Children.Remove(child);
                            }

                            ExerciseContent.RowDefinitions.RemoveAt(ExerciseContent.RowDefinitions.Count - 1);
                            ExerciseContent.RowDefinitions.RemoveAt(ExerciseContent.RowDefinitions.Count - 1);
                            ExerciseContent.RowDefinitions.RemoveAt(ExerciseContent.RowDefinitions.Count - 1);

                            ExerciseHeader.Children.RemoveAt(1);
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

                        // This removed it from the list completely. REMOVES IT FROM THE XML                                    
                        if (lastListElement == 0)
                        {
                            doc.Element("Routine").Elements("Exercise").Where(x => x.Element("ID").Value == exercise.Element("ID").Value).Remove();
                        }
                        else
                        {
                            exercise.Elements("Set").Where(x => (int)x.Element("Number") == lastListElement + 1).Remove();
                        }

                        if (doc.Element("Routine").Elements("Exercise").Elements("Set").Count() == 1)
                        {
                            RemoveSet.IsVisible = false;
                        }
                        if (doc.Element("Routine").Elements("Exercise").Elements("Set").Count() == 2)
                        {
                            RemoveSet.IsVisible = true;
                        }

                    }
                }

            };
        }

        private void AddSetClick(Grid ExerciseContent, XElement set, XElement exercise, Grid ExerciseHeader, Frame ExerciseFrame, bool isLastInSuper, bool isSuper, bool isFirstInSuper = false)
        {
            // Create new object based off of XML data
            var RowToAdd = new RoutineList();

            var lastGridRowElement = ExerciseContent.RowDefinitions.Count - 1;
            int maxSetForExercise = exercise.Elements("Set").Max(x => (int)x.Element("Number"));
            var lastSetNode = exercise.Elements("Set").Where(x => (int)x.Element("Number") == maxSetForExercise);

            RowToAdd.ExerciseDescription = exercise.Element("Description").Value;
            RowToAdd.ExerciseNumber = Int32.Parse(exercise.Element("Number").Value);
            RowToAdd.KeepRow = false;
            RowToAdd.Reps = lastSetNode.Max(x => (string)x.Element("Reps"));
            RowToAdd.RestTimeAfterSet = Int32.Parse(exercise.Element("RestTimeAfterSet").Value);
            RowToAdd.SetNumber = maxSetForExercise + 1;
            RowToAdd.Weight = lastSetNode.Max(x => (string)x.Element("Weight"));

            // Remove the Add and Remove buttons
            if (isSuper == false || isLastInSuper == true)
            {
                foreach (var child in ExerciseContent.Children.ToList().Where(child => Grid.GetRow(child) == lastGridRowElement))
                {
                    ExerciseContent.Children.Remove(child);
                }

                foreach (var child in ExerciseContent.Children.ToList().Where(child => Grid.GetRow(child) > lastGridRowElement))
                {
                    Grid.SetRow(child, Grid.GetRow(child) - 1);
                }

                ExerciseContent.RowDefinitions.RemoveAt(lastGridRowElement);
            }

            // Create new Set
            XElement SetX = new XElement("Set",
                                                   new XElement("Number", RowToAdd.SetNumber),
                                                   new XElement("Reps", RowToAdd.Reps),
                                                   new XElement("Weight", RowToAdd.Weight),
                                                   new XElement("KeepRow", 0)
                                                  );// end Set 

            // Loop the XML to get the right Exercise and add a set there.
            foreach (var element in doc.Element("Routine").Elements("Exercise"))
            {
                if (element.Element("Description").Value == RowToAdd.ExerciseDescription)
                {
                    element.Add(SetX);
                }
            }

            // Add a row in the UI and place back the Add and remove buttons
            AddRow(SetX, exercise, ExerciseContent, ExerciseHeader, ExerciseFrame, 2, isLastInSuper, isSuper);
        }

        private async void FinishRoutine()
        {
            var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", "Are you finished with your routine", "Yes", "No");
            var answer = await QuestionPop.Show();
            if (answer) // Yes
            {
                timer.Stop();

                await Navigation.PushAsync(new RoutineSummary(oldRoutine, doc, FromSourceToPass, StartSourceToPass));
            }
        }

        private async void PressAddExercise(object sender, EventArgs e)
        {
            if (FromSourceToPass == "Coach" && oldRoutine.Locked == true)
            {
                var Pop = new Pages.PopUps.InformationMessage("Routine Locked", "Your coach has locked this routine. You cannot add exercises to it.", "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
            }
            else
            {
                //Handle popup page 
                Application.Current.ModalPopping += HandleModalPopping;
                myPage = new AddExercise.Single(App.exerciseListApp, "Add To Routine");

                await Application.Current.MainPage.Navigation.PushModalAsync(myPage);
            }
        }

        private async void OpenBarbellGuide(object sender, EventArgs e)
        {
            if (BarbellMenuSet == false)
            {
                await BarbellGrid.TranslateTo(0, 0, 250);
                BarbellMenuSet = true;
            }
            else
            {
                await BarbellGrid.TranslateTo(((screenSize.Width / 2) * -1), 0, 250);
                BarbellMenuSet = false;
            }
        }

        private void HandleModalPopping(object sender, ModalPoppingEventArgs e)
        {

            if (e.Modal == myPage)
            {
                // now we can retrieve that phone number:
                var PageExerciseObject = myPage.returnExercise;

                if (PageExerciseObject == null)
                {

                }
                else
                {
                    //Add a new exercise
                    AddNewExercise(PageExerciseObject, myPage.exercisePassedIn);
                }

                // remember to remove the event handler:
                myPage = null;
                Application.Current.ModalPopping -= HandleModalPopping;

                //Remove old filler at bottom of page
                gridStack.Children.Remove(Filler);

                //add new filler at bottom of page
                gridStack.Children.Add(Filler);

            }

        }

        //This is used to add a new Exercise to a superset
        private void AddNewExercise(ExerciseList pageExerciseObject, XElement exercisePassedIn)
        {
            //Create new Exercise Object
            RoutineList NewExerciseObject = new RoutineList();

            NewExerciseObject.ExerciseDescription = pageExerciseObject.ExerciseDescription;
            NewExerciseObject.KeepRow = false;
            NewExerciseObject.Reps = "0";
            NewExerciseObject.RestTimeAfterSet = 30;
            NewExerciseObject.SetNumber = 1;
            NewExerciseObject.Weight = "0";

            //Get variables needed to change exercise numbers
            int passedInExerciseNumber = Int32.Parse(exercisePassedIn.Element("Number").Value);
            //Increment all exercise numbers that are greater than the exercise that was passed in by 1
            foreach (var exercise in doc.Element("Routine").Elements("Exercise"))
            {
                //Get the loop exercise number
                int currentExerciseNumberInLoop = Int32.Parse(exercise.Element("Number").Value);

                if (currentExerciseNumberInLoop > passedInExerciseNumber)
                {
                    //Set the new exercise number for the looped exercise
                    exercise.Element("Number").Value = (currentExerciseNumberInLoop + 1).ToString();
                }
            }

            //Get the NewExerciseNumber by adding 1 to the exercise that was passed in.
            var NewExerciseNumber = passedInExerciseNumber++;

            NewExerciseObject.ExerciseNumber = NewExerciseNumber;

            XElement ExerciseX = new XElement("Exercise",
                                    new XAttribute("Superset", pageExerciseObject.SupersetID),
                                    new XElement("Number", NewExerciseNumber),
                                      new XElement("Description", pageExerciseObject.ExerciseDescription),
                                      new XElement("ID", pageExerciseObject.ExerciseID),
                                      new XElement("RestTimeAfterSet", 60),
                                      new XElement("Set",
                                          new XElement("Number", "1"),
                                          new XElement("Reps", "0"),
                                          new XElement("Weight", "0"),
                                          new XElement("KeepRow", 0)
                                         ) // end Set 
                                     ); // end Exercise

            doc.Element("Routine").Add(ExerciseX); // end Exercise);                    

            BuildRoutine(2);
        }

        public void timer_Tick(object sender, ElapsedEventArgs e)
        {
            // Display the new time left
            // by updating the TimerLabel label.
            timeLeft--;

            var timespan = TimeSpan.FromSeconds(timeLeft);
            formattedTimeLeft = timespan.ToString(@"mm\:ss");

            // When using Timers and UI be sure to invoke on the main thread or the UI will not update.
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                TimerLabel.Text = formattedTimeLeft;

                //Create the entry
                Microcharts.Entry AddToGraph = new Microcharts.Entry(timeLeft)
                {
                    Color = SKColor.Parse(App.SecondaryThemeColor.ToHex().ToString())
                };

                timerEntries.RemoveAt(0);
                timerEntries.Add(AddToGraph);

                TimerChart.Chart = new Microcharts.RadialGaugeChart
                {
                    Entries = timerEntries,
                    MinValue = 0,
                    MaxValue = startTime,
                    LineAreaAlpha = 50
                };

                ClockDisplay.Text = formattedTimeLeft;
            });

            //Load sounds
            var soundOne = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            soundOne.Load("Beep2.wav");

            if (timeLeft <= 15)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.IsVisible = false; });
            }

            if (timeLeft > 15)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { MinusTimer.IsVisible = true; });
            }

            if (timeLeft < 284)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.IsVisible = true; });
            }

            if (timeLeft >= 285)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { AddTimer.IsVisible = false; });
            }


            if (timeLeft == 10)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    ClockBackground.BackgroundColor = App.SecondaryThemeColor;
                });
            }
            if (timeLeft == 9)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    ClockBackground.BackgroundColor = App.PrimaryThemeColor;
                });
            }
            if (timeLeft == 8)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    ClockBackground.BackgroundColor = App.SecondaryThemeColor;
                });
            }
            if (timeLeft == 7)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    ClockBackground.BackgroundColor = App.PrimaryThemeColor;
                });
            }
            if (timeLeft == 6)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    ClockBackground.BackgroundColor = App.SecondaryThemeColor;
                });
            }

            if (timeLeft == 5)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    ClockBackground.BackgroundColor = App.PrimaryThemeColor;
                });
            }

            if (timeLeft == 4)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    ClockBackground.BackgroundColor = App.SecondaryThemeColor;
                });
            }

            if (timeLeft == 3)
            {
                soundOne.Play();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    ClockBackground.BackgroundColor = App.PrimaryThemeColor;
                });
            }

            if (timeLeft == 2)
            {
                soundOne.Play();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    ClockBackground.BackgroundColor = App.SecondaryThemeColor;
                });
            }

            if (timeLeft == 1)
            {
                soundOne.Play();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    ClockBackground.BackgroundColor = App.PrimaryThemeColor;
                });
            }

            if (timeLeft <= 0)
            {
                timer.Stop();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    ClockBackground.BackgroundColor = App.SecondaryThemeColor;
                    TimerLabel.Text = "";
                    TimerGrid.TranslateTo((screenSize.Width / 2) * -1, 0, 150);
                    TimerGrid.IsVisible = false;
                    RestClock.TranslateTo((screenSize.Width / 2) * -1, 0, 150);
                    RestClock.IsVisible = false;
                    timerEntries.RemoveAt(0);        
                    TimerForward.IsVisible = false;
                    soundOne.Load("Beep.wav"); 
                    soundOne.Play();
                });
            }
        }

        override protected bool OnBackButtonPressed()
        {

            LeavePageCheck();

            return true;
        }

        private async void LeavePageCheck()
        {
            var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", "Are you sure you want to cancel your routine?", "Yes", "No");
            var answer = await QuestionPop.Show();
            if (answer) // Yes
            {
                timer.Stop();

                if (StartSourceToPass == "Routine")
                {
                    PassID idToPass = new PassID { ID = App.userInformationApp[0].UserId.ToString() };

                    var errorPop = new Pages.PopUps.Loading("", false);
                    await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);

                    string Response = await SharedClasses.WebserviceCalls.GetUserRoutineList(idToPass);

                    await App.Current.MainPage.Navigation.PopPopupAsync();

                    if (Response == "success")
                    {
                        await Navigation.PushAsync(new Routines());
                    }
                    else
                    {
                        var Pop = new Pages.PopUps.ErrorMessage("Error", Response, "OK");
                        await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                    }
                }
                else
                {
                    await Navigation.PushAsync(new HomeScreen());
                }
            }
        }

        private async Task CalculateWeightAsync()
        {
            double chosenWeight = 45.0;
            double chosenFactor = WeightDropDown.SelectedIndex * 5.0;

            chosenWeight = chosenWeight + chosenFactor;

            // This will subtract out the weight of the bar and then divide by 2 because you show one side of the bar in the display
            double displayWeight = (chosenWeight - 45) / 2;

            int amountOf45;
            int amountOf35;
            int amountOf25;
            int amountOf10;
            int amountOf5;
            int amountOf2Point5;

            int numberOf45Plates = number45Count.SelectedIndex;
            int numberOf35Plates = number35Count.SelectedIndex;
            int numberOf25Plates = number25Count.SelectedIndex;
            int numberOf10Plates = number10Count.SelectedIndex;
            int numberOf5Plates = number5Count.SelectedIndex;
            int numberOf2Point5Plates = number2Point5Count.SelectedIndex;


            int gridColumnIndex = 1;
            int i;

            Label labelPlate = new Label
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                FontFamily = App.CustomRegular,
                Text = "Plate #",
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                Margin = new Thickness(10, 0, 0, 0)
            };
            Label labelWeightPicker = new Label
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
                FontFamily = App.CustomRegular,
                Text = "Chosen Weight: ",
                TextColor = Color.White,
                Margin = new Thickness(0, 0, 0, 17)
            };

            BarbellGrid.Children.Add(labelPlate, 0, 0);
            BarbellGrid.Children.Add(labelWeightPicker, 0, 7);
            Grid.SetColumnSpan(labelWeightPicker, 6);


            if (displayWeight >= 45)
            {
                amountOf45 = Convert.ToInt32(Math.Floor(displayWeight / 45));

                if (amountOf45 > numberOf45Plates && numberOf45Plates != 0)
                {
                    amountOf45 = numberOf45Plates;
                }

                displayWeight = displayWeight - (amountOf45 * 45);

                for (i = 0; i < amountOf45; i++)
                {
                    Frame newFrame = new Frame
                    {
                        BackgroundColor = Color.BlueViolet,
                        HasShadow = false,
                        BorderColor = Color.White
                    };
                    Label newLabel = new Label
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.End,
                        FontFamily = App.CustomRegular,
                        Text = "45",
                        TextColor = Color.White,
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        Rotation = 315,

                    };

                    BarbellGrid.Children.Add(newLabel, gridColumnIndex, 0);
                    BarbellGrid.Children.Add(newFrame, gridColumnIndex, 1);
                    Grid.SetRowSpan(newFrame, 5);

                    gridColumnIndex = gridColumnIndex + 1;
                }
            }
            if (displayWeight >= 35)
            {
                amountOf35 = Convert.ToInt32(Math.Floor(displayWeight / 35));

                if (amountOf35 > numberOf35Plates && numberOf35Plates != 0)
                {
                    amountOf35 = numberOf35Plates;
                }

                displayWeight = displayWeight - (amountOf35 * 35);

                for (i = 0; i < amountOf35; i++)
                {
                    if (gridColumnIndex > 13)
                    {
                        var Pop = new Pages.PopUps.ErrorMessage("NOT ENOUGH PLATES", "There are not enough 45 plates to calculate this amount. Please allow more 45 pound plates.", "OK");
                        await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                        break;
                    }

                    Frame newFrame = new Frame
                    {
                        BackgroundColor = Color.Coral,
                        BorderColor = Color.White,
                        HasShadow = false,
                        Margin = new Thickness(1, 10, 1, 10)
                    };
                    Label newLabel = new Label
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.End,
                        FontFamily = App.CustomRegular,
                        Text = "35",
                        TextColor = Color.White,
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        Rotation = 315,
                    };

                    BarbellGrid.Children.Add(newLabel, gridColumnIndex, 0);
                    BarbellGrid.Children.Add(newFrame, gridColumnIndex, 1);
                    Grid.SetRowSpan(newFrame, 5);

                    gridColumnIndex = gridColumnIndex + 1;
                }
            }
            if (displayWeight >= 25 && gridColumnIndex <= 13)
            {
                amountOf25 = Convert.ToInt32(Math.Floor(displayWeight / 25));

                if (amountOf25 > numberOf25Plates && numberOf25Plates != 0)
                {
                    amountOf25 = numberOf25Plates;
                }

                displayWeight = displayWeight - (amountOf25 * 25);

                for (i = 0; i < amountOf25; i++)
                {
                    if (gridColumnIndex > 13)
                    {
                        var Pop = new Pages.PopUps.ErrorMessage("NOT ENOUGH PLATES", "There are not enough 45 plates or 35 plates to calculate this amount. Please allow more plates.", "OK");
                        await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);

                        break;
                    }

                    Frame newFrame = new Frame
                    {
                        BackgroundColor = Color.OrangeRed,
                        BorderColor = Color.White,
                        HasShadow = false,
                        Margin = new Thickness(2, 30, 2, 30)
                    };
                    Label newLabel = new Label
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.End,
                        FontFamily = App.CustomRegular,
                        Text = "25",
                        TextColor = Color.White,
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        Rotation = 315,
                    };

                    BarbellGrid.Children.Add(newLabel, gridColumnIndex, 0);
                    BarbellGrid.Children.Add(newFrame, gridColumnIndex, 1);
                    Grid.SetRowSpan(newFrame, 5);

                    gridColumnIndex = gridColumnIndex + 1;
                }
            }
            if (displayWeight >= 10 && gridColumnIndex <= 13)
            {
                amountOf10 = Convert.ToInt32(Math.Floor(displayWeight / 10));

                if (amountOf10 > numberOf10Plates && numberOf10Plates != 0)
                {
                    amountOf10 = numberOf10Plates;
                }

                displayWeight = displayWeight - (amountOf10 * 10);

                for (i = 0; i < amountOf10; i++)
                {

                    if (gridColumnIndex > 13)
                    {
                        var Pop = new Pages.PopUps.ErrorMessage("NOT ENOUGH PLATES", "There are not enough 45, 35, or 25 pound plates to calculate this amount. Please allow more plates.", "OK");
                        await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);

                        break;
                    }

                    Frame newFrame = new Frame
                    {
                        BackgroundColor = Color.Firebrick,
                        BorderColor = Color.White,
                        HasShadow = false,
                        Margin = new Thickness(4, 50, 4, 50)
                    };
                    Label newLabel = new Label
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.End,
                        FontFamily = App.CustomRegular,
                        Text = "10",
                        TextColor = Color.White,
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        Rotation = 315,
                    };

                    BarbellGrid.Children.Add(newLabel, gridColumnIndex, 0);
                    BarbellGrid.Children.Add(newFrame, gridColumnIndex, 1);
                    Grid.SetRowSpan(newFrame, 5);

                    gridColumnIndex = gridColumnIndex + 1;
                }
            }
            if (displayWeight >= 5 && gridColumnIndex <= 13)
            {
                amountOf5 = Convert.ToInt32(Math.Floor(displayWeight / 5));

                if (amountOf5 > numberOf5Plates && numberOf5Plates != 0)
                {
                    amountOf5 = numberOf5Plates;
                }

                displayWeight = displayWeight - (amountOf5 * 5);

                for (i = 0; i < amountOf5; i++)
                {
                    if (gridColumnIndex > 13)
                    {
                        var Pop = new Pages.PopUps.ErrorMessage("NOT ENOUGH PLATES", "There are not enough 45, 35, 25, or 10 pound plates to calculate this amount. Please allow more plates.", "OK");
                        await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                        break;
                    }

                    Frame newFrame = new Frame
                    {
                        BackgroundColor = Color.LightGreen,
                        BorderColor = Color.White,
                        HasShadow = false,
                        Margin = new Thickness(5, 60, 5, 60)
                    };
                    Label newLabel = new Label
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.End,
                        FontFamily = App.CustomRegular,
                        Text = "5",
                        TextColor = Color.White,
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        Rotation = 315,
                    };

                    BarbellGrid.Children.Add(newLabel, gridColumnIndex, 0);
                    BarbellGrid.Children.Add(newFrame, gridColumnIndex, 1);
                    Grid.SetRowSpan(newFrame, 5);

                    gridColumnIndex = gridColumnIndex + 1;
                }
            }
            if (displayWeight >= 2.5 && gridColumnIndex <= 13)
            {
                amountOf2Point5 = Convert.ToInt32(Math.Floor(displayWeight / 2.5));

                if (amountOf2Point5 > numberOf2Point5Plates && numberOf2Point5Plates != 0)
                {
                    amountOf2Point5 = numberOf2Point5Plates;
                }

                displayWeight = displayWeight - (amountOf2Point5 * 2.5);

                for (i = 0; i < amountOf2Point5; i++)
                {
                    if (gridColumnIndex > 13)
                    {
                        var Pop = new Pages.PopUps.ErrorMessage("NOT ENOUGH PLATES", "There are not enough 45, 35, 25, 10, or 5 pound plates to calculate this amount. Please allow more plates.", "OK");
                        await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                        
                        break;
                    }

                    Frame newFrame = new Frame
                    {
                        BackgroundColor = Color.LightBlue,
                        BorderColor = Color.White,
                        HasShadow = false,
                        Margin = new Thickness(6, 70, 6, 70)
                    };
                    Label newLabel = new Label
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.End,
                        FontFamily = App.CustomRegular,
                        Text = "2.5",
                        TextColor = Color.White,
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        Rotation = 315,
                    };

                    BarbellGrid.Children.Add(newLabel, gridColumnIndex, 0);
                    BarbellGrid.Children.Add(newFrame, gridColumnIndex, 1);
                    Grid.SetRowSpan(newFrame, 5);

                    gridColumnIndex = gridColumnIndex + 1;
                }
            }

            if (displayWeight > 0)
            {
                var Pop = new Pages.PopUps.ErrorMessage("NOT ENOUGH PLATES", "You have run out of plates. Add more to recalculate this weight.", "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
            }



        }

        private int GetSupersetIndex(XElement exercise, Grid ExerciseContent)
        {
            //Create a list of all exercises in the superset
            IEnumerable<XElement> exerciseInSuperset = GetSupersetList(exercise);

            //Declare variables to use for Grid placement
            int totalExerciseCount = 0;
            int totalSetCount = 0;
            int totalrows = 0;
            int exerciseContentRowCount = ExerciseContent.RowDefinitions.Count;

            //Count the rows from the bottom to get to an index that is needed for the insert
            foreach (var ex in exerciseInSuperset)
            {
                //If the exercise number is greater than the passed in then count it toward the index
                if (Int32.Parse(ex.Element("Number").Value) > Int32.Parse(exercise.Element("Number").Value))
                {
                    //Count the sets in this exercise
                    int numOfSets = ex.Elements("Set").Count();

                    //increment the totals.
                    //increment this twice to accoung for the weight and rep label row and the title row for the exercise. 
                    totalExerciseCount = totalExerciseCount + 2;
                    totalSetCount = totalSetCount + numOfSets;
                }
            }

            //The additional 1 is to account for the add remove row
            totalrows = 1 + totalSetCount + totalExerciseCount;

            return exerciseContentRowCount - totalrows;
        }

        private IEnumerable<XElement> GetSupersetList(XElement exercise)
        {
            return doc.Element("Routine")
                      .Elements("Exercise")
                      .Where(x => x.Attribute("SupersetID").Value == exercise.Attribute("SupersetID").Value)
                      .OrderBy(x => x.Element("Number").Value);
        }
    }
}