using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TrackWorkout.Entitys;
using System.Xml.Linq;

namespace TrackWorkout.Pages.AddExercise
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickExercise : ContentPage
    {
        List<ExerciseList> listExercises = new List<ExerciseList>();

        List<RoutineList> selectedExercises = new List<RoutineList>();

        Entitys.ClientInformation clientToPass = new Entitys.ClientInformation();

        XDocument doc;

        Color colorToUse = new Color();

        public PickExercise(Entitys.ClientInformation Client)
        {
            InitializeComponent();

            clientToPass = Client;

            if (clientToPass == null)
            {
                colorToUse = App.PrimaryThemeColor;
            }
            else
            {
                colorToUse = App.PrimaryThemeColor;
            }

            addExercises.BackgroundColor = colorToUse;
            addExercises.FontFamily = App.CustomRegular;

            doc = new XDocument(new XElement("Routine",
                                    new XElement("Name", "")));

            addExercisesFrame.IsVisible = false;

            listExercises = App.exerciseListApp;

            List<char> uniqueFirstLetter = listExercises.Select(x => x.FirstLetter).AsParallel().Distinct().ToList();
            uniqueFirstLetter.Sort();

            for (int i = 0; i < uniqueFirstLetter.Count; i++)
            {
                BuildNewWay(uniqueFirstLetter[i]);
            };
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
                TextColor = colorToUse
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
                FontFamily = App.CustomRegular,
                TextColor = App.ThemeColor4,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent
            };

            ExerciseContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50, GridUnitType.Absolute) });

            ExerciseContent.Children.Add(ExerciseButton, 0, i);

            ExerciseButton.Clicked += (o, e) =>
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
                    ExerciseButton.BackgroundColor = colorToUse;
                    ExerciseButton.TextColor = Color.White;


                    selectedExercises.Add(newRoutineObject);

                    XElement ExerciseX = new XElement("Exercise",
                    new XElement("Number", newRoutineObject.ExerciseNumber),
                                               new XElement("Description", newRoutineObject.ExerciseDescription),
                                               new XElement("ID", newExerciseObject.ExerciseID),
                                               new XElement("RestTimeAfterSet", newRoutineObject.RestTimeAfterSet),
                                               new XElement("Set",
                                                   new XElement("Number", "1"),
                                                   new XElement("Reps", "0"),
                                                   new XElement("Weight", "0")
                                                  ) // end Set 
                                              ); // end Exercise

                    doc.Element("Routine").Add(ExerciseX); // end Exercise);

                    addExercisesFrame.IsVisible = true;
                }

                else
                {
                    ExerciseButton.BackgroundColor = Color.Transparent;
                    ExerciseButton.TextColor = Color.Black;

                    foreach (var element in doc.Element("Routine").Elements("Exercise"))
                    {
                        if (element.Element("Description").Value == newRoutineObject.ExerciseDescription)
                        {
                            foreach (var elementAfterDeleted in doc.Element("Routine").Elements("Exercise").Where(x => Int32.Parse(x.Element("Number").Value) > Int32.Parse(element.Element("Number").Value)))
                            {
                                int storeNumber = Int32.Parse(elementAfterDeleted.Element("Number").Value);
                                storeNumber = storeNumber - 1;
                                elementAfterDeleted.Element("Number").SetValue(storeNumber);
                            }
                            element.Remove();
                        }
                    }

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
            selectedExercises[0].RoutineDetail = doc.ToString();
            await Navigation.PushAsync(new Pages.Routine.RoutineInformationEdit(selectedExercises[0], clientToPass));
        }
    }
}