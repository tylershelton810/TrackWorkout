using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Json.Net;
using System.Net.Http;
using Newtonsoft.Json;
using TrackWorkout.Entitys;

namespace TrackWorkout.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickExercise : ContentPage
    {
        List<ExerciseList> selectedExercises = new List<ExerciseList>();

        string UserID;

        List<UserInformation> UserList;


        public PickExercise(List<UserInformation> userInformation)
        {
            InitializeComponent();

            UserID = userInformation[0].UserId.ToString();

            UserList = userInformation;

            BuildList();
        }

        public async void BuildList()
        {
            // Use the Email object to get the rest of the data set
            var client = new HttpClient();
            // API call to webservice to save user in the database
            var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetExerciseList", null);
            var responseString = await response.Content.ReadAsStringAsync();
            var exerciseList = JsonConvert.DeserializeObject<ExerciseListRootObject>(responseString);
            
            char firstLetter = 'Z';
            int OutGridRowCount = 0;                        

            for (int i = 0; i < exerciseList.ExerciseList.Count; i++)
            {
                Button dynamicButton = new Button();
                Label dynamicLabel = new Label();
                BoxView labelBoxView = new BoxView();

                labelBoxView.BackgroundColor = Color.FromHex("#dc5b81");

                //int ExerciseID = exerciseList.ExerciseList[i].ExerciseID;
                string ExerciseDescription = exerciseList.ExerciseList[i].ExerciseDescription;
                ExerciseList exerciseObject = exerciseList.ExerciseList[i];

                exerciseObject.UserID = UserID;

                char FirstValue = ExerciseDescription[0];

                // Set Button properties
                dynamicButton.Text = ExerciseDescription;
                dynamicButton.BackgroundColor = Color.White; // Pink
                dynamicButton.TextColor = Color.Black;
                dynamicButton.HorizontalOptions = LayoutOptions.FillAndExpand;
                dynamicButton.HeightRequest = 50;
                dynamicButton.FontAttributes = FontAttributes.Bold;

                // Set Label properties
                dynamicLabel.Text = FirstValue.ToString();
                dynamicLabel.FontAttributes = FontAttributes.Bold;
                dynamicLabel.FontSize = 25;
                dynamicLabel.HorizontalOptions = LayoutOptions.Center;
                dynamicLabel.VerticalOptions = LayoutOptions.Center;
                dynamicLabel.TextColor = Color.White;
                dynamicLabel.BackgroundColor = Color.FromHex("#dc5b81");

                dynamicButton.Clicked += (o, e) =>
                {
                    if (dynamicButton.BackgroundColor == Color.White)
                    {

                        dynamicButton.BackgroundColor = Color.FromHex("#5bdcb6"); // Green
                        dynamicButton.TextColor = Color.White;

                        selectedExercises.Add(exerciseObject);
                    }

                    else
                    {
                        dynamicButton.BackgroundColor = Color.White;
                        dynamicButton.TextColor = Color.Black;

                        selectedExercises.Remove(exerciseObject);
                    }
                };

                if (firstLetter != FirstValue)
                {
                    firstLetter = FirstValue;

                    exerciseContent.Children.Add(labelBoxView, 0, OutGridRowCount);
                    exerciseContent.Children.Add(dynamicLabel, 0, OutGridRowCount);

                    OutGridRowCount = OutGridRowCount + 1;

                    exerciseContent.Children.Add(dynamicButton, 0, OutGridRowCount);

                    OutGridRowCount = OutGridRowCount + 1;
                }

                else
                {

                    exerciseContent.Children.Add(dynamicButton, 0, OutGridRowCount);

                    OutGridRowCount = OutGridRowCount + 1;
                }

            }
        
        }

        private async void addExercisesClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.RoutineInformationEdit(selectedExercises, UserList)); 
        }
    }
}