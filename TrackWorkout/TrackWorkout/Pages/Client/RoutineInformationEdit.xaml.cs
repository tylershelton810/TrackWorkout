using System;
using System.Collections.Generic;
using System.Linq;
using TrackWorkout.Entitys;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Data;
using Rg.Plugins.Popup.Extensions;

namespace TrackWorkout.Pages.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutineInformationEdit : ContentPage
    {
        Entitys.ClientInformation ClientInfo;
        AddExercise.Single myPage;
        string editOrCreate;
        List<RoutineList> BuildingRoutine;
        List<RoutineList> AddedDuringRoutine;

        Frame Filler = new Frame
        {
            BackgroundColor = Color.Transparent,
            HeightRequest = 70,
            HasShadow = false
        };

        Grid HeaderContent = new Grid
        {
            RowSpacing = 0,
            ColumnSpacing = 0,
            BackgroundColor = App.SecondaryThemeColor,
            ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = 70 },
                            new ColumnDefinition { Width = GridLength.Auto }
                        }
        };

        Entry RoutineName = new Entry
        {
            Placeholder = "Routine Name",
            HeightRequest = 50,
            MaxLength = 120
        };

        Label HeaderLabel = new Label
        {
            Text = "Create Routine",
            FontSize = 15,
            FontAttributes = FontAttributes.Bold,
            HeightRequest = 20,
            Margin = new Thickness(15, 0, 0, 0),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            TextColor = Color.White
        };

        ImageButton LockButton = new ImageButton
        {
            Source = "Unlock.png",
            Opacity = 0.25,
            BackgroundColor = App.PrimaryThemeColor,
            VerticalOptions = LayoutOptions.End
        };

        Button FinishButton = new Button
        {
            Text = "Finish",
            TextColor = Color.White,
            BackgroundColor = App.PrimaryThemeColor,
            VerticalOptions = LayoutOptions.End
        };

        public RoutineInformationEdit(List<RoutineList> exercises, Entitys.ClientInformation Client)
        {
            InitializeComponent();

            if (exercises[0].RoutineName != null)
            {
                RoutineName.Text = exercises[0].RoutineName;
                editOrCreate = "edit";
                FinishButton.Text = "Update";
                HeaderLabel.Text = "Update Routine";
                if (exercises[0].Locked == true)
                {
                    LockButton.Source = "Lock.png";
                    LockButton.Opacity = 1;
                }
            }
            else
            {
                editOrCreate = "create";
            }

            BuildingRoutine = exercises;

            AddedDuringRoutine = new List<RoutineList>();

            //addedID = 0;
            //OrderID = 1;
            ClientInfo = Client;

            NavigationPage.SetHasNavigationBar(this, false);

            // Create Frame
            Frame HeaderFrame = new Frame
            {
                HasShadow = false,
                Padding = 0,
            };

            Frame RoutineNameFrame = new Frame
            {
                HasShadow = false,
                Padding = 0,
            };

            // Add Row to Grid that will contain the headers. Then add the headers to the grid
            HeaderContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(90, GridUnitType.Absolute) });
            HeaderContent.Children.Add(HeaderLabel, 0, 0);
            HeaderContent.Children.Add(LockButton, 1, 0);
            HeaderContent.Children.Add(FinishButton, 2, 0);

            // Add Grid to Frame
            HeaderFrame.Content = HeaderContent;

            // Add Entry to Frame
            RoutineNameFrame.Content = RoutineName;

            // Add Frame to page
            HeaderLayout.Children.Add(HeaderFrame);
            HeaderLayout.Children.Add(RoutineNameFrame);

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
            };
        }

        private void BuildRoutine(int Type)
        {
            List<int> uniqueExerciseNumber;


            if (Type == 1)
            {
                // Create new list with unique routine id
                uniqueExerciseNumber = BuildingRoutine.Select(x => x.ExerciseNumber).AsParallel().Distinct().ToList();
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
                    listOfSetWithSameExerciseNumber = BuildingRoutine.Where(value => value.ExerciseNumber == uniqueExerciseNumber[i]).ToList();
                    ExerciseLabelText = listOfSetWithSameExerciseNumber.Min(minExerciseDescription => minExerciseDescription.ExerciseDescription);
                }
                else
                {
                    listOfSetWithSameExerciseNumber = AddedDuringRoutine.Where(value => value.ExerciseNumber == uniqueExerciseNumber[i]).ToList();
                    ExerciseLabelText = listOfSetWithSameExerciseNumber.Min(minExerciseDescription => minExerciseDescription.ExerciseDescription);
                }
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
                    TextColor = App.PrimaryThemeColor
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
                    TextColor = App.PrimaryThemeColor
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
                FillGrid(listOfSetWithSameExerciseNumber, ExerciseContent, ExerciseHeader, ExerciseFrame, Type);

                // Add Grid to Frame
                ExerciseFrame.Content = ExerciseContent;

                // Add Frame to page
                gridStack.Children.Add(ExerciseFrame);
            }
        }

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
                BuildingRoutine.Add(newRoutineObject);
            }

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

            WeightEntry.Focused += (o, e) =>
            {
                WeightEntry.Text = "";
            };

            WeightEntry.Unfocused += (o, e) =>
            {
                foreach (var ObjectToUpdate in BuildingRoutine.Where(ExerciseID => ExerciseID.ExerciseNumber == newRoutineObject.ExerciseNumber).Where(SetID => SetID.SetNumber == newRoutineObject.SetNumber))
                {
                    if (WeightEntry.Text == "")
                    {
                        if (ObjectToUpdate.Weight == "0" || ObjectToUpdate.Weight == "")
                        {
                            WeightEntry.Text = "0";
                        }
                        else
                        {
                            WeightEntry.Text = ObjectToUpdate.Weight;
                        }
                    }
                    else
                    {
                        ObjectToUpdate.Weight = WeightEntry.Text;
                    }
                }

            };

            RepEntry.Focused += (o, e) =>
            {
                RepEntry.Text = "";
            };

            RepEntry.Unfocused += (o, e) =>
            {
                foreach (var ObjectToUpdate in BuildingRoutine.Where(ExerciseID => ExerciseID.ExerciseNumber == newRoutineObject.ExerciseNumber).Where(SetID => SetID.SetNumber == newRoutineObject.SetNumber))
                {
                    if (RepEntry.Text == "")
                    {
                        if (ObjectToUpdate.Reps == "0" || ObjectToUpdate.Reps == "")
                        {
                            RepEntry.Text = "0";
                        }
                        else
                        {
                            RepEntry.Text = ObjectToUpdate.Reps;
                        }
                    }
                    else
                    {
                        ObjectToUpdate.Reps = RepEntry.Text;
                    }
                }
            };

            AddSet.Clicked += (o, e) =>
            {
                AddSetClick(ExerciseContent, listOfSetWithSameExerciseNumber, ExerciseHeader, ExerciseFrame);
            };

            RemoveSet.Clicked += async (o, e) =>
            {
                var lastListElement = listOfSetWithSameExerciseNumber.Count - 1;

                // Stops the last set from being removed.
                if (BuildingRoutine.Count == 1)
                {
                    var Pop = new Pages.PopUps.ErrorMessage("Warning", "This is the last set. This cannot be removed.", "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);

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

            AddRow(listOfSetWithSameExerciseNumber, ExerciseContent, lastListElement, ExerciseHeader, ExerciseFrame, 1);
        }

        private void LockRoutine()
        {
            var LockSound = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                LockSound.Load("Lock.wav");
                LockSound.Play();

                if (LockButton.Opacity == 0.25)
                {
                    LockButton.Source = "Lock.png";
                    LockButton.Opacity = 1;

                    foreach (var ObjectToUpdate in BuildingRoutine)
                    {
                        ObjectToUpdate.Locked = true;
                    }
                }
                else
                {
                    LockButton.Source = "Unlock.png";
                    LockButton.Opacity = 0.25;

                    foreach (var ObjectToUpdate in BuildingRoutine)
                    {
                        ObjectToUpdate.Locked = false;
                    }
                }
            });
        }


        private async void FinishRoutine()
        {
            var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", "Are you finished with your routine", "Yes", "No");
            var answer = await QuestionPop.Show();
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
                    var Pop = new Pages.PopUps.ErrorMessage("Invalid Routine Name", $"Please add a routine name", "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                }

                else if (weightValid == false)
                {
                    var Pop = new Pages.PopUps.ErrorMessage("Fill in all weight fields", $"There are weight fields that are not filled", "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                }
                else if (repValid == false)
                {
                    var Pop = new Pages.PopUps.ErrorMessage("Fill in all rep fields", $"There are rep fields that are not filled", "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                }
                else
                {
                    BuildingRoutine[0].RoutineName = RoutineName.Text;
                    BuildingRoutine[0].UserID = ClientInfo.ClientID;
                    BuildingRoutine[0].ProcedureType = 2;
                    BuildingRoutine[0].CoachID = ClientInfo.CoachID;
                    InsertRoutine();
                }
            }


        }

        private async void InsertRoutine()
        {
            string messageType;

            BuildingRoutine[0].RoutineName = RoutineName.Text;
            if (editOrCreate == "edit")
            {
                BuildingRoutine[0].ProcedureType = 3;
                messageType = "updated";
            }
            else
            {
                messageType = "created";
            }

            var errorPop = new Pages.PopUps.Loading("Adding Routine");
            await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);

            string Response = await SharedClasses.WebserviceCalls.AddRoutine(BuildingRoutine);

            await App.Current.MainPage.Navigation.PopPopupAsync();

            if (Response == "success")
            {
                var Pop = new Pages.PopUps.InformationMessage("Routine Added!", $"{RoutineName.Text} has been successfully {messageType}!", "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);              

                await Navigation.PushAsync(new ClientRoutine(ClientInfo));
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

                    var CurrentMaxExerciseNumber = BuildingRoutine.Max(maxExerciseNumber => maxExerciseNumber.ExerciseNumber);
                    NewExerciseObject.ExerciseNumber = CurrentMaxExerciseNumber + 1;

                    string NameOfRoutine = BuildingRoutine.Min(RoutineName => RoutineName.RoutineName);

                    if (NameOfRoutine != null)
                    {
                        NewExerciseObject.RoutineName = BuildingRoutine.Min(RoutineName => RoutineName.RoutineName);
                    }

                    AddedDuringRoutine.Add(NewExerciseObject);

                    BuildRoutine(2);
                }

                // remember to remove the event handler:
                myPage = null;
                AddedDuringRoutine = new List<RoutineList>();
                Application.Current.ModalPopping -= HandleModalPopping;

                //Remove old filler at bottom of page
                gridStack.Children.Remove(Filler);

                //add new filler at bottom of page
                gridStack.Children.Add(Filler);

            }

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
            else
            {
                typeOfTransaction = "creating";
            }
            var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", $"Are you sure you want to cancel {typeOfTransaction} your routine?", "Yes", "No");
            var answer = await QuestionPop.Show();
            if (answer) // Yes
            {
                await Navigation.PushAsync(new ClientRoutine(ClientInfo));
            }
        }

    }
}