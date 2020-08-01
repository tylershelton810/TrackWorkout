using System;

using Xamarin.Forms;

namespace TrackWorkout.Pages.Routine
{
    public class RoutineDetailBottomNav : ContentPage
    {
        public RoutineDetailBottomNav()
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

