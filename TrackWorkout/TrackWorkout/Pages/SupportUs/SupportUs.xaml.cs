using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.SupportUs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SupportUs : ContentPage
    {
        public SupportUs()
        {
            InitializeComponent();

            ButtonGridBottom.BackgroundColor = App.PrimaryThemeColor;
            MainButtonFrame.BackgroundColor = App.PrimaryThemeColor;
            TeeSpringLabel.FontFamily = App.CustomBold;
            TeeSpringLabel.TextColor = App.PrimaryThemeColor;
            TeeSpringFrame.BackgroundColor = App.SecondaryThemeColor;
        }

        private async void TeeSpringCick(object sender, EventArgs e)
        {
            //Take the user to the TeeSpring Website
            await OpenBrowser(new Uri("https://teespring.com/stores/coachme"));
        }


        public async Task<bool> OpenBrowser(Uri uri)
        {
            return await Browser.OpenAsync(uri, new BrowserLaunchOptions
                                                {
                                                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                                                    TitleMode = BrowserTitleMode.Show,
                                                    PreferredToolbarColor = App.PrimaryThemeColor,
                                                    PreferredControlColor = App.ThemeColor4
                                                });
        }
    }
}