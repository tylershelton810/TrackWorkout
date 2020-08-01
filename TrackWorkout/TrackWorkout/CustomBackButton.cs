using System;

using Xamarin.Forms;

namespace TrackWorkout
{
    public class CustomBackButton : ContentPage
    {
        public CustomBackButton()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

