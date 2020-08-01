using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackWorkout.Entitys;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Routines : MasterDetailPage
    {

        public Routines(List<UserInformation> userInformation)
        {
            InitializeComponent();           

            NavigationPage.SetHasNavigationBar(this, false);

            this.Master = new MasterNavigation(userInformation); // This is the nav menu
            this.Detail = new NavigationPage(new RoutinesDetail(userInformation))
            {
                 
                BarBackgroundColor = Color.FromHex("#5bc2dc")                
           
            }; // This is the Content of the page

            App.MasterDetail = this;
        }
    }
}