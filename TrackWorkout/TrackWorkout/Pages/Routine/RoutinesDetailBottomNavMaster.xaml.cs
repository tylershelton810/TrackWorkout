using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.Routine
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutinesDetailBottomNavMaster : ContentPage
    {
        public ListView ListView;

        public RoutinesDetailBottomNavMaster()
        {
            InitializeComponent();

            BindingContext = new RoutinesDetailBottomNavMasterViewModel();
            ListView = MenuItemsListView;
        }

        class RoutinesDetailBottomNavMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<RoutinesDetailBottomNavMenuItem> MenuItems { get; set; }

            public RoutinesDetailBottomNavMasterViewModel()
            {
                MenuItems = new ObservableCollection<RoutinesDetailBottomNavMenuItem>(new[]
                {
                    new RoutinesDetailBottomNavMenuItem { Id = 0, Title = "Page 1" },
                    new RoutinesDetailBottomNavMenuItem { Id = 1, Title = "Page 2" },
                    new RoutinesDetailBottomNavMenuItem { Id = 2, Title = "Page 3" },
                    new RoutinesDetailBottomNavMenuItem { Id = 3, Title = "Page 4" },
                    new RoutinesDetailBottomNavMenuItem { Id = 4, Title = "Page 5" },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}
