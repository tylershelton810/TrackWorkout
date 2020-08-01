using System;
using Xamarin.Forms;
using TrackWorkout.Entitys;
using Rg.Plugins.Popup.Extensions;

namespace TrackWorkout.Pages
{

    public partial class CreateUser : ContentPage
    {
        string SourceToPass;
        UserEmail userToPass;
        Boolean ShowPass;

        public CreateUser(UserEmail user, string Source)
        {
            InitializeComponent();

            //Fonts
            userNameLabel.FontFamily = App.CustomBold;
            ValidFormat.FontFamily = App.CustomRegular;
            userBirthdayLabel.FontFamily = App.CustomBold;
            weightPickerLabel.FontFamily = App.CustomBold;
            weightLabel.FontFamily = App.CustomBold;
            button.FontFamily = App.CustomRegular;
            userName.FontFamily = App.CustomRegular;
            userBirthday.FontFamily = App.CustomRegular;
            weightPicker.FontFamily = App.CustomRegular;
            weight.FontFamily = App.CustomRegular;            

            ShowPass = false;

            NavigationPage.SetHasNavigationBar(this, false);

            SourceToPass = Source;
            userToPass = user;
        }
        private void InsertNewUserClick(object sender, EventArgs e)
        {
            // This will need to be passed to the stored procedure in the near future
            int TypeOfWeight = 0;
            //Handle the user not selecting a Unit of weight
            try
            {
                //Error handling
                if (weightPicker.SelectedItem.ToString() == "Pounds")
                {
                    TypeOfWeight = 1;
                }
                else if (weightPicker.SelectedItem.ToString() == "Kilograms")
                {
                    TypeOfWeight = 2;
                }
            }
            catch
            {
                var pop = new Pages.PopUps.ErrorMessage("Invalid Entry", "Please select a Unit of Weight", "OK");
                App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
                return;
            }           
            if (userName.Text == null)
            {
                var pop = new Pages.PopUps.ErrorMessage("Invalid Entry", "Please type in your Name", "OK");
                App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
            }
            else if (weightPicker.SelectedItem.ToString() == "Kilograms")
            {
                TypeOfWeight = 2;
                var pop = new Pages.PopUps.InformationMessage("Coming soon!", "The kilogram option is still in development", "OK");
                App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
            }
            else if (TypeOfWeight == 0)
            {
                var pop = new Pages.PopUps.ErrorMessage("Invalid Entry", "lease select your unit of measurement for weight", "OK");
                App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
            }
            else if (weight.Text == null)
            {
                var pop = new Pages.PopUps.ErrorMessage("Invalid Entry", "Please enter your weight", "OK");
                App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
            }
            else if (ValidFormat.Text != "")
            {
                var pop = new Pages.PopUps.ErrorMessage("Invalid Entry", "Please enter a valid Birthday", "OK");
                App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
            }
            else
            {
                // Passed all error handling. Create the user object.
                Entitys.User CreatedUser = new Entitys.User { Name = userName.Text, Birthday = DateTime.Parse(userBirthday.Text), WeightType = TypeOfWeight, Email = App.loginToken.User.FindFirst(x => x.Type == "email").Value.ToString(), Weight = float.Parse(weight.Text)};
                if (SourceToPass == "From Google")
                {
                    CreatedUser.Source = "Google";
                    CreatedUser.Photo = userToPass.Photo;
                }

                //Pass the user to the database
                InsertNewUserProcess(CreatedUser);
            }
        }

        private async void InsertNewUserProcess(Entitys.User CreatedUser)
        {
            var LoadingPop = new Pages.PopUps.Loading("Creating User");
            await App.Current.MainPage.Navigation.PushPopupAsync(LoadingPop, true);

            (string subject, string body) = await SharedClasses.WebserviceCalls.SaveNewUser(CreatedUser);

            await App.Current.MainPage.Navigation.PopPopupAsync();

            if (subject == "Account Created")
            {
                // Display Successful Message
                var pop = new Pages.PopUps.InformationMessage(subject, body, "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);

                Entitys.UserEmail passEmail = new Entitys.UserEmail
                {
                    Email = App.loginToken.User.FindFirst(x => x.Type == "email").Value.ToString(),
                    Photo = App.loginToken.User.FindFirst(x => x.Type == "picture").Value.ToString()
                };

                LoadingPop = new Pages.PopUps.Loading("Retrieving Profile Data");
                await App.Current.MainPage.Navigation.PushPopupAsync(LoadingPop, true);

                string Response = await SharedClasses.WebserviceCalls.GetMasterData(passEmail);

                await App.Current.MainPage.Navigation.PopPopupAsync();

                if (Response == "success")
                {
                    //Go to Homescreen 
                    await Navigation.PushAsync(new HomeScreen());
                }
                else
                {
                    // Catch other errors
                    var errorPop = new Pages.PopUps.ErrorMessage("Error", Response, "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);
                }
            }
            else
            { 
                // Catch other errors
                var errorPop = new Pages.PopUps.ErrorMessage(subject, body, "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);
            }            
        }


    }
}
