using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Clients : MasterDetailPage
    {
        public Clients()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            this.Master = new MasterNavigation(); // This is the nav menu
            this.Detail = new NavigationPage(new ClientsDetail())
            {
                BarBackgroundColor = App.PrimaryThemeColor
            };  // This is the Content of the page

            App.MasterDetail = this;
        }
    }
}