using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorMessage : PopupPage
    {
        public ErrorMessage(string header, string body, string button)
        {            
            InitializeComponent();

            MessageHeader.Text = header;
            MessageHeader.FontFamily = App.CustomBold;
            MessageBody.Text = body;
            MessageBody.FontFamily = App.CustomRegular;
            MessageButton.Text = button;
            MessageButton.FontFamily = App.CustomRegular;
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
    }
}
