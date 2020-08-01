using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackWorkout.Entitys;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Data;
using System.Xml.Linq;
using TrackWorkout.CustomRenderer;
using Rg.Plugins.Popup.Extensions;

namespace TrackWorkout.Pages.Routine
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoutineInformationEdit : ContentPage
	{
        Entitys.ClientInformation clientToPass;
        Color colorToUse = new Color();
        bool isFromClient = false;
        AddExercise.Single myPage;
        string editOrCreate;
        RoutineList BuildingRoutine;
        RoutineList AddedDuringRoutine;

        Frame Filler = new Frame
        {
            BackgroundColor = Color.Transparent,
            HeightRequest = 70
        };

        XDocument doc;

        public RoutineInformationEdit (RoutineList exercises, Entitys.ClientInformation Client)
		{
			InitializeComponent ();

            ButtonGridBottom.BackgroundColor = App.PrimaryThemeColor;
            AddExerciseButtonButtonAction.BackgroundColor = App.SecondaryThemeColor;
            addButtonFrame2.BackgroundColor = App.SecondaryThemeColor;
            addButtonFrame.BackgroundColor = App.PrimaryThemeColor;
            RoutineName.FontFamily = App.CustomRegular;

            clientToPass = Client;


            if (clientToPass == null)
            {
                colorToUse = App.PrimaryThemeColor;
                LockButton.IsVisible = false;
            }
            else
            {
                colorToUse = App.PrimaryThemeColor;
                isFromClient = true;
            }
            FinishButton.BackgroundColor = colorToUse;

            if (exercises.RoutineName != null)
            {
                RoutineName.Text = exercises.RoutineName;
                editOrCreate = "edit";
                HeaderLabel.Text = exercises.RoutineName;
            }
            else {
                editOrCreate = "create";
            }

            BuildingRoutine = exercises;

            doc = XDocument.Parse(BuildingRoutine.RoutineDetail);

            AddedDuringRoutine = new RoutineList();

            NavigationPage.SetHasNavigationBar(this, false);
            
            // Create Frame
            Frame HeaderFrame = new Frame
            {
                HasShadow = false,
                Padding = 0,
            };

            if (isFromClient == true)
            {
                if (BuildingRoutine.Locked == true)
                {
                    LockButton.Source = "Lock.png";
                    LockButton.Opacity = 1;
                    BuildingRoutine.Locked = true;
                }
            }

            FinishButton.Clicked += (o, e) =>
            {
                FinishRoutine();
            };

            LockButton.Clicked += (o, e) =>
            {
                LockRoutine();
            };

            BuildRoutine(1);

            gridStack.Children.Add(Filler);

            RoutineName.Unfocused += (o, e) =>
            {
                HeaderLabel.Text = RoutineName.Text;
                BuildingRoutine.RoutineName = RoutineName.Text;
            };
        }

        private void BuildRoutine(int Type)
        {
            // Create a node list to loop through to create the page
            IEnumerable<XElement> loopThrough;

            // 1 means it is on load of the page while a 2 indicates it was an added exercise
            if (Type == 1)
            {
                // Loop all exercises
                loopThrough = doc.Element("Routine").Elements("Exercise");
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

                int RestTimeValue = int.Parse(exercise.Element("RestTimeAfterSet").Value);

                // Create Name Label
                Label ExerciseLabel = new Label
                {
                    Text = exercise.Element("Description").Value,
                    FontSize = 15,
                    FontFamily = App.CustomRegular,
                    HeightRequest = 20,
                    Margin = new Thickness(15, 0, 0, 5),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = colorToUse
                };

                Button RemoveRest = new Button
                {
                    Text = "-",
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.FromHex("#dc765b"),
                    BackgroundColor = Color.Transparent
                };
                
                Label RestLabel = new Label
                {
                    Text = "Rest Time: " + RestTimeValue.ToString(),
                    FontSize = 15,
                    FontFamily = App.CustomLight,
                    HeightRequest = 20,
                    Margin = new Thickness(0, 0, 0, 5),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = colorToUse
                };

                Button AddRest = new Button
                {
                    Text = "+",
                    FontSize = 15,         
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
                    FontFamily = App.CustomBold,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End,
                    TextColor = Color.Gray
                };

                // Create Reps Label
                Label RepsLabel = new Label
                {
                    Text = "Reps",
                    FontAttributes = FontAttributes.Bold,
                    FontFamily = App.CustomBold,
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
                    Margin = new Thickness(0, 0, 0, 10),
                };


                Grid ExerciseHeader = new Grid
                {
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    BackgroundColor = Color.Transparent,
                    ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = 40 },
                            new ColumnDefinition { Width = GridLength.Auto },
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
                    
                    exercise.Element("RestTimeAfterSet").SetValue(RestTimeValue);                                            
                };

                AddRest.Clicked += (o, e) =>
                {
                    RestTimeValue = RestTimeValue + 15;
                    RestLabel.Text = "Rest Time: " + RestTimeValue.ToString();

                    if (RestTimeValue == 30)
                    {
                        RemoveRest.IsVisible = true;
                    }

                    exercise.Element("RestTimeAfterSet").SetValue(RestTimeValue);                    
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
                FillGrid(exercise, ExerciseContent, ExerciseHeader, ExerciseFrame, Type);

                // Add Grid to Frame
                ExerciseFrame.Content = ExerciseContent;

                // Add Frame to page
                gridStack.Children.Add(ExerciseFrame);
            }
        }

        private void FillGrid(XElement exercise, Grid ExerciseContent, Grid ExerciseHeader, Frame ExerciseFrame, int Type)
        {
            foreach(var set in exercise.Elements("Set"))
            { 
                AddRow(set, exercise, ExerciseContent, ExerciseHeader, ExerciseFrame, Type);            
            }
        }

        private void AddRow(XElement set, XElement exercise, Grid ExerciseContent, Grid ExerciseHeader, Frame ExerciseFrame, int Type)
        {

            RoutineList newRoutineObject = new RoutineList();
            int maxSetForExercise = exercise.Elements("Set").Max(x => (int)x.Element("Number"));

            newRoutineObject.ExerciseNumber = Int32.Parse(exercise.Element("Number").Value);
            newRoutineObject.Reps = set.Element("Reps").Value;
            newRoutineObject.SetNumber = Int32.Parse(set.Element("Number").Value);
            newRoutineObject.Weight = set.Element("Weight").Value;
            newRoutineObject.RestTimeAfterSet = Int32.Parse(exercise.Element("RestTimeAfterSet").Value);
            newRoutineObject.ExerciseDescription = exercise.Element("Description").Value;

            MyEntry WeightEntry = new MyEntry
            {
                Text = newRoutineObject.Weight.ToString(),
                FontSize = 15,
                FontFamily = App.CustomLight,
                Keyboard = Keyboard.Numeric,
                WidthRequest = 30,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                MaxLength = 3,
                TextColor = Color.Gray,
                BackgroundColor = Color.White
            };
            MyEntry RepEntry = new MyEntry
            {
                Text = newRoutineObject.Reps.ToString(),
                FontSize = 15,
                FontFamily = App.CustomLight,
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
                FontFamily = App.CustomRegular,
                BackgroundColor = Color.Transparent
            };

            Button RemoveSet = new Button
            {
                Text = "Remove",
                TextColor = Color.FromHex("#dc765b"),
                FontFamily = App.CustomRegular,
                BackgroundColor = Color.Transparent
            };
            
            int RestTime = newRoutineObject.RestTimeAfterSet;            

            ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });

            ExerciseContent.Children.Add(WeightEntry, 0, newRoutineObject.SetNumber);
            ExerciseContent.Children.Add(RepEntry, 1, newRoutineObject.SetNumber);

            // If this is the last set go ahead and add another row for the add remove set buttons. 
            if (newRoutineObject.SetNumber == maxSetForExercise)
            {
                ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
                ExerciseContent.Children.Add(AddSet, 0, newRoutineObject.SetNumber + 1);
                ExerciseContent.Children.Add(RemoveSet, 1, newRoutineObject.SetNumber + 1);
            }

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

            // Add set click
            AddSet.Clicked += (o, e) =>
            {
                AddSetClick(ExerciseContent, set, exercise, ExerciseHeader, ExerciseFrame);
            };

            RemoveSet.Clicked += async (o, e) =>
            {
                // Get index to handle the UI. This is 0 based so when handling the max exercise ID subtract 1
                var lastListElement = exercise.Elements("Set").Max(x => (int)x.Element("Number")) - 1;

                // Stops the last set from being removed.
                if (doc.Element("Routine").Elements("Exercise").Elements("Set").Count() == 1)
                {
                    var Pop = new Pages.PopUps.InformationMessage("Warning", "This is the last set. This cannot be removed.", "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);

                    RemoveSet.IsVisible = false;
                }
                else
                {

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

            };
        }

        private void AddSetClick(Grid ExerciseContent, XElement set, XElement exercise, Grid ExerciseHeader, Frame ExerciseFrame)
        {
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
            foreach (var child in ExerciseContent.Children.ToList().Where(child => Grid.GetRow(child) == lastGridRowElement))
            {
                ExerciseContent.Children.Remove(child);
            }

            foreach (var child in ExerciseContent.Children.ToList().Where(child => Grid.GetRow(child) > lastGridRowElement))
            {
                Grid.SetRow(child, Grid.GetRow(child) - 1);
            }

            ExerciseContent.RowDefinitions.RemoveAt(lastGridRowElement);

            // Create new Set
            XElement SetX = new XElement("Set",
                                                   new XElement("Number", RowToAdd.SetNumber),
                                                   new XElement("Reps", RowToAdd.Reps),
                                                   new XElement("Weight", RowToAdd.Weight)
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
            AddRow(SetX, exercise, ExerciseContent, ExerciseHeader, ExerciseFrame, 1);            
        }

        private async void FinishRoutine()
        {
            var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", "Are you finished with your ROUTINE?", "Yes", "No");
            var answer = await QuestionPop.Show();
            if (answer) // Yes
            {
                bool weightValid = true;
                bool repValid = true;

                foreach (var exercise in doc.Element("Routine").Elements("Exercise"))
                {
                    foreach (var set in exercise.Elements("Set"))
                    {
                        if (set.Element("Reps").Value == "0" || set.Element("Reps").Value == null)
                        {
                            repValid = false;
                        }

                        if (set.Element("Weight").Value == "0" || set.Element("Weight").Value == null)
                        {
                            weightValid = false;
                        }
                    }
                }                

                if (RoutineName.Text == null)
                {
                    var Pop = new Pages.PopUps.ErrorMessage("Invalid ROUTINE Name", $"Please add a ROUTINE name.", "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                }

                else if (weightValid == false)
                {
                    var Pop = new Pages.PopUps.ErrorMessage("Fill in all weight fields", $"There are weight fields that are not filled.", "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                }
                else if (repValid == false)
                {
                    var Pop = new Pages.PopUps.ErrorMessage("Fill in all rep fields", $"There are rep fields that are not filled.", "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                }
                else
                {                    
                    InsertRoutine();
                }
            }
        }                      
       
        private async void InsertRoutine()
        {
            var soundOne = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            soundOne.Load("chime1.wav");

            doc.Element("Routine").Element("Name").SetValue(RoutineName.Text);
            
            BuildingRoutine.RoutineName = RoutineName.Text;
            if (editOrCreate == "edit")
            {
                BuildingRoutine.ProcedureType = 3;
            }
            else
            {
                BuildingRoutine.ProcedureType = 1;
            }
            BuildingRoutine.UserID = App.userInformationApp[0].UserId.ToString();
            
            BuildingRoutine.RoutineDetail = doc.ToString();
            
            if (isFromClient == true)
            {
                BuildingRoutine.CoachID = Int32.Parse(App.UserRoutineListApp[0].UserID);
                BuildingRoutine.UserID = clientToPass.ClientID;
            }

            List<RoutineList> listToPass = new List<RoutineList>();

            listToPass.Add(BuildingRoutine);

            var errorPop = new Pages.PopUps.Loading("Adding routine");
            await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);

            string Response = await SharedClasses.WebserviceCalls.AddRoutine(listToPass);

            await App.Current.MainPage.Navigation.PopPopupAsync();

            if (Response == "success")
            {
                DoneAnimationContainer.IsVisible = true;
                DoneAnimation.Play();
                soundOne.Play();
            }
            else
            {
                var Pop = new Pages.PopUps.ErrorMessage("Error", Response, "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
            }
        }

        private async void PressAddExercise(object sender, EventArgs e)
        {           
            //Handle popup page 
            Application.Current.ModalPopping += HandleModalPopping;
            myPage = new AddExercise.Single(App.exerciseListApp);

            await Application.Current.MainPage.Navigation.PushModalAsync(myPage);
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

                    RoutineList NewExerciseObject = new RoutineList();

                    NewExerciseObject.ExerciseDescription = PageExerciseObject.ExerciseDescription;
                    NewExerciseObject.KeepRow = false;
                    NewExerciseObject.Reps = "0";
                    NewExerciseObject.RestTimeAfterSet = 30;
                    NewExerciseObject.SetNumber = 1;
                    NewExerciseObject.Weight = "0";                                      

                    int NewExerciseNumber = doc.Element("Routine").Elements("Exercise").Max(Number => Int32.Parse(Number.Element("Number").Value)) + 1;
                    NewExerciseObject.ExerciseNumber = NewExerciseNumber;

                    XElement ExerciseX = new XElement("Exercise",
                                            new XElement("Number", NewExerciseNumber),
                                              new XElement("Description", PageExerciseObject.ExerciseDescription),
                                              new XElement("ID", PageExerciseObject.ExerciseID),
                                              new XElement("RestTimeAfterSet", 60),
                                              new XElement("Set",
                                                  new XElement("Number", "1"),
                                                  new XElement("Reps", "0"),
                                                  new XElement("Weight", "0")
                                                 ) // end Set 
                                             ); // end Exercise

                    doc.Element("Routine").Add(ExerciseX); // end Exercise);                   

                    BuildRoutine(2);

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

        private void LockRoutine()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                var LockSound = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                LockSound.Load("Lock.wav");
                LockSound.Play();

                if (LockButton.Opacity == 0.25)
                {
                    LockButton.Source = "Lock.png";
                    LockButton.Opacity = 1;
                    BuildingRoutine.Locked = true;
                }
                else
                {
                    LockButton.Source = "Unlock.png";
                    LockButton.Opacity = 0.25;
                    BuildingRoutine.Locked = false;
                }
            });
        }

        override protected bool OnBackButtonPressed()
        {

            LeavePageCheck();

            return true;
        }

        private async void LeavePageCheck()
        {
            string typeOfTransaction;
            if (editOrCreate == "edit")
            {
                typeOfTransaction = "editing";
            }
            else {
                typeOfTransaction = "creating";
            }
            var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", $"Are you sure you want to cancel {typeOfTransaction} your ROUTINE?", "Yes", "No");
            var answer = await QuestionPop.Show();

            if (answer) // Yes
            {
                if (isFromClient == true)
                {
                    await Navigation.PushAsync(new Client.ClientRoutine(clientToPass));
                }
                else
                {
                    await Navigation.PushAsync(new Routines());
                }
            }
        }

        private async void AnimationComplete(object sender, EventArgs e)
        {
            await EndRoutineAsync();
        }

        private async Task EndRoutineAsync()
        {
            if (isFromClient == true)
            {
                await Navigation.PushAsync(new Pages.Client.ClientRoutine(clientToPass));
            }
            else
            {
                await Navigation.PushAsync(new Pages.Routine.Routines());
            }
        }

    }
}