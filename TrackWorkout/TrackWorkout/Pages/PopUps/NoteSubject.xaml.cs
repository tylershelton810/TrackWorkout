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
    public partial class NoteSubject : PopupPage
    {
        TaskCompletionSource<string> _tcs = null;

        public NoteSubject()
        {            
            InitializeComponent();

            MessageHeader.FontFamily = App.CustomBold;
            MessageBody.FontFamily = App.CustomRegular;
            Choice1Button.FontFamily = App.CustomRegular;
            Choice2Button.FontFamily = App.CustomRegular;
        }

        async void Button1_Clicked(System.Object sender, System.EventArgs e)
        {
            if (MessageBody.Text != null)
            {
                await App.Current.MainPage.Navigation.PopPopupAsync(true);
                _tcs?.SetResult(MessageBody.Text);
            }
            else
            {
                var Pop = new Pages.PopUps.ErrorMessage("Error", "Please fill out a subject", "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
            }
        }

        void Button2_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage.Navigation.PopPopupAsync(true);
            _tcs?.SetResult("Cancelled");
        }

        public async Task<string> Show()
        {
            _tcs = new TaskCompletionSource<string>();
            await Navigation.PushPopupAsync(this);
            return await _tcs.Task;
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }
    }
}
