using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TrackWorkout.Entitys;

namespace TrackWorkout.Pages.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickExercise : ContentPage
    {
        List<ExerciseList> listExercises = new List<ExerciseList>();

        List<RoutineList> selectedExercises = new List<RoutineList>();

        Entitys.ClientInformation ClientInfo;

        public PickExercise(Entitys.ClientInformation Client)
        {
            InitializeComponent();

            addExercises.BackgroundColor = App.PrimaryThemeColor;

            addExercisesFrame.IsVisible = false;            

            listExercises = App.exerciseListApp;

            List<char> uniqueFirstLetter = listExercises.Select(x => x.FirstLetter).AsParallel().Distinct().ToList();
            uniqueFirstLetter.Sort();

            for (int i = 0; i < uniqueFirstLetter.Count; i++)
            {
                BuildNewWay(uniqueFirstLetter[i]);
            }

            ClientInfo = Client;

            //BuildList();
        }

        public void BuildNewWay(char X)
        {
            var listOfExercisesWithSameFirstLetter = listExercises.Where(value => value.FirstLetter == X).ToList();

            Label LetterLabel = new Label
            {
                Text = X.ToString(),
                FontSize = 15,
                HeightRequest = 20,
                Margin = new Thickness(15, 0, 0, 0),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                TextColor = App.PrimaryThemeColor
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
                BackgroundColor = Color.Transparent,
                ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star }
                        }
            };

            ExerciseHeader.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Absolute) });
            ExerciseHeader.Children.Add(LetterLabel, 0, 0);

            gridStack.Children.Add(ExerciseHeader);

            Grid ExerciseContent = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                BackgroundColor = Color.White,
                ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star }
                        }
            };

            for (int i = 0; i < listOfExercisesWithSameFirstLetter.Count; i++)
            {
                AddRow(listOfExercisesWithSameFirstLetter, ExerciseContent, i);
            }

            // Add Grid to Frame
            ExerciseFrame.Content = ExerciseContent;

            // Add Frame to page
            gridStack.Children.Add(ExerciseFrame);

        }

        private void AddRow(List<ExerciseList> listOfExercisesWithSameFirstLetter, Grid ExerciseContent, int i)
        {

            ExerciseList newExerciseObject = new ExerciseList();

            newExerciseObject.ExerciseDescription = listOfExercisesWithSameFirstLetter[i].ExerciseDescription;
            newExerciseObject.ExerciseID = listOfExercisesWithSameFirstLetter[i].ExerciseID;
            newExerciseObject.FirstMuscleCode = listOfExercisesWithSameFirstLetter[i].FirstMuscleCode;
            newExerciseObject.FirstMuscleCodeDescription = listOfExercisesWithSameFirstLetter[i].FirstMuscleCodeDescription;
            newExerciseObject.SecondMuscleCode = listOfExercisesWithSameFirstLetter[i].SecondMuscleCode;
            newExerciseObject.SecondMuscleCodeDescription = listOfExercisesWithSameFirstLetter[i].SecondMuscleCodeDescription;
            newExerciseObject.ThirdMuscleCode = listOfExercisesWithSameFirstLetter[i].ThirdMuscleCode;
            newExerciseObject.ThirdMuscleCodeDescription = listOfExercisesWithSameFirstLetter[i].ThirdMuscleCodeDescription;

            Button ExerciseButton = new Button
            {
                Text = newExerciseObject.ExerciseDescription,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent
            };

            ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50, GridUnitType.Absolute) });

            ExerciseContent.Children.Add(ExerciseButton, 0, i);

            ExerciseButton.Clicked += (o, e) => //async
            {
                RoutineList newRoutineObject = new RoutineList();

                newRoutineObject.ExerciseDescription = newExerciseObject.ExerciseDescription;
                newRoutineObject.ExerciseNumber = selectedExercises.Count + 1;
                newRoutineObject.Reps = "0";
                newRoutineObject.RestTimeAfterSet = 60;
                newRoutineObject.SetNumber = 1;
                newRoutineObject.Weight = "0";

                if (ExerciseButton.BackgroundColor == Color.Transparent)
                {

                    ExerciseButton.BackgroundColor = App.PrimaryThemeColor;
                    ExerciseButton.TextColor = Color.White;

                    selectedExercises.Add(newRoutineObject);

                    addExercisesFrame.IsVisible = true;                   
                }

                else
                {
                    ExerciseButton.BackgroundColor = Color.Transparent;
                    ExerciseButton.TextColor = Color.Black;

                    var getObjectToIndex = selectedExercises.Where<RoutineList>(x => x.ExerciseDescription == newRoutineObject.ExerciseDescription).Single<RoutineList>();
                    int indexToDelete = selectedExercises.IndexOf(getObjectToIndex);

                    selectedExercises.RemoveAt(indexToDelete);                    

                    if (selectedExercises.Count < 1)
                    {
                        addExercisesFrame.IsVisible = false;
                    }                    
                }

            };

        }
        
        private async void addExercisesClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Client.RoutineInformationEdit(selectedExercises, ClientInfo)); 
        }
    }
}