using System;
using Rg.Plugins.Popup.Extensions;
using TrackWorkout.Entitys;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {

        public Settings()
        {
            InitializeComponent();

            SaveButton.BackgroundColor = App.SecondaryThemeColor;
            SaveFrame.BackgroundColor = App.PrimaryThemeColor;
            ButtonGridBottom.BackgroundColor = App.PrimaryThemeColor;
            WeightTypeLabel.TextColor = App.PrimaryThemeColor;
            WeightTypeLabel.FontFamily = App.CustomRegular;
            ThemeColorLabel.TextColor = App.PrimaryThemeColor;
            ThemeColorLabel.FontFamily = App.CustomRegular;
            ThemeColorLabelSecondary.TextColor = App.SecondaryThemeColor;
            ThemeColorLabelSecondary.FontFamily = App.CustomRegular;
            LbsButton.FontFamily = App.CustomLight;
            KgsButton.FontFamily = App.CustomLight;

            foreach (ColorWheel Color in App.MyPallet)
            {
                InsertColorGrid(Color);
            }

            // Set the initial WeightType based on users current settings. 
            if (App.userInformationApp[0].WeightType == "Lbs")
            {
                LbsButton.BackgroundColor = App.SecondaryThemeColor;
                KgsButton.BackgroundColor = Color.LightGray;
            }
            else
            {
                KgsButton.BackgroundColor = App.SecondaryThemeColor;
                LbsButton.BackgroundColor = Color.LightGray;
            }
        }
        private void LbsButtonClicked(object sender, EventArgs e)
        {
            LbsButton.BackgroundColor = App.SecondaryThemeColor;
            KgsButton.BackgroundColor = Color.LightGray;
            App.userInformationApp[0].WeightType = "Lbs";
        }
        private void KgsButtonClicked(object sender, EventArgs e)
        {
            KgsButton.BackgroundColor = App.SecondaryThemeColor;
            LbsButton.BackgroundColor = Color.LightGray;
            App.userInformationApp[0].WeightType = "Kgs";
        }
        private void SaveClicked(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void InsertColorGrid(ColorWheel ColorToUse)
        {
            Button PrimaryColorButton = new Button
            {
                CornerRadius = 25,
                BorderColor = Color.DarkGray,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 50,
                WidthRequest = 50,
                BackgroundColor = ColorToUse.Hex,
                Opacity = ColorToUse.PrimaryOpacity
            };
            Button SecondaryColorButton = new Button
            {
                CornerRadius = 25,
                HeightRequest = 50,
                BorderColor = Color.DarkGray,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 50,
                BackgroundColor = ColorToUse.Hex,
                Opacity = ColorToUse.SecondaryOpacity
            };

            PrimaryColorButton.Clicked += (o, e) =>
            {
                foreach (var button in PrimaryColorWheelGrid.Children)
                {
                    button.Opacity = .4;
                    foreach (ColorWheel color in App.MyPallet)
                    {
                        color.PrimaryOpacity = .4;
                    }
                }

                PrimaryColorButton.Opacity = 1;
                App.MyPallet[ColorToUse.ColorCode].PrimaryOpacity = 1;
                App.userInformationApp[0].ThemeColor = ColorToUse.ColorCode;
                SaveFrame.BackgroundColor = PrimaryColorButton.BackgroundColor;
                ButtonGridBottom.BackgroundColor = PrimaryColorButton.BackgroundColor;
                WeightTypeLabel.TextColor = PrimaryColorButton.BackgroundColor;
                ThemeColorLabel.TextColor = PrimaryColorButton.BackgroundColor;
            };
            SecondaryColorButton.Clicked += (o, e) =>
            {
                foreach (var button in SecondaryColorWheelGrid.Children)
                {
                    button.Opacity = .4;
                    foreach (ColorWheel color in App.MyPallet)
                    {
                        color.SecondaryOpacity = .4;
                    }
                }

                SecondaryColorButton.Opacity = 1;
                App.MyPallet[ColorToUse.ColorCode].SecondaryOpacity = 1;
                App.userInformationApp[0].ThemeColor2 = ColorToUse.ColorCode;
                ThemeColorLabelSecondary.TextColor = SecondaryColorButton.BackgroundColor;
                SaveButton.BackgroundColor = SecondaryColorButton.BackgroundColor;
                if (LbsButton.BackgroundColor != Color.LightGray)
                {
                    LbsButton.BackgroundColor = SecondaryColorButton.BackgroundColor;
                }
                if (KgsButton.BackgroundColor != Color.LightGray)
                {
                    KgsButton.BackgroundColor = SecondaryColorButton.BackgroundColor;
                }
            };

            PrimaryColorWheelGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60, GridUnitType.Absolute) });
            PrimaryColorWheelGrid.Children.Add(PrimaryColorButton, ColorToUse.ColorCode, 0);

            SecondaryColorWheelGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60, GridUnitType.Absolute) });
            SecondaryColorWheelGrid.Children.Add(SecondaryColorButton, ColorToUse.ColorCode, 0);
        }

        private void SetPrimaryColor(Color ThemeToUse, int ColorCode)
        {
            App.userInformationApp[0].ThemeColor = ColorCode;
            SaveButton.BackgroundColor = ThemeToUse;
            WeightTypeLabel.TextColor = ThemeToUse;
            ThemeColorLabel.TextColor = ThemeToUse;
        }

        private void SetSecondaryColor(Color ThemeToUse, int ColorCode)
        {
            App.userInformationApp[0].ThemeColor2 = ColorCode;
            ThemeColorLabelSecondary.TextColor = ThemeToUse;
            if (LbsButton.BackgroundColor != Color.LightGray)
            {
                LbsButton.BackgroundColor = ThemeToUse;
            }
            if (KgsButton.BackgroundColor != Color.LightGray)
            {
                KgsButton.BackgroundColor = ThemeToUse;
            }
        }
        private async void SaveSettings()
        {
            if (App.userInformationApp[0].WeightType == "Kgs")
            {
                var Pop = new Pages.PopUps.InformationMessage("Coming soon!", $"The kilogram option is still in development", "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
            }
            else
            {
                var soundOne = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                soundOne.Load("chime1.wav");
                MainStack.IsEnabled = false;
                SaveButton.IsEnabled = false;

                try
                {
                    UserInformation UserInfoToPass = App.userInformationApp[0];

                    var LoadingPop = new Pages.PopUps.Loading("Updating Settings");
                    await App.Current.MainPage.Navigation.PushPopupAsync(LoadingPop, true);

                    string Response = await SharedClasses.WebserviceCalls.UpdateSettings(UserInfoToPass);

                    await App.Current.MainPage.Navigation.PopPopupAsync();

                    if (Response != "success")
                    {
                        var errorPop = new Pages.PopUps.ErrorMessage("Error", Response, "OK");
                        await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);
                    }
                    else
                    {
                        DoneAnimation.IsVisible = true;
                        DoneAnimation.Play();
                        //soundOne.Play();
                    }
                }
                catch (Exception ex)
                {
                    MainStack.IsEnabled = true;
                    SaveButton.IsEnabled = true;

                    var errorPop = new Pages.PopUps.ErrorMessage("Error", "SaveSettings | " + ex.Message.ToString(), "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(errorPop, true);

                    string error = ex.ToString();
                }
            }
        }

        private void AnimationComplete(object sender, EventArgs e)
        {
            //await Navigation.PopAsync();
            Navigation.PushAsync(new HomeScreen());
        }

        override protected bool OnBackButtonPressed()
        {
            LeavePageCheck();
            return true;
        }

        private async void LeavePageCheck()
        {
            var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", "Would you like to save your changes?", "Yes", "No");
            var result = await QuestionPop.Show();

            if (result) // Yes
            {
                SaveSettings();
            }
            else
            {
                await Navigation.PushAsync(new Pages.HomeScreen());
            }
        }
    }
}