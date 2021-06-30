using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackWorkout.Entitys;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Xml.Linq;
using Rg.Plugins.Popup.Extensions;

namespace TrackWorkout.Pages.Routine
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutineSummary : ContentPage
    {
        XDocument compareOldRoutine;
        XDocument compareNewRoutine;
        List<string> PRid = new List<string>();
        XDocument PRInfo = new XDocument (new XElement("PRInfo"));
        XDocument OldPRInfo = XDocument.Parse(App.userInformationApp[0].PRData);
        RoutineList OldOnBack = new RoutineList();
        String FromSourceToPass;
        String StartSourceToPass;
        bool Locked;
        bool PlaySound = false;

        public RoutineSummary(RoutineList OldRoutine, XDocument NewRoutine, string FromSource, string StartSource)
        {
            InitializeComponent();

            FinishRoutineFrame.BackgroundColor = App.PrimaryThemeColor;
            FinishButtonNew.BackgroundColor = App.SecondaryThemeColor;
            ButtonGridBottom.BackgroundColor = App.PrimaryThemeColor;
            Notes.FontFamily = App.CustomRegular;

            var soundOne = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            soundOne.Load("PR.mp3");

            // Grab both the old and new routine to compare later. 
            compareOldRoutine = XDocument.Parse(OldRoutine.RoutineDetail);
            compareNewRoutine = NewRoutine;
            OldOnBack = OldRoutine;

            Locked = OldRoutine.Locked;

            // store source to pass
            FromSourceToPass = FromSource;
            StartSourceToPass = StartSource;

            // Hide default nav
            NavigationPage.SetHasNavigationBar(this, false);

            //Create UI elements
            HeaderLabel.Text = NewRoutine.Element("Routine").Element("Name").Value;
            HeaderLabel.FontFamily = App.CustomBold;

            // Create Frame
            Frame HeaderFrame = new Frame
            {
                HasShadow = false,
                Padding = 0,
            };

            // Create grid on this loop
            Grid HeaderContent = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                BackgroundColor = App.PrimaryThemeColor,
                ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Auto }
                        }
            };           

            ShowRoutineSummary(NewRoutine);

            Notes.Unfocused += (o, e) =>
            {
                OldRoutine.Notes = Notes.Text;
            };

            if (PlaySound == true)
            {
                soundOne.Play();
            }
        }
        private void FinishClick(object sender, EventArgs e) // async?
        {
            FinishRoutine(OldOnBack);
        }

        private void ShowRoutineSummary(XDocument NewRoutine)
        {
            foreach (var exercise in NewRoutine.Element("Routine").Elements("Exercise"))
            {
                bool AddExercise = false;

                // Loop through set to make sure at least one has KeepRow on 
                foreach (var set in exercise.Elements("Set"))
                {
                    if (set.Element("KeepRow").Value == "1")
                    {
                        AddExercise = true;
                    }
                }

                // If one set does have at least one add the exercise.
                if (AddExercise == true)
                {
                    // Create Name Label
                    Label ExerciseLabel = new Label
                    {
                        Text = exercise.Element("Description").Value,
                        FontFamily = App.CustomRegular,
                        FontSize = 15,
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
        }

        // Use to fill the exercise info
        private void FillGrid(XElement exercise, Grid ExerciseContent)
        {
            foreach (var set in exercise.Elements("Set"))
            {
                // If Keep row is true add it
                if (set.Element("KeepRow").Value == "1")
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
                    double currentPR;
                    string weightType;
                    try
                    {
                        var PRObject = OldPRInfo.Element("PRInfo").Elements("PR").Where(x => x.Element("Exercise").Value == exercise.Element("Description").Value).Max();
                        currentPR = Double.Parse(PRObject.Element(PRObject.Element("WeightType").Value).Value);
                        weightType = PRObject.Element("WeightType").Value;

                        if (weightType == "Lbs" && App.userInformationApp[0].WeightType == "Kgs")
                        {
                            // convert to Kgs (What the user is now using)
                            currentPR = currentPR * 0.4536;
                        }
                        else if (weightType == "Kgs" && App.userInformationApp[0].WeightType == "Lbs")
                        {
                            // convert to Lbs (What the user is now using)
                            currentPR = currentPR * 2.2045855;
                        }
                    }
                    catch
                    {
                        currentPR = 0;
                    }
                    double thisWorkoutLbs;
                    double thisWorkoutKgs;
                    XElement addPR;
                    bool alreadyAdded = false;
                    
                    

                    // Check if the CurrentPR stored in the old XML is less than the set you are in.
                    if (Double.Parse(set.Element("Weight").Value) > currentPR)
                    {
                        // loop through the current PR XML created on this page to make sure a PR was not already set
                        // if it was already set then remove that value and add the new one. (the old PR was broken twice in
                        // this scenario)
                        foreach (var checkSetXML in PRInfo.Element("PRInfo").Elements("PR"))
                        {
                            // match on Exercise from this set and the PR XML created on this page
                            if (checkSetXML.Element("Exercise").Value == exercise.Element("Description").Value)
                            {
                                // grab the PR from the PR XML
                                currentPR = Double.Parse(checkSetXML.Element(checkSetXML.Element("WeightType").Value).Value);

                                // Convert it if neccessary
                                if (checkSetXML.Element("WeightType").Value == "Lbs" && App.userInformationApp[0].WeightType == "Kgs")
                                {
                                    // convert to Kgs (What the user is now using)
                                    currentPR = currentPR * 0.45359237;
                                }
                                else if (checkSetXML.Element("WeightType").Value == "Kgs" && App.userInformationApp[0].WeightType == "Lbs")
                                {
                                    // convert to Lbs (What the user is now using)
                                    currentPR = currentPR * 2.20462262;
                                }

                                // if this set is greater than the PR XML then do this
                                if (Double.Parse(set.Element("Weight").Value) > currentPR)
                                {
                                    // convert values to store in the new XML both ways
                                    if (App.userInformationApp[0].WeightType == "Lbs")
                                    {
                                        thisWorkoutLbs = Double.Parse(set.Element("Weight").Value);
                                        thisWorkoutKgs = Double.Parse(set.Element("Weight").Value) * 0.45359237;
                                    }
                                    else
                                    {
                                        thisWorkoutLbs = Double.Parse(set.Element("Weight").Value) * 2.20462262;
                                        thisWorkoutKgs = Double.Parse(set.Element("Weight").Value);
                                    }

                                    // change the color for display purposes
                                    textColor = Color.White;
                                    backgroundColor = App.ThemeColor3;
                                    // create new node to add
                                    addPR = new XElement("PR",
                                                            new XElement("ExerciseID", exercise.Element("ID").Value),
                                                            new XElement("Exercise", exercise.Element("Description").Value),
                                                            new XElement("Lbs", Math.Round(thisWorkoutLbs, 2)),
                                                            new XElement("Kgs", Math.Round(thisWorkoutKgs, 2)),
                                                            new XElement("WeightType", App.userInformationApp[0].WeightType),
                                                            new XElement("RecordDate", DateTime.Today));
                                    // add to list to check for unboken PR's later
                                    PRid.Add(exercise.Element("ID").Value);

                                    // Remove the old value
                                    checkSetXML.Remove();
                                    // Add the new value
                                    PRInfo.Element("PRInfo").Add(addPR);

                                }
                                // set this so the program knows that this PR was found in the PR XML and already evaluated.
                                // If it wasn't then it needs to be added below.
                                alreadyAdded = true;
                            }
                        }

                        // If the PR was not already evaluated above then go ahead and add it.
                        if (alreadyAdded == false)
                        {
                            // convert values to store in the new XML both ways
                            if (App.userInformationApp[0].WeightType == "Lbs")
                            {
                                thisWorkoutLbs = Double.Parse(set.Element("Weight").Value);
                                thisWorkoutKgs = Double.Parse(set.Element("Weight").Value) * 0.45359237;
                            }
                            else
                            {
                                thisWorkoutLbs = Double.Parse(set.Element("Weight").Value) * 2.20462262;
                                thisWorkoutKgs = Double.Parse(set.Element("Weight").Value);
                            }

                            // change the color for display purposes
                            textColor = Color.White;
                            backgroundColor = App.ThemeColor3;
                            // create new node to add
                            addPR = new XElement("PR",
                                                    new XElement("ExerciseID", exercise.Element("ID").Value),
                                                    new XElement("Exercise", exercise.Element("Description").Value),
                                                    new XElement("Lbs", Math.Round(thisWorkoutLbs, 2)),
                                                    new XElement("Kgs", Math.Round(thisWorkoutKgs, 2)),
                                                    new XElement("WeightType", App.userInformationApp[0].WeightType),
                                                    new XElement("RecordDate", DateTime.Today));
                            // add to list to check for unboken PR's later
                            PRid.Add(exercise.Element("ID").Value);

                            // Add the new value
                            PRInfo.Element("PRInfo").Add(addPR);
                        }
                    }

                    // Create objects for the UI
                    Label WeightEntry = new Label
                    {
                        Text = newRoutineObject.Weight.ToString(),
                        FontSize = 15,
                        FontFamily = App.CustomRegular,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        TextColor = textColor,
                        BackgroundColor = backgroundColor
                    };
                    Label RepEntry = new Label
                    {
                        Text = newRoutineObject.Reps.ToString(),
                        FontSize = 15,
                        FontFamily = App.CustomRegular,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        TextColor = textColor,
                        BackgroundColor = backgroundColor
                    };
                    Label PR = new Label
                    {
                        Text = "NEW PR!",
                        FontSize = 15,
                        FontFamily = App.CustomBold,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
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

                    ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute)});
                    // This allows the column to change color using a span on a box view. This must be inserted before the Label.
                    ExerciseContent.Children.Add(box1, 0, 3, newRoutineObject.SetNumber, newRoutineObject.SetNumber + 1);

                    // Then the labels are placed over the box view.
                    ExerciseContent.Children.Add(WeightEntry, 0, newRoutineObject.SetNumber);
                    ExerciseContent.Children.Add(RepEntry, 1, newRoutineObject.SetNumber);
                    if (backgroundColor == App.ThemeColor3)
                    {
                        ExerciseContent.Children.Add(PR, 2, newRoutineObject.SetNumber);
                        if (PlaySound != true)
                        {                            
                            PlaySound = true;
                        }
                    }                                       
                }

            }

        }

        // When hitting the start routine button a pop up appears asking for confirmation to start. Once confirmed the routine will begin
        private async void FinishRoutine(RoutineList OldRoutine)
        {
            ModifyXML();

            bool Match = true;
            var soundOne = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            soundOne.Load("chime1.wav");
            

            if (Locked == false)
            {
                if (compareNewRoutine != compareOldRoutine)
                {
                    Match = false;
                }
            }

            if (Match == false && OldRoutine.Locked != true)
            {
                var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", "Would you like to overlay your old routine with this updated one?", "Yes", "No");
                var answer = await QuestionPop.Show();
                if (answer) // Yes
                {                    
                    OldRoutine.ProcedureType = 2; //Replaces routine and logs history  
                }
                else
                {
                    OldRoutine.ProcedureType = 1; //Keeps old routine and logs history  
                }
            }
            else
            {
                OldRoutine.ProcedureType = 1; //Keeps old routine and logs history
                OldRoutine.UserID = App.userInformationApp[0].UserId.ToString(); //Keeps old routine and logs history
            }
            foreach (var PR in OldPRInfo.Element("PRInfo").Elements("PR"))
            {
                if (PRid.Contains(PR.Element("ExerciseID").Value))
                {
                    continue;
                }
                else
                {
                    PRInfo.Element("PRInfo").Add(PR);
                }
            }

            OldRoutine.RoutineDetail = compareNewRoutine.ToString();
            OldRoutine.PRData = PRInfo.ToString();

            try
            {
                var errorPop = new Pages.PopUps.Loading("Saving Workout");
                await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);

                string Response = await SharedClasses.WebserviceCalls.CompleteWorkout(OldRoutine);                

                if (Response == "success")
                {                   
                    PassID idToPass = new PassID { ID = App.userInformationApp[0].UserId.ToString() };

                    Response = await SharedClasses.WebserviceCalls.GetUserRoutineList(idToPass);

                    if (Response == "success")
                    {                       
                        Entitys.UserEmail passEmail = new Entitys.UserEmail { Email = App.userInformationApp[0].Email };

                        Response = await SharedClasses.WebserviceCalls.GetUserHistoryData(passEmail);

                        await App.Current.MainPage.Navigation.PopPopupAsync();

                        if (Response == "success")
                        {
                            Entitys.PassID passID = new Entitys.PassID { ID = App.userInformationApp[0].UserId.ToString() };                            
                            DoneAnimation.IsVisible = true;
                            DoneAnimation.Play();
                            soundOne.Play();
                        }
                        else
                        {
                            var Pop = new Pages.PopUps.ErrorMessage("Error", Response, "OK");
                            await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                        }                                                
                    }
                    else
                    {
                        await App.Current.MainPage.Navigation.PopPopupAsync();
                        var Pop = new Pages.PopUps.ErrorMessage("Error", Response, "OK");
                        await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                    }
                }
                else
                {
                    await App.Current.MainPage.Navigation.PopPopupAsync();
                    var Pop = new Pages.PopUps.ErrorMessage("Error", Response, "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                }
            }
            catch
            {
                var Pop = new Pages.PopUps.ErrorMessage("Connection Error", "Connection was lost. Please try again.", "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);

                await Navigation.PushAsync(new RoutineSummary(OldOnBack, compareNewRoutine, FromSourceToPass, StartSourceToPass));
            }

        }

        private async void AnimationComplete(object sender, EventArgs e)
        {
            await EndRoutineAsync();
        } 

        private async Task EndRoutineAsync()
        {           
            await Navigation.PushAsync(new HomeScreen());
        }

        override protected bool OnBackButtonPressed()
        {
            OldOnBack.RoutineDetail = compareNewRoutine.ToString();
            Navigation.PushAsync(new RoutineInProgress(OldOnBack, FromSourceToPass, StartSourceToPass));
            return true;
        }

        private void ModifyXML()
        {
            // Used to handle the renumbering of sets
            int removedFromSet = 0;

            // remove sets that were not completed and renumber the rest
            foreach (var exercise in compareNewRoutine.Element("Routine").Elements("Exercise"))
            {
                removedFromSet = 0;
                int numberOfSets = exercise.Elements("Set").Count();

                for (int x = 1; x <= numberOfSets; x++)
                {
                    var nodeToEvaluate = exercise.Elements("Set").Where(y => Int32.Parse(y.Element("Number").Value) == x).Max();
                    if (nodeToEvaluate.Element("KeepRow").Value == "0")
                    {
                        removedFromSet = removedFromSet + 1;
                        nodeToEvaluate.Remove();
                        continue;
                    }
                    if (removedFromSet != 0)
                    {
                        int newNumberValue = Int32.Parse(nodeToEvaluate.Element("Number").Value) - removedFromSet;
                        exercise.Elements("Set").Where(y => Int32.Parse(y.Element("Number").Value) == x).Max().Element("Number").SetValue(newNumberValue);
                    }
                }
            }
            
            //Remove KeepRow from the xml. 
            foreach (var exercise in compareNewRoutine.Element("Routine").Elements("Exercise"))
            {
                foreach (var set in exercise.Elements("Set"))
                {
                    foreach (var keepRow in set.Elements("KeepRow"))
                    {
                        keepRow.Remove();
                    }
                }
            }
        }

    }
}