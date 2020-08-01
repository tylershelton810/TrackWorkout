using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.Routine
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutinesDetailBottomNavDetail : ContentPage
    {
        public RoutinesDetailBottomNavDetail()
        {
            InitializeComponent();

            //NavigationPage.SetHasNavigationBar(this, false);

            this.Master = new MasterNavigation(); // This is the nav menu
            this.Detail = new NavigationPage(new RoutinesDetail())
            {
                BarBackgroundColor = App.PrimaryThemeColor
            }; // This is the Content of the page

            App.MasterDetail = this;
        }
    }
}
