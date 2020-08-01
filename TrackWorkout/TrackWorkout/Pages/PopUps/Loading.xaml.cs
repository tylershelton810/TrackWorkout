using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Loading : PopupPage
    {
        public Loading(string message, bool showLoading = true)
        {
            InitializeComponent();

            MessageText.FontFamily = App.CustomRegular;
            MessageText.Text = message;
            LoadingMessageDots.IsVisible = showLoading;
        }
        protected override bool OnBackgroundClicked()
        {
            return false;
        }
    }
}
