using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TrackWorkout.Entitys;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.AddExercise
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Single : ContentPage
    {
        List<ExerciseList> listExercises = new List<ExerciseList>();
        
        public ExerciseList returnExercise;
        public string TypeOfCall;
        public int SupersetID;

        public Single(List<ExerciseList> exerciseList, string Type, int superID = 0)
        {
            InitializeComponent();

            //This is used to know what called the function 
            TypeOfCall = Type;

            //If passed with a superset it will have a value if not it will default to 0 approprietly. 
            SupersetID = superID;

            listExercises = exerciseList;

            List<char> uniqueFirstLetter = listExercises.Select(x => x.FirstLetter).AsParallel().Distinct().ToList();
            uniqueFirstLetter.Sort();

            for (int i = 0; i < uniqueFirstLetter.Count; i++)
            {
                BuildNewWay(uniqueFirstLetter[i]);
            }
            
        }

        public void BuildNewWay(char X)
        {
            var listOfExercisesWithSameFirstLetter = listExercises.Where(value => value.FirstLetter == X).ToList();            

            Label LetterLabel = new Label
            {
                Text = X.ToString(),
                FontSize = 15,
                FontFamily = App.CustomBold,
                HeightRequest = 20,
                Margin = new Thickness(15, 0, 0, 0),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                TextColor = App.PrimaryThemeColor
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
            ExerciseList ObjectToReturn = new ExerciseList();

            ObjectToReturn.ExerciseDescription = listOfExercisesWithSameFirstLetter[i].ExerciseDescription;
            ObjectToReturn.ExerciseID = listOfExercisesWithSameFirstLetter[i].ExerciseID;
            ObjectToReturn.FirstMuscleCode = listOfExercisesWithSameFirstLetter[i].FirstMuscleCode;
            ObjectToReturn.FirstMuscleCodeDescription = listOfExercisesWithSameFirstLetter[i].FirstMuscleCodeDescription;
            ObjectToReturn.SecondMuscleCode = listOfExercisesWithSameFirstLetter[i].SecondMuscleCode;
            ObjectToReturn.SecondMuscleCodeDescription = listOfExercisesWithSameFirstLetter[i].SecondMuscleCodeDescription;
            ObjectToReturn.ThirdMuscleCode = listOfExercisesWithSameFirstLetter[i].ThirdMuscleCode;
            ObjectToReturn.ThirdMuscleCodeDescription = listOfExercisesWithSameFirstLetter[i].ThirdMuscleCodeDescription;
            ObjectToReturn.SupersetID = SupersetID;

            Button ExerciseButton = new Button
            {
                Text = ObjectToReturn.ExerciseDescription,
                FontFamily = App.CustomRegular,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = App.ThemeColor4,
                BackgroundColor = Color.Transparent
            };                        

            ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50, GridUnitType.Absolute) });

            ExerciseContent.Children.Add(ExerciseButton, 0, i);

            ExerciseButton.Clicked += async (o, e) =>
            {
                //What happens when the button is clicked. This needs to be handled by TypeOfCall
                switch (TypeOfCall)
                {
                    //Set the fonts per platform
                    case "Add To Routine":
                        ObjectToReturn.SupersetID = SupersetID;
                        var QuestionPopAddToRoutine = new Pages.PopUps.UserChoiceMessage("Confirm", $"Would you like to add {ExerciseButton.Text} to your routine?", "Yes", "No");
                        var answerAddToRoutine = await QuestionPopAddToRoutine.Show();
                        if (answerAddToRoutine) // Yes
                        {
                            returnExercise = ObjectToReturn;
                            PopThisPage();
                        }

                        break;
                    case "Superset":
                        var QuestionPopSuperset = new Pages.PopUps.UserChoiceMessage("Confirm", $"Would you like to add {ExerciseButton.Text} to your Superset?", "Yes", "No");
                        var answerSuperset = await QuestionPopSuperset.Show();
                        if (answerSuperset) // Yes
                        {
                            returnExercise = ObjectToReturn;
                            PopThisPage();
                        }

                        break;
                }
            };
                                   
        }

        override protected bool OnBackButtonPressed()
        {
            returnExercise = null;
            PopThisPage();

            return true;
        }

       
        private async void PopThisPage()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

    }

}