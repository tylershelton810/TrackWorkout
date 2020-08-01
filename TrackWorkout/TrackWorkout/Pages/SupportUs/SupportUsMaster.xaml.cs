using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.SupportUs
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SupportUsMaster : MasterDetailPage
    {

        public SupportUsMaster()
        {
            InitializeComponent();

            // Get rid of default navigation
            NavigationPage.SetHasNavigationBar(this, false);

            // Set up Navigation
            this.Master = new MasterNavigation(); // This is the nav menu
            this.Detail = new NavigationPage(new Pages.SupportUs.SupportUs())
            {
                BarBackgroundColor = App.PrimaryThemeColor
            }; // This is the Content of the page           
            App.MasterDetail = this;

        }
    }
}