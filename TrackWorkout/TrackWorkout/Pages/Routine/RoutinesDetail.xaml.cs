using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Rg.Plugins.Popup.Extensions;
using TrackWorkout.Entitys;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace TrackWorkout.Pages.Routine
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutinesDetail : ContentPage
    {
        Frame Filler = new Frame
        {
            BackgroundColor = Color.Transparent,
            HeightRequest = 70
        };

        public RoutinesDetail()
        {
            InitializeComponent();

            NoRoutineLabel.FontFamily = App.CustomBold;
            NoRoutineLabel.TextColor = App.SecondaryThemeColor;
            addButton.BackgroundColor = App.SecondaryThemeColor;
            CoachRoutines.BackgroundColor = App.PrimaryThemeColor;
            addButtonFrame.BackgroundColor = App.PrimaryThemeColor;
            addButtonFrame2.BackgroundColor = App.SecondaryThemeColor;
            ButtonGridBottom.BackgroundColor = App.PrimaryThemeColor;

            if (App.CoachAssignedRoutineListApp.Count == 0)
            {
                CoachRoutines.Opacity = 0.25;
            }

            if (App.UserRoutineListApp.Count != 0)            
            {
                NoRoutineLabel.IsVisible = false;
                NoRoutineAnimation.IsVisible = false;
                // Create new list with unique routine id
                List<int> uniqueRoutineID = App.UserRoutineListApp.Select(x => x.RoutineID).AsParallel().Distinct().ToList();
                uniqueRoutineID.Sort();
                // Loop through new list
                for (int i = 0; i < uniqueRoutineID.Count; i++)
                {
                    CreateRoutineGrids(uniqueRoutineID, i);
                }

                gridStack.Children.Add(Filler);
            }

            
        }

        private void AddFrameClick(object sender, EventArgs e)
        {
            AddNewRoutine(sender, e);
        }

        public void CreateRoutineGrids(List<int> uniqueRoutineID, int i)
        {
            // I need to set the set ID of the new object added then use that to control the GridCount
            var currentRoutine = App.UserRoutineListApp[i];

            string RoutineLabelText = currentRoutine.RoutineName;

            // Create Name Label
            Label RoutineLabel = new Label
            {
                Text = RoutineLabelText,
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                HeightRequest = 20,
                Margin = new Thickness(15, 10, 0, 0),
                //FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                TextColor = App.SecondaryThemeColor
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
                BackgroundColor = App.SecondaryThemeColor
            };

            // Create Options Button
            ImageButton OptionsButton = new ImageButton
            {
                Source = "LineMenu.png",
                HeightRequest = 20,
                Margin = new Thickness(0, 10, 20, 0),
                WidthRequest = 20,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = Color.Transparent,
                Aspect = Aspect.Fill
            };

            bool OptionPressed = false;
            // Create Frame
            Frame RoutineFrame = new Frame
            {
                HasShadow = false,
                BackgroundColor = Color.FromHex("#f5f5f5"),
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

            Grid backgroundMenu = new Grid
            {
                RowSpacing = 0,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(0, 5, 0, 5),
                WidthRequest = 100,
                RowDefinitions = {
                    new RowDefinition { Height = 50 },
                    new RowDefinition { Height = 50 }
                }
            };

            Grid holderGrid = new Grid
            {
                RowSpacing = 0,
                BackgroundColor = Color.Transparent,
                RowDefinitions = {
                    new RowDefinition { Height = GridLength.Star }
                }
            };

            Frame DeleteFrame = new Frame
            {
                Padding = 1,
                HasShadow = false,
                CornerRadius = 20,
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            Frame EditFrame = new Frame
            {
                Padding = 1,
                HasShadow = false,
                CornerRadius = 20,
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            // Create Options Button
            ImageButton DeleteRoutineButton = new ImageButton
            {
                Source = "Trash.png",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Color.Transparent,
                CornerRadius = 20,
                WidthRequest = 40,
                HeightRequest = 40
            };

            // Create Options Button
            ImageButton EditRoutineButton = new ImageButton
            {
                Source = "Edit.png",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Color.Transparent,
                CornerRadius = 20,
                WidthRequest = 40,
                HeightRequest = 40
            };

            DeleteFrame.Scale = 0;
            EditFrame.Scale = 0;

            DeleteFrame.Content = DeleteRoutineButton;
            EditFrame.Content = EditRoutineButton;

            backgroundMenu.Children.Add(DeleteFrame, 0, 0);
            backgroundMenu.Children.Add(EditFrame, 0, 1);

            holderGrid.Children.Add(backgroundMenu, 0, 0);

            // Add Row to Grid that will contain the option button. Then add the button to row last column
            routineContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
            routineContent.Children.Add(OptionsButton, 1, 0);

            // Fill the grid with workout content
            FillGrid(currentRoutine, routineContent);

            // Create a new row to store the start routine button then add the button to the row.
            routineContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
            routineContent.Children.Add(StartRoutine, 0, 3);
            Grid.SetColumnSpan(StartRoutine, 2);

            // Add Grid to Frame
            RoutineFrame.Content = routineContent;

            holderGrid.Children.Add(RoutineFrame, 0, 0);

            // What to do when clicking the start routine button
            StartRoutine.Clicked += (o, e) =>
            {
                StartNewRoutine(currentRoutine, RoutineLabelText, RoutineFrame);
            };

            OptionsButton.Clicked += async (object o, EventArgs e) =>
            {
                if (OptionPressed == false)
                {
                    await Task.WhenAll(
                        RoutineFrame.TranslateTo(-100, 0, 750, Easing.BounceOut),
                        DeleteFrame.RotateTo(270, 750),
                        EditFrame.RotateTo(270, 750),
                        DeleteFrame.ScaleTo(1, 500),
                        EditFrame.ScaleTo(1, 500),
                        OptionsButton.RotateTo(360, 750),
                        OptionsButton.ScaleTo(0, 750)
                    );

                    OptionsButton.Source = "Forward.png";

                    await Task.WhenAll(
                        DeleteFrame.RotateTo(0, 500, Easing.SpringOut),
                        EditFrame.RotateTo(0, 500, Easing.SpringOut),
                        OptionsButton.ScaleTo(1, 500, Easing.SpringOut)
                        );


                    OptionPressed = true;
                }
                else
                {
                    await Task.WhenAll(
                        RoutineFrame.TranslateTo(0, 0, 1000, Easing.BounceOut),
                        OptionsButton.RotateTo(0, 1000),
                        OptionsButton.ScaleTo(0, 1000),
                        DeleteFrame.ScaleTo(0, 750),
                        EditFrame.ScaleTo(0, 750)
                    );

                    OptionsButton.Source = "LineMenu.png";

                    await OptionsButton.ScaleTo(1, 500, Easing.SpringOut);

                    OptionPressed = false;
                }
            };

            EditRoutineButton.Clicked += async (object o, EventArgs e) =>
            {
                var QuestionPop = new Pages.PopUps.UserChoiceMessage("EDIT ROUTINE", $"Would you like to edit {RoutineLabelText}?", "Yes", "No");
                var answer = await QuestionPop.Show();
                if (answer)
                {
                    await Navigation.PushAsync(new Pages.Routine.RoutineInformationEdit(currentRoutine, null));
                }

                await Task.WhenAll(
                        RoutineFrame.TranslateTo(0, 0, 1000, Easing.BounceOut),
                        OptionsButton.RotateTo(0, 1000),
                        OptionsButton.ScaleTo(0, 1000),
                        DeleteFrame.ScaleTo(0, 750),
                        EditFrame.ScaleTo(0, 750)
                    );

                OptionsButton.Source = "LineMenu.png";

                await OptionsButton.ScaleTo(1, 500, Easing.SpringOut);

                OptionPressed = false;
            };

            DeleteRoutineButton.Clicked += async (object o, EventArgs e) =>
            {
                var QuestionPop = new Pages.PopUps.UserChoiceMessage("DELETE ROUTINE!", $"Are you sure you want to delete {RoutineLabelText} and all of the associated history?", "Yes", "No");
                var answer = await QuestionPop.Show();
                if (answer)
                {

                    PassID RoutineIDToPass = new PassID();

                    RoutineIDToPass.ID = currentRoutine.RoutineID.ToString();

                    var errorPop = new Pages.PopUps.Loading("Deleting Routine");
                    await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);

                    string Response = await SharedClasses.WebserviceCalls.DeleteRoutine(RoutineIDToPass);                    

                    if (Response == "success")
                    {
                        PassID idToPass = new PassID { ID = App.userInformationApp[0].UserId.ToString() };

                        Response = await SharedClasses.WebserviceCalls.GetUserRoutineList(idToPass);

                        await App.Current.MainPage.Navigation.PopPopupAsync();

                        if (Response == "success")
                        {                           
                            await Task.WhenAll(
                            holderGrid.TranslateTo(-500, 0, 1000),
                            holderGrid.ScaleTo(0, 1000),
                            RoutineLabel.TranslateTo(-500, 0, 1000),
                            RoutineLabel.ScaleTo(0, 1000)
                            );

                            gridStack.Children.Remove(RoutineLabel);

                            foreach (var child in holderGrid.Children.ToList())
                            {
                                holderGrid.Children.Remove(child);
                            }

                            gridStack.Children.Remove(holderGrid);
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
                    await Task.WhenAll(
                        RoutineFrame.TranslateTo(0, 0, 1000, Easing.BounceOut),
                        OptionsButton.RotateTo(0, 1000),
                        OptionsButton.ScaleTo(0, 1000),
                        DeleteFrame.ScaleTo(0, 750),
                        EditFrame.ScaleTo(0, 750)
                    );

                    OptionsButton.Source = "LineMenu.png";

                    await OptionsButton.ScaleTo(1, 500, Easing.SpringOut);

                    OptionPressed = false;
                }
            };

            // Add Frame to page
            gridStack.Children.Add(holderGrid);

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

        // When hitting the add new routine button go to the PickExcercise page to begin the process 
        // of creating a new routine.
        private async void AddNewRoutine(object sender, EventArgs e)
        {
            

            await Navigation.PushAsync(new AddExercise.PickExercise(null));
        }

        private async void CoachRoutinesClicked(object sender, EventArgs e)
        {
            if (App.CoachAssignedRoutineListApp.Count == 0)
            {
                var Pop = new Pages.PopUps.InformationMessage("No Coach ROUTINES", "To use the Coach ROUTINE feature, one of your Coaches must first create a ROUTINE for you.", "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
            }
            else
            {
                await Navigation.PushAsync(new RoutinesCoaches());
            }
        }

        // When hitting the start routine button a pop up appears asking for confirmation to start. Once confirmed the routine will begin
        private async void StartNewRoutine(RoutineList currentRoutine, string RoutineLabelText, Frame RoutineFrame)
        {
            var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", "Are you ready to start the " + RoutineLabelText + " ROUTINE", "Yes", "No");
            var answer = await QuestionPop.Show();
            if (answer) // Yes
            {                
                await Navigation.PushAsync(new RoutineInProgress(currentRoutine, "User", "Routine"));
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