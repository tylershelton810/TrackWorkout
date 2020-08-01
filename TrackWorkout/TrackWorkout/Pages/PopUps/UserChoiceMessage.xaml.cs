using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class UserChoiceMessage : PopupPage
    {
        TaskCompletionSource<bool> _tcs = null;

        public UserChoiceMessage(string header, string body, string GreenButton, string RedButton)
        {            
            InitializeComponent();

            MessageHeader.Text = header;
            MessageHeader.FontFamily = App.CustomBold;
            MessageBody.Text = body;
            MessageBody.FontFamily = App.CustomRegular;
            Choice1Button.Text = GreenButton;
            Choice1Button.FontFamily = App.CustomRegular;
            Choice2Button.Text = RedButton;
            Choice2Button.FontFamily = App.CustomRegular;
        }
       
        void Button1_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage.Navigation.PopPopupAsync(true);
            _tcs?.SetResult(true);
        }

        void Button2_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage.Navigation.PopPopupAsync(true);
            _tcs?.SetResult(false);
        }

        public async Task<bool> Show()
        {
            _tcs = new TaskCompletionSource<bool>();
            await Navigation.PushPopupAsync(this);
            return await _tcs.Task;
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }

        //##  EXAMPLE OF HOW TO CALL ##
        //
        //
        //    var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", "Would you like to save your changes?", "Yes", "No");
        //    var result = await QuestionPop.Show();

        //    if (result) // Yes
        //    {
        //        SaveSettings();
        //    }
        //    else
        //    {
        //        await Navigation.PushAsync(new Pages.HomeScreen());
        //    }
    }
}
